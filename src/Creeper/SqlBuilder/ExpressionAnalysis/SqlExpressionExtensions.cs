using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	internal static class SqlExpressionExtensions
	{
		/// <summary>
		/// 表达式节点类型转化操作符
		/// </summary>
		public static readonly IReadOnlyDictionary<ExpressionType, string> ExpressionOperator = new Dictionary<ExpressionType, string>
		{
			[ExpressionType.And] = "AND",
			[ExpressionType.AndAlso] = "AND",
			[ExpressionType.Equal] = "=",
			[ExpressionType.GreaterThan] = ">",
			[ExpressionType.GreaterThanOrEqual] = ">=",
			[ExpressionType.LessThan] = "<",
			[ExpressionType.LessThanOrEqual] = "<=",
			[ExpressionType.NotEqual] = "<>",
			[ExpressionType.Or] = "OR",
			[ExpressionType.OrElse] = "OR",
			[ExpressionType.Add] = "+",
			[ExpressionType.AddChecked] = "+",
			[ExpressionType.Subtract] = "-",
			[ExpressionType.SubtractChecked] = "-",
			[ExpressionType.Divide] = "/",
			[ExpressionType.Multiply] = "*",
			[ExpressionType.MultiplyChecked] = "*",

		};

		/// <summary>
		/// 成员表达式改成数据库字段 a.Xxx-> a."xxx"
		/// </summary>
		/// <param name="mb"></param>
		/// <returns></returns>
		public static string ToDatebaseField(this MemberExpression mb, CreeperConverter converter) => string.Concat(mb.Expression, '.', converter.WithQuote(converter.CaseInsensitiveTranslator(mb.Member.Name)));

		/// <summary>
		/// 递归member表达式, 针对optional字段, 从 a.xxx.Value->a.xxx
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static MemberExpression GetOriginExpression(this MemberExpression node) => node.NodeType == ExpressionType.MemberAccess && node.Expression is MemberExpression me ? me.GetOriginExpression() : node;

		/// <summary>
		/// 运算符转换
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string OperatorCast(this ExpressionType type)
			=> ExpressionOperator.TryGetValue(type, out var value)
			? value
			: throw new CreeperNotSupportedException(type + "is not supported.");

		/// <summary>
		/// 空与非空运算符转换
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string NullableOperatorCast(this ExpressionType type) => type == ExpressionType.Equal ? "IS NULL" : (type == ExpressionType.NotEqual ? "IS NOT NULL" : type.OperatorCast());

		/// <summary>
		/// 空与非空运算符转换
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsBracketsExpressionType(this ExpressionType type) => type == ExpressionType.AndAlso || type == ExpressionType.OrElse;

		/// <summary>
		/// 其获取表达式类型并拼接左右条件
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="right"></param>
		/// <param name="left"></param>
		/// <returns></returns>
		public static string GetCondition(this BinaryExpression expression, string left, string right, CreeperConverter converter)
		{
			string cond = null;
			switch (expression.NodeType)
			{
				case ExpressionType.Coalesce:
					cond = converter.CallCoalesce(left.Trim(), right, null);
					break;

				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Divide:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					{
						if (left == null) cond = string.Format("{0} {1}", right.Trim(), expression.NodeType.NullableOperatorCast());

						else if (right == null) cond = string.Format("{0} {1}", left.Trim(), expression.NodeType.NullableOperatorCast());

						else
						{
							cond = string.Format("{0} {1} {2}", left.Trim(), expression.NodeType.OperatorCast(), right.Trim());

							if (expression.NodeType.IsBracketsExpressionType())
								cond = string.Concat('(', cond, ')');
						}
					}
					break;

				default:
					break;
			}

			return cond;
		}

		/// <summary>
		/// 把表达式转换为常量表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="constantType">常量表达式的类型</param>
		/// <returns></returns>
		public static ConstantExpression GetConstantFromExpression(this Expression expression, Type constantType)
		{
			var obj = expression.GetExpressionValue();
			return Expression.Constant(obj, constantType);
		}

		/// <summary>
		/// 直接输出表达式的值
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static object GetExpressionValue(this Expression expression)
			=> Expression.Lambda(expression).Compile().DynamicInvoke();
	}
}