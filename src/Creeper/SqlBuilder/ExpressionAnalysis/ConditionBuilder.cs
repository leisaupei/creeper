using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	internal class ConditionBuilder : ExpressionVisitor
	{
		/// <summary>
		/// string条件
		/// </summary>
		public string Condition { get; private set; }

		/// <summary>
		/// 参数
		/// </summary>
		public object[] Arguments { get; private set; }

		/// <summary>
		/// 数据库别名
		/// </summary>
		public string[] Alias { get; private set; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		private readonly DataBaseKind _dataBaseKind;
		/// <summary>
		/// 字段是否加引号
		/// </summary>
		private readonly bool _ifWithQuotationMarks = false;
		private readonly List<object> _arguments = new List<object>();
		private readonly HashSet<string> _alias = new HashSet<string>();
		private readonly Stack<string> _conditionParts = new Stack<string>();

		public ConditionBuilder(DataBaseKind dataBaseKind)
		{
			_dataBaseKind = dataBaseKind;
			_ifWithQuotationMarks = dataBaseKind.GetWithQuotationMarks();
		}

		public void Build(Expression expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var evaluator = new PartialEvaluator();
			var evaluatedExpression = evaluator.Eval(expression);

			Visit(evaluatedExpression);

			Arguments = _arguments.ToArray();
			Condition = _conditionParts.Count > 0 ? _conditionParts.Pop() : null;
			Alias = new string[_alias.Count];
			_alias.CopyTo(Alias);
		}

		protected override Expression VisitUnary(UnaryExpression expression)
		{
			if (expression == null) return expression;
			// !xxx.Equal("") 非表达式语句
			if (expression.NodeType == ExpressionType.Not) _conditionParts.Push("NOT");
			if (expression.NodeType == ExpressionType.ArrayLength)
			{
				_conditionParts.Push("array_length(");
				Visit(expression.Operand);
				_conditionParts.Push(",1)");
				MergeConditionParts("{0}{1}{2}");
				return expression;
			}
			//这里只是添加NOT标记, 直接请求父类的访问方法继续递归
			return base.VisitUnary(expression);
		}
		protected override Expression VisitBinary(BinaryExpression expression)
		{
			if (expression == null) return expression;
			//表达式是否包含转换类型的表达式, 一般枚举类型运算需要用到
			if (new[] { expression.Left, expression.Right }.Any(a => a.NodeType == ExpressionType.Convert))
			{
				if (expression.Left.NodeType == ExpressionType.Convert)
					VisitConvert((UnaryExpression)expression.Left, expression.Right, true);
				else
					VisitConvert((UnaryExpression)expression.Right, expression.Left, false);
			}
			else if (expression.NodeType == ExpressionType.ArrayIndex) //array[1]表达式包含数组索引
			{
				if (expression.Left.NodeType == ExpressionType.MemberAccess)
				{
					Visit(expression.Left);

					//数据库索引从1开始
					_conditionParts.Push(string.Concat("[", (int)expression.Right.GetExpressionValue() + 1, "]"));
					MergeConditionParts();
					return expression;
				}
			}
			else
			{
				Visit(expression.Left);
				Visit(expression.Right);
			}
			var right = _conditionParts.Pop();
			var left = _conditionParts.Pop();

			string cond = expression.GetCondition(left, right);

			if (cond != null) _conditionParts.Push(cond);
			return expression;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression == null) return expression;

			if (expression.Value == null)
				_conditionParts.Push(null);

			else
			{
				_arguments.Add(expression.Value);
				string cond = null;
				if (expression.IsIListImplementation()) //如果是list/array类型
				{
					if (expression.Type.GetElementType() == typeof(string)) //如果是字符串数据, 部分数据库需要强制转换
						cond = string.Format("CAST({{{0}}} AS {1}[])", _arguments.Count - 1, _dataBaseKind.GetCastBaseStringDbType());
				}

				if (cond == null) cond = string.Format("{{{0}}}", _arguments.Count - 1);

				if (cond != null) _conditionParts.Push(cond);
			}
			return expression;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (expression == null) return expression;

			var propertyInfo = expression.Member as PropertyInfo;
			if (propertyInfo == null) return expression;

			if (expression.Expression.NodeType == ExpressionType.Parameter)
			{
				var p = expression.Expression.ToString();

				// 返回当前表达式里面所有的参数
				if (!_alias.Contains(p)) _alias.Add(p);
			}

			// 返回数据库成员字段
			if (_ifWithQuotationMarks) //是否添加引号
			{
				_conditionParts.Push(string.Format("{0}", expression.GetOriginExpression().ToDatebaseField()));
			}
			else
			{
				_conditionParts.Push(string.Format("{0}", expression.GetOriginExpression()));
			}

			return expression;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			if (expression == null) return expression;

			string connectorWords = _dataBaseKind.GetStrConnectWords(); //获取字符串连接符
			bool useDefault = true; //是否使用默认表达式访问方式

			string format = null;
			var not = _conditionParts.TryPop(out var result) ? result : string.Empty; //非运算符
			switch (expression.Method.Name)
			{
				case "StartsWith": //Like 'xxx%',
					format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(expression.Arguments), " ''", connectorWords, "{1}", connectorWords, "'%'");
					break;

				case "Contains": //Like '%xxx%',
					if (expression.Object?.Type == typeof(string)) //如果是String.Contains, 那么使用like
						format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(expression.Arguments), " '%'", connectorWords, "{1}", connectorWords, "'%'");

					//其他情况使用 IList.Contains
					else
					{
						useDefault = false;
						var opr = not == "NOT" ? "<>" : "=";
						format = string.Concat("{0} ", opr, " {1}");
						var method = not == "NOT" ? "ALL" : "ANY";

						for (int i = 0; i < expression.Arguments.Count; i++) //遍历Visit成员表达式
						{
							Expression arg = expression.Arguments[i];
							if (arg.IsIListImplementation()) //如果当前表达式是ILIst
								format = format.Replace($"{{{i}}}", $"{method}({{{i}}})");
							Visit(arg);
						}
						if (!format.StartsWith("{0}")) //ALL/ANY只能在操作符后面, 这里添加前后对调方法, 用PostgreSql为例
						{
							var conds = format.Split($" {opr} ").Reverse();
							format = string.Join($" {opr} ", conds);
						}
					}
					break;

				case "EndsWith": //Like '%xxx',
					format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(expression.Arguments), " '%'", connectorWords, "{1}", connectorWords, "''");
					break;

				case "Equals":
					format = string.Concat("({0} ", not == "NOT" ? "<>" : "=", " {1})");
					break;

				case "ToString": //a.Name.ToString()=>(CAST a.Name AS VARCHAR)
					if (expression.Object.NodeType != ExpressionType.MemberAccess) goto default;
					useDefault = false;
					_conditionParts.Push("CAST(");
					VisitMember((MemberExpression)expression.Object);
					_conditionParts.Push(string.Format(" AS {0})", _dataBaseKind.GetCastBaseStringDbType()));
					MergeConditionParts();
					break;

				default: //如果没有特殊的方法解析, 直接返回方法的返回值, 用常量表达式Visit
					useDefault = false;
					var constantExpression = expression.GetConstantFromExression(expression.Type);
					VisitConstant(constantExpression);
					break;
					//throw new NotSupportedException(expression.NodeType + " is not supported!");
			}

			if (useDefault)
			{
				Visit(expression.Object);
				Visit(expression.Arguments[0]);
			}

			if (format != null) MergeConditionParts(format);

			return expression;
		}

		/// <summary>
		/// 忽略大小写用ILIKE, 否则用LIKE
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private string IgnoreCaseConvert(ReadOnlyCollection<Expression> arguments)
		{
			var ignoreCase = VisitStringContainCulture(arguments);
			return ignoreCase ? "ILIKE" : "Like";
		}

		/// <summary>
		/// 检查模糊查询是否忽略大小写
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private bool VisitStringContainCulture(ReadOnlyCollection<Expression> arguments)
		{
			if (arguments.Count != 2) return false;
			if (arguments[1].ToString().EndsWith("IgnoreCase")) return true;
			return false;
		}

		/// <summary>
		/// 访问转换类型的表达式, 常用于枚举类型转换
		/// </summary>
		/// <param name="unaryExpression">数据库成员一元表达式</param>
		/// <param name="expression">包含变量输出的表达式</param>
        /// <param name="unaryFirst">判断表达式左右顺序</param>
		/// <returns>是否完成解析标志</returns>
		private bool VisitConvert(UnaryExpression unaryExpression, Expression expression, bool unaryFirst)
		{
			//获取成员表达式类型, 如果是枚举类型就继续, 否则结束
			var unaryExpressionOperandType = unaryExpression.Operand.Type.GetOriginalType();
			if (!unaryExpressionOperandType.IsEnum) return false;

			//因为枚举在lambda表达式会转化为int类型, 则不是int类型就结束
			var unaryExpressionType = unaryExpression.Type.GetOriginalType();
			if (unaryExpressionType != typeof(int)) return false;

			//如果是常量则直接输出, 否则输出表达式的值
			object expressionValue = expression is ConstantExpression ce ? ce.Value : expression.GetExpressionValue();

			//获取int值枚举
			var enumValue = Enum.ToObject(unaryExpressionOperandType, expressionValue);

			//以枚举类型包装常量表达式
			var constantExpression = Expression.Constant(enumValue, unaryExpressionOperandType);

			//下面是调整lambda表达式保持一致
			if (unaryFirst)
			{
				VisitUnary(unaryExpression);
				VisitConstant(constantExpression);
			}
			else
			{
				VisitConstant(constantExpression);
				VisitUnary(unaryExpression);
			}
			return true;
		}

		/// <summary>
		/// 按照格式合并所有条件部件
		/// </summary>
		/// <param name="format"></param>
		private void MergeConditionParts(string format)
		{
			var length = _conditionParts.Count;
			var parts = new string[length];

			for (int i = length - 1; i > -1; i--) parts[i] = _conditionParts.Pop();

			if (format != null) _conditionParts.Push(string.Format(format, parts));
		}

		/// <summary>
		/// 连接所有条件部件
		/// </summary>
		private void MergeConditionParts()
		{
			var connect = string.Concat(_conditionParts.Reverse());
			_conditionParts.Clear();
			_conditionParts.Push(connect);
		}
		
	}
}
