using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.DbHelper;
using Creeper.Generic;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	/// <summary>
	/// lambda表达式转为where条件sql
	/// </summary>
	public class SqlGenerator
    {
        #region Test
        public static string GetWhereByLambda<T>(Expression<Func<T, bool>> predicate, DataBaseKind databaseType)
        {
            return GetWhereByLambdaBase(predicate, databaseType);
        }

        public static string GetWhereByLambdaSelectorWithParameters<T, TKey>(Expression<Func<T, TKey>> predicate, DataBaseKind databaseType)
        {
            return GetWhereByLambdaBase(predicate, databaseType);
        }

        public static string GetWhereByLambdaUnion<TModel, TSource>(Expression<Func<TModel, TSource, bool>> predicate, DataBaseKind databaseType)
        {
            return GetWhereByLambdaBase(predicate, databaseType);
        }

        private static string GetWhereByLambdaBase(Expression predicate, DataBaseKind databaseType)
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

        #region Expression转成Where/Selector/UnionExpression
        /// <summary>
        /// 获取参数返回的sql语句
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fnCreateParameter"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static ExpressionModel GetExpression(Expression expression, Func<string, object, DbParameter> fnCreateParameter, DataBaseKind databaseType)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder(databaseType);
            conditionBuilder.Build(expression);
            var argumentsLength = conditionBuilder.Arguments.Length;

            var ps = new DbParameter[argumentsLength];

            var indexs = new string[argumentsLength];

            for (int i = 0; i < argumentsLength; i++)
            {
                var index = EntityHelper.ParamsIndex;
                ps[i] = fnCreateParameter(index, conditionBuilder.Arguments[i]);
                indexs[i] = string.Concat("@", index);
            }
            string cmdText = string.Format(conditionBuilder.Condition, indexs);
            return new ExpressionModel(cmdText, ps, conditionBuilder.Alias);
        }

        /// <summary>
        /// 获取selector
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="selector"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static string GetSelector(Expression selector, DataBaseKind databaseType)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder(databaseType);
            conditionBuilder.Build(selector);

            return conditionBuilder.Condition;
        }

        /// <summary>
        /// 获取没有别名的selector
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="selector"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static string GetSelectorWithoutAlias(Expression selector, DataBaseKind databaseType)
        {
            var key = GetSelector(selector, databaseType);
            var keyArray = key.Split('.');
            return keyArray.Length > 1 ? keyArray[1] : key;
        }
        #endregion
    }
}
