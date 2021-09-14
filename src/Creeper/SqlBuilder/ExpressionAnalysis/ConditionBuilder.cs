using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections;
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

		private readonly List<object> _arguments = new List<object>();
		private readonly HashSet<string> _alias = new HashSet<string>();
		private readonly Stack<string> _conditionParts = new Stack<string>();
		private readonly CreeperConverter _converter;

		public ConditionBuilder(CreeperConverter converter)
		{
			_converter = converter;
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

		protected override Expression VisitNew(NewExpression node)
		{
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				Visit(node.Arguments[i]);
				if (i != node.Arguments.Count - 1)
					_conditionParts.Push(", ");
			}
			MergeConditionParts();
			return node;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node == null) return node;
			// !xxx.Equal("") 非表达式语句
			if (node.NodeType == ExpressionType.Not) _conditionParts.Push("NOT");
			if (node.NodeType == ExpressionType.ArrayLength)
			{
				_conditionParts.Push("array_length(");
				Visit(node.Operand);
				_conditionParts.Push(",1)");
				MergeConditionParts("{0}{1}{2}");
				return node;
			}
			//这里只是添加NOT标记, 直接请求父类的访问方法继续递归
			return base.VisitUnary(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node == null) return node;
			var isVisit = false;
			//表达式是否包含转换类型的表达式, 一般枚举类型运算需要用到
			if (node.Left.NodeType == ExpressionType.Convert) isVisit = VisitConvert((UnaryExpression)node.Left, node.Right, true);

			else if (node.Right.NodeType == ExpressionType.Convert) isVisit = VisitConvert((UnaryExpression)node.Right, node.Left, false);

			else if (node.NodeType == ExpressionType.ArrayIndex && node.Left.NodeType == ExpressionType.MemberAccess) //array[1]表达式包含数组索引
			{
				Visit(node.Left);

				//数据库索引从1开始
				_conditionParts.Push(string.Concat("[", (int)node.Right.GetExpressionValue() + 1, "]"));
				MergeConditionParts();
				return node;
			}
			if (!isVisit) //如果没有被解析, 那么使用通用解析方法
			{
				Visit(node.Left);
				Visit(node.Right);
			}
			var right = _conditionParts.Pop();
			var left = _conditionParts.Pop();

			string cond = node.GetCondition(left, right, _converter);

			if (cond != null) _conditionParts.Push(cond);
			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node == null) return node;

			if (node.Value == null)
				_conditionParts.Push(null);

			else
			{
				_arguments.Add(node.Value);
				string cond = null;
				if (typeof(IList).IsAssignableFrom(node.Type)) //如果是list/array类型
				{
					if (node.Type.GetElementType() == typeof(string)) //如果是字符串数据, 部分数据库需要强制转换
					{
						if (_converter.DataBaseKind != DataBaseKind.PostgreSql)
							throw new CreeperNotSupportedException("暂不支持PostgreSql以外的数据库的数组操作");
						cond = string.Format("CAST({{{0}}} AS VARCHAR[])", _arguments.Count - 1); // 目前只有postgresql会执行到此处所以VARCHAR[]先写死
					}
				}

				if (cond == null) cond = $"{{{_arguments.Count - 1}}}";

				if (cond != null) _conditionParts.Push(cond);
			}
			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node == null) return node;

			var propertyInfo = node.Member as PropertyInfo;
			if (propertyInfo == null) return node;

			if (node.Expression.NodeType == ExpressionType.Parameter)
			{
				var p = node.Expression.ToString();

				// 返回当前表达式里面所有的参数
				if (!_alias.Contains(p)) _alias.Add(p);
			}

			// 返回数据库成员字段
			_conditionParts.Push(string.Format("{0}", node.GetOriginExpression().ToDatebaseField(_converter)));

			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node == null) return node;

			string connector = _converter.StringConnectWord; //获取字符串连接符
			bool useDefault = true; //是否使用默认表达式访问方式

			string format = null;
			bool isNot = false;
			if (_conditionParts.Count > 0)
			{
				isNot = _conditionParts.Peek() == "NOT";
				if (isNot) _conditionParts.Pop();
			}
			switch (node.Method.Name)
			{
				case "StartsWith": //Like 'xxx%',
					format = string.Format(_converter.ExplainLike(VisitStringContainCulture(node.Arguments), isNot), "{0}", string.Concat("{1}", connector, "'%'"));
					break;

				case "Contains": //Like '%xxx%',
					if (node.Object?.Type == typeof(string)) //如果是String.Contains, 那么使用like
					{
						format = string.Format(_converter.ExplainLike(VisitStringContainCulture(node.Arguments), isNot), "{0}", string.Concat("'%'", connector, "{1}", connector, "'%'"));
						break;
					}
					//其他情况使用 IEnumerable.Contains

					useDefault = false;
					MemberExpression memberExpression = null;
					Expression arrayExpression = null;

					for (int i = 0; i < node.Arguments.Count; i++) //遍历Visit成员表达式
					{
						if (node.Arguments[i] is UnaryExpression ue && ue.Operand is MemberExpression me1)
							memberExpression = me1;
						else if (node.Arguments[i] is MemberExpression me2)
							memberExpression = me2;
						else
							arrayExpression = node.Arguments[i];
					}
					var memberIsArray = false;
					VisitMember(memberExpression);
					if (typeof(IEnumerable).IsAssignableFrom(arrayExpression.Type) && arrayExpression.Type != typeof(string))
					{
						if (_converter.MergeArray)
						{
							if (!typeof(IList).IsAssignableFrom(arrayExpression.Type)) //因为数据库不支持非IList类型的集合, 所以要转一下
							{
								var obj = arrayExpression.GetExpressionValue(); //获取表达式的值
								obj = obj.GetType().GetMethod("ToArray").Invoke(obj, new object[0]); //调用ToArray()方法

								arrayExpression = Expression.Constant(obj);
							}
							Visit(arrayExpression);
						}
						else
						{
							var values = arrayExpression.GetExpressionValue();
							foreach (var item in (IEnumerable)values)
							{
								_arguments.Add(item);
								_conditionParts.Push($"{{{_arguments.Count - 1}}}");
							}
						}
					}
					else
					{
						memberIsArray = true;
						Visit(arrayExpression);
					}

					var ps = PopAllParts();
					var cond = _converter.ExplainAny(ps[0], isNot, ps[1..], memberIsArray);
					_conditionParts.Push(cond);

					break;

				case "EndsWith": //Like '%xxx',
					format = string.Format(_converter.ExplainLike(VisitStringContainCulture(node.Arguments), isNot), "{0}", string.Concat("'%'", connector, "{1}"));
					break;

				case "Equals":
					format = string.Concat("({0} ", isNot ? "<>" : "=", " {1})");
					break;

				case "ToString": //a.Name.ToString()=>(CAST a.Name AS VARCHAR)
					if (node.Object.NodeType != ExpressionType.MemberAccess) goto default;
					useDefault = false;
					VisitMember((MemberExpression)node.Object);
					_conditionParts.Push(_converter.CastStringDataType(_conditionParts.Pop()));
					break;

				default: //如果没有特殊的方法解析, 直接返回方法的返回值, 用常量表达式Visit
					useDefault = false;
					var constantExpression = node.GetConstantFromExpression(node.Type);
					VisitConstant(constantExpression);
					break;
			}

			if (useDefault)
			{
				Visit(node.Object);
				Visit(node.Arguments[0]);
			}

			if (format != null) MergeConditionParts(format);

			return node;
		}

		/// <summary>
		/// 检查模糊查询是否忽略大小写, 默认忽略大小写
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private bool VisitStringContainCulture(ReadOnlyCollection<Expression> arguments)
		{
			if (arguments.Count != 2) return true;
			if (!arguments[1].ToString().EndsWith("IgnoreCase")) return false;
			return true;
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
			string[] parts = PopAllParts();

			if (format != null) _conditionParts.Push(string.Format(format, parts));
		}

		private string[] PopAllParts()
		{
			var length = _conditionParts.Count;
			var parts = new string[length];

			for (int i = length - 1; i > -1; i--) parts[i] = _conditionParts.Pop();
			return parts;
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
