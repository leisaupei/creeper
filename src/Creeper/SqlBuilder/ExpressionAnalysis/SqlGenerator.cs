using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.DbHelper;
using Creeper.Generic;
using Creeper.Driver;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	/// <summary>
	/// lambda表达式转为where条件sql
	/// </summary>
	public class SqlGenerator
	{
		#region Expression转成Where/Selector/UnionExpression
		/// <summary>
		/// 获取参数返回的sql语句
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="fnCreateParameter"></param>
		/// <returns></returns>
		public static ExpressionModel GetExpression(Expression expression, Func<string, object, DbParameter> fnCreateParameter, ICreeperDbTypeConverter converter)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(converter);
			conditionBuilder.Build(expression);
			var argumentsLength = conditionBuilder.Arguments.Length;

			var ps = new DbParameter[argumentsLength];

			var indexs = new string[argumentsLength];

			for (int i = 0; i < argumentsLength; i++)
			{
				var index = ParameterCounting.Index;
				ps[i] = fnCreateParameter(index, conditionBuilder.Arguments[i]);
				indexs[i] = string.Concat("@", index);
			}
			string cmdText = string.Format(conditionBuilder.Condition, indexs);
			return new ExpressionModel(cmdText, ps, conditionBuilder.Alias);
		}

		/// <summary>
		/// 获取selector
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="converter"></param>
		/// <returns></returns>
		public static string GetSelector(Expression selector, ICreeperDbTypeConverter converter)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(converter);
			conditionBuilder.Build(selector);

			return conditionBuilder.Condition;
		}

		/// <summary>
		/// 获取没有别名的selector
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="converter"></param>
		/// <returns></returns>
		public static string GetSelectorWithoutAlias(Expression selector, ICreeperDbTypeConverter converter)
		{
			var key = GetSelector(selector, converter);
			var keyArray = key.Split('.');
			return keyArray.Length > 1 ? keyArray[1] : key;
		}
		#endregion
	}
}
