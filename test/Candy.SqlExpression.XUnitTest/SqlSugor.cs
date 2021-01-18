using System;
using System.Linq.Expressions;

namespace Candy.SqlExpression.XUnitTest
{
	/// <summary>
	/// lambda表达式转为where条件sql
	/// </summary>
	public class SqlSugor
	{
		#region Expression 转成 where
		/// <summary>
		/// Expression 转成 Where String
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="predicate"></param>
		/// <param name="databaseType">数据类型（用于字段是否加引号）</param>
		/// <returns></returns>
		public static string GetWhereByLambda<T>(Expression<Func<T, bool>> predicate, DataBaseType databaseType)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(databaseType);
			conditionBuilder.Build(predicate);

			for (int i = 0; i < conditionBuilder.Arguments.Length; i++)
			{
				object ce = conditionBuilder.Arguments[i];
				switch (ce)
				{
					case null:
						conditionBuilder.Arguments[i] = DBNull.Value;
						break;
					case string:
					case char:
						if (ce.ToString().ToLower().Trim().IndexOf(@"in(") == 0 ||
							ce.ToString().ToLower().Trim().IndexOf(@"not in(") == 0 ||
							ce.ToString().ToLower().Trim().IndexOf(@" like '") == 0 ||
							ce.ToString().ToLower().Trim().IndexOf(@"not like") == 0)
						{
							conditionBuilder.Arguments[i] = string.Format(" {0} ", ce.ToString());
						}
						else
							goto default;
						//{
						//	//****************************************
						//	conditionBuilder.Arguments[i] = string.Format("'{0}'", ce.ToString());
						//}
						break;
					case DateTime:
						goto default;

					case int:
					case long:
					case short:
					case decimal:
					case double:

					case float:
					case bool:
					case byte:
					case sbyte:
					case ValueType:
						conditionBuilder.Arguments[i] = ce.ToString();
						break;

					default:
						conditionBuilder.Arguments[i] = string.Format("'{0}'", ce.ToString());
						break;
				}

			}
			string strWhere = string.Format(conditionBuilder.Condition, conditionBuilder.Arguments);
			return strWhere;
		}

		


		#endregion
	}
}
