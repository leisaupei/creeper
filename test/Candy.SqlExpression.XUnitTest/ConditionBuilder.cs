using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Candy.SqlExpression.XUnitTest
{
	internal class ConditionBuilder : ExpressionVisitor
	{
		/// <summary>
		/// 数据库类型
		/// </summary>
		private readonly DataBaseType _dataBaseType;
		/// <summary>
		/// 字段是否加引号
		/// </summary>
		private readonly bool _ifWithQuotationMarks = false;

		private readonly List<object> _arguments = new List<object>();
		private readonly Stack<string> _conditionParts = new Stack<string>();

		public ConditionBuilder(DataBaseType dataBaseType)
		{
			_dataBaseType = dataBaseType;
			_ifWithQuotationMarks = GetWithQuotationMarks(dataBaseType);
		}

		public string Condition { get; private set; }

		public object[] Arguments { get; private set; }


		#region 加双引号
		/// <summary>
		/// 加双引号
		/// </summary>
		/// <param name="str">字串</param>
		/// <returns></returns>
		public static string AddQuotationMarks(string str)
			=> string.IsNullOrEmpty(str) ? string.Empty : string.Concat('"', str.Trim(), '"');


		#endregion


		/// <summary>
		/// 获取是否字段加双引号
		/// </summary>
		/// <param name="databaseType"></param>
		/// <returns></returns>
		public static bool GetWithQuotationMarks(DataBaseType databaseType) => databaseType switch
		{
			DataBaseType.PostgreSql or DataBaseType.Oracle => true,
			_ => false,
		};

		public void Build(Expression expression)
		{
			var evaluator = new PartialEvaluator();
			var evaluatedExpression = evaluator.Eval(expression);

			Visit(evaluatedExpression);

			Arguments = _arguments.ToArray();
			Condition = _conditionParts.Count > 0 ? _conditionParts.Pop() : null;
		}

		protected override Expression VisitBinary(BinaryExpression expression)
		{
			if (expression == null) return expression;

			var opr = expression.NodeType switch
			{
				ExpressionType.Equal => "=",
				ExpressionType.NotEqual => "<>",
				ExpressionType.GreaterThan => ">",
				ExpressionType.GreaterThanOrEqual => ">=",
				ExpressionType.LessThan => "<",
				ExpressionType.LessThanOrEqual => "<=",
				ExpressionType.AndAlso => "AND",
				ExpressionType.OrElse => "OR",
				ExpressionType.Add => "+",
				ExpressionType.Subtract => "-",
				ExpressionType.Multiply => "*",
				ExpressionType.Divide => "/",
				_ => throw new NotSupportedException(expression.NodeType + "is not supported."),
			};

			Visit(expression.Left);
			Visit(expression.Right);

			var right = _conditionParts.Pop();
			var left = _conditionParts.Pop();
			var condition = string.Format("({0} {1} {2})", left, opr, right);
			_conditionParts.Push(condition);

			return expression;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression == null) return expression;


			_arguments.Add(expression.Value);
			_conditionParts.Push(string.Format("{{{0}}}", _arguments.Count - 1));
			return expression;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (expression == null) return expression;

			var propertyInfo = expression.Member as PropertyInfo;
			if (propertyInfo == null) return expression;

			//   this.m_conditionParts.Push(String.Format("(Where.{0})", propertyInfo.Name));
			//是否添加引号
			if (_ifWithQuotationMarks)
			{
				_conditionParts.Push(string.Format(" {0} ", AddQuotationMarks(propertyInfo.Name)));
			}
			else
			{
				// this.m_conditionParts.Push(String.Format("[{0}]", propertyInfo.Name));
				_conditionParts.Push(string.Format(" {0} ", propertyInfo.Name));
			}

			return expression;
		}
		#region 其他
		private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type)
		{
			string sb = "(";
			//先处理左边
			sb += ExpressionRouter(left);

			sb += ExpressionTypeCast(type);

			//再处理右边
			var tmpStr = ExpressionRouter(right);

			if (tmpStr == "null")
			{
				if (sb.EndsWith(" ="))
					sb = sb[0..^1] + " is null";
				else if (sb.EndsWith("<>"))
					sb = sb[0..^2] + " is not null";
			}
			else
				sb += tmpStr;
			return sb += ")";
		}

		private static string ExpressionRouter(Expression expression)
		{
			switch (expression)
			{
				case BinaryExpression be:
					return BinarExpressionProvider(be.Left, be.Right, be.NodeType);

				case MemberExpression me:
					return me.Member.Name;

				case NewArrayExpression ae:
					StringBuilder tmpstr = new();
					foreach (var ex in ae.Expressions)
					{
						tmpstr.Append(ExpressionRouter(ex));
						tmpstr.Append(',');
					}
					return tmpstr.ToString(0, tmpstr.Length - 1);

				case MethodCallExpression mce:
					switch (mce.Method.Name)
					{
						case "Like": return string.Format("({0} like {1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

						case "NotLike": return string.Format("({0} Not like {1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

						case "In": return string.Format("{0} In ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

						case "NotIn": return string.Format("{0} Not In ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

						case "StartWith": return string.Format("{0} like '{1}%'", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
					}
					break;

				case ConstantExpression ce:

					if (ce.Value == null)
						return "null";

					else if (ce.Value is ValueType)
						return ce.Value.ToString();

					else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
						return string.Format("'{0}'", ce.Value.ToString());
					break;

				case UnaryExpression ue:
					return ExpressionRouter(ue.Operand);
			}
			return null;
		}

		private static string ExpressionTypeCast(ExpressionType type) => type switch
		{
			ExpressionType.And or ExpressionType.AndAlso => " AND ",
			ExpressionType.Equal => " =",
			ExpressionType.GreaterThan => " >",
			ExpressionType.GreaterThanOrEqual => ">=",
			ExpressionType.LessThan => "<",
			ExpressionType.LessThanOrEqual => "<=",
			ExpressionType.NotEqual => "<>",
			ExpressionType.Or or ExpressionType.OrElse => " Or ",
			ExpressionType.Add or ExpressionType.AddChecked => "+",
			ExpressionType.Subtract or ExpressionType.SubtractChecked => "-",
			ExpressionType.Divide => "/",
			ExpressionType.Multiply or ExpressionType.MultiplyChecked => "*",
			_ => null,
		};
		#endregion


		/// <summary>
		/// ConditionBuilder 并不支持生成Like操作，如 字符串的 StartsWith，Contains，EndsWith 并不能生成这样的SQL： Like ‘xxx%’, Like ‘%xxx%’ , Like ‘%xxx’ . 只要override VisitMethodCall 这个方法即可实现上述功能。
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			string connectorWords = GetLikeConnectorWords(_dataBaseType); //获取like链接符

			if (expression == null) return expression;

			string format = expression.Method.Name switch
			{
				"StartsWith" => string.Concat("({0} LIKE ''", connectorWords, "{1}", connectorWords, "'%')"),
				"Contains" => string.Concat("({0} LIKE '%'", connectorWords, "{1}", connectorWords, "'%')"),
				"EndsWith" => string.Concat("({0} LIKE '%'", connectorWords, "{1}", connectorWords, "'')"),
				"Equals" => "({0} {1})",// not in 或者  in 或 not like
				_ => throw new NotSupportedException(expression.NodeType + " is not supported!"),
			};

			Visit(expression.Object);
			Visit(expression.Arguments[0]);

			string right = _conditionParts.Pop();
			string left = _conditionParts.Pop();

			_conditionParts.Push(string.Format(format, left, right));
			return expression;
		}

		/// <summary>
		/// 获得like语句链接符
		/// </summary>
		/// <param name="databaseType"></param>
		/// <returns></returns>
		public static string GetLikeConnectorWords(DataBaseType databaseType)
		{
			return databaseType switch
			{
				DataBaseType.PostgreSql or DataBaseType.Oracle or DataBaseType.MySql or DataBaseType.Sqlite => "||",
				_ => "+",
			};
		}

	}
}
