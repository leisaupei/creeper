using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
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
		public static string ToDatebaseField(this MemberExpression mb)
			=> string.Concat(mb.ToString().ToLower().Replace(".", ".\""), '"');

		/// <summary>
		/// 递归member表达式, 针对optional字段, 从 a.xxx.Value->a.xxx
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static MemberExpression GetOriginExpression(this MemberExpression node)
		{
			if (node.NodeType == ExpressionType.MemberAccess && node.Expression is MemberExpression me)
				return GetOriginExpression(me);
			return node;
		}

		/// <summary>
		/// 获取是否字段加双引号
		/// </summary>
		/// <param name="databaseType"></param>
		/// <returns></returns>
		public static bool GetWithQuotationMarks(this DataBaseKind dataBaseKind)
			=> dataBaseKind switch
			{
				DataBaseKind.PostgreSql or DataBaseKind.Oracle => true,
				_ => false,
			};

		/// <summary>
		/// 运算符转换
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string OperatorCast(this ExpressionType type)
			=> ExpressionOperator.TryGetValue(type, out var value)
			? value
			: throw new NotSupportedException(type + "is not supported.");

		/// <summary>
		/// 空与非空运算符转换
		/// </summary>
		/// <param name="opr"></param>
		/// <returns></returns>
		public static string NullableOperatorCast(this ExpressionType type)
			=> type == ExpressionType.Equal
			? "IS NULL"
			: (type == ExpressionType.NotEqual ? "IS NOT NULL" : type.OperatorCast());

		/// <summary>
		/// 空与非空运算符转换
		/// </summary>
		/// <param name="opr"></param>
		/// <returns></returns>
		public static bool IsBracketsExpressionType(this ExpressionType type)
			=> type == ExpressionType.AndAlso || type == ExpressionType.OrElse;

		/// <summary>
		/// 获得like语句链接符
		/// </summary>
		/// <param name="databaseType"></param>
		/// <returns></returns>
		public static string GetStrConnectWords(this DataBaseKind dataBaseKind)
			=> dataBaseKind switch
			{
				DataBaseKind.PostgreSql or DataBaseKind.Oracle or DataBaseKind.MySql or DataBaseKind.Sqlite => "||",
				_ => "+",
			};

		/// <summary>
		/// 转换数据库主从
		/// </summary>
		/// <param name="databaseType"></param>
		/// <returns></returns>
		public static string ChangeDataBaseKind(this DataBaseType dataBaseType, string dbName)
		{
			if (dataBaseType == DataBaseType.Secondary && dbName.EndsWith(DataBaseType.Main.ToString()))
				dbName = dbName.Replace(DataBaseType.Main.ToString(), DataBaseType.Secondary.ToString());

			if (dataBaseType == DataBaseType.Main && dbName.EndsWith(DataBaseType.Secondary.ToString()))
				dbName = dbName.Replace(DataBaseType.Secondary.ToString(), DataBaseType.Main.ToString());

			return dbName;
		}

		/// <summary>
		/// 其获取表达式类型并拼接左右条件
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="right"></param>
		/// <param name="left"></param>
		/// <returns></returns>
		public static string GetCondition(this BinaryExpression expression, string left, string right)
		{
			string cond = null;
			switch (expression.NodeType)
			{
				case ExpressionType.Coalesce:
					cond = string.Format("COALESCE({0},{1})", left.Trim(), right);
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
		/// 获取字符串数据转换类型
		/// </summary>
		/// <param name="dataBaseKind"></param>
		/// <returns></returns>
		public static string GetCastBaseStringDbType(this DataBaseKind dataBaseKind)
		 => dataBaseKind switch
		 {
			 DataBaseKind.PostgreSql or DataBaseKind.MySql => "VARCHAR",
			 _ => "",
		 };

		/// <summary>
		/// 获取表达式是否可用的集合类型
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static bool IsIListImplementation(this Expression expression)
		{
			return typeof(IList).IsAssignableFrom(expression.Type);
		}

		/// <summary>
		/// 把表达式转换为常量表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ConstantExpression GetConstantFromExression(this Expression expression, Type constantType)
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