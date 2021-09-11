using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.Utils;
using Creeper.Generic;
using Creeper.Driver;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	/// <summary>
	/// lambda表达式转为where条件sql
	/// </summary>
	internal class SqlTranslator
	{
		private readonly CreeperConverter _converter;

		public SqlTranslator(CreeperConverter converter) => _converter = converter;

		/// <summary>
		/// 获取参数返回的sql语句
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ExpressionModel GetExpression(Expression expression)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(_converter);
			conditionBuilder.Build(expression);
			var argumentsLength = conditionBuilder.Arguments.Length;

			var ps = new DbParameter[argumentsLength];

			var indexs = new string[argumentsLength];

			for (int i = 0; i < argumentsLength; i++)
			{
				var index = ParameterUtils.Index;
				ps[i] = _converter.GetDbParameter(index, conditionBuilder.Arguments[i]);
				indexs[i] = _converter.GetSqlDbParameterName(index);
			}
			string cmdText = string.Format(conditionBuilder.Condition, indexs);
			return new ExpressionModel(cmdText, ps, conditionBuilder.Alias);
		}

		/// <summary>
		/// 获取selector
		/// </summary>
		/// <param name="selector">表达式</param>
		/// <param name="alias">是否包含别名</param>
		/// <param name="special">是否包含特殊转换</param>
		/// <returns></returns>
		public string GetSelector(Expression selector, bool alias = true, bool special = false)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(_converter);
			conditionBuilder.Build(selector);

			var key = conditionBuilder.Condition;
			if (!alias)
			{
				var keyArray = key.Split('.');
				key = keyArray.Length > 1 ? keyArray[1] : key;
			}

			if (special)
			{
				if (selector is LambdaExpression le && _converter.TryGetSpecialReturnFormat(le.ReturnType, out string format))
					key = string.Format(format, key);
			}
			return key;
		}

		/// <summary>
		/// a=>a.Key ==> a."key"
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public string GetSelectorSpecial(Expression selector) => GetSelector(selector, true, true);

		/// <summary>
		/// a=>a.Key ==> a."key"
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public string GetSelector(Expression selector) => GetSelector(selector, true, false);

		/// <summary>
		/// a=>a.Key ==> key
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public string GetSelectorWithoutAlias(Expression selector) => GetSelector(selector, false, false);
	}
}
