using Creeper.Utils;
using Creeper.Driver;
using Creeper.SqlBuilder.ExpressionAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Creeper.SqlBuilder.Impi
{
	internal abstract class WhereBuilder<TBuilder, TModel> : SqlBuilder<TBuilder, TModel>, IWhereBuilder<TBuilder, TModel>
		where TBuilder : class, ISqlBuilder
		where TModel : class, ICreeperModel, new()
	{
		#region Field
		/// <summary>
		/// 
		/// </summary>
		private TBuilder This => this as TBuilder;
		private SqlTranslator _translator;
		protected SqlTranslator Translator => _translator ??= new SqlTranslator(DbConverter);
		/// <summary>
		/// 是否or状态
		/// </summary>
		private bool _isOrState = false;

		/// <summary>
		/// or表达式
		/// </summary>
		private readonly List<string> _orExpression = new List<string>();

		/// <summary>
		/// where条件数量
		/// </summary>
		public bool HasWhere => WhereList.Count > 0;

		/// <summary>
		/// where条件列表
		/// </summary>
		protected List<string> WhereList { get; } = new List<string>();
		#endregion

		#region Constructor
		protected WhereBuilder(ICreeperContext context) : base(context) { }

		protected WhereBuilder(ICreeperExecute execute) : base(execute) { }
		#endregion

		#region Where
		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource3">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where<TSource1, TSource2, TSource3>(Expression<Func<TSource1, TSource2, TSource3, bool>> predicate)
			where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new()
			where TSource3 : ICreeperModel, new()
			=> SetWhereExpression(predicate);

		private TBuilder SetWhereExpression(Expression predicate)
		{
			var info = Translator.GetExpression(predicate);
			AddParameters(info.Parameters);
			return Where(info.CmdText);
		}

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where<TSource1, TSource2>(Expression<Func<TModel, TSource1, TSource2, bool>> predicate) where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new()
			=> Where<TModel, TSource1, TSource2>(predicate);

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where<TSource1, TSource2>(Expression<Func<TSource1, TSource2, bool>> predicate) where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new()
			=> SetWhereExpression(predicate);

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where<TSource>(Expression<Func<TModel, TSource, bool>> predicate) where TSource : ICreeperModel, new()
			=> Where<TModel, TSource>(predicate);

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : ICreeperModel, new()
			=> SetWhereExpression(predicate);

		/// <summary>
		/// 主模型重载
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public TBuilder Where(Expression<Func<TModel, bool>> predicate)
			=> Where<TModel>(predicate);

		/// <summary>
		/// 开始Or where表达式
		/// </summary>
		/// <returns></returns>
		public TBuilder WhereOrStart()
		{
			_isOrState = true;
			return This;
		}

		/// <summary>
		/// 结束Or where表达式
		/// </summary>
		/// <returns></returns>
		public TBuilder WhereOrEnd()
		{
			_isOrState = false;
			if (_orExpression.Count > 0)
			{
				Where(string.Join(" OR ", _orExpression));
				_orExpression.Clear();
			}
			return This;
		}

		/// <summary>
		/// 字符串where语句
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		public TBuilder Where(string where)
		{
			if (_isOrState)
				_orExpression.Add($"({where})");
			else
				WhereList.Add($"({where})");
			return This;
		}

		/// <summary>
		/// any
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereAny<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new()
			=> WhereAny(Translator.GetSelectorSpecial(selector), values);

		/// <summary>
		/// any方法
		/// </summary>
		/// <typeparam name="TKey">key类型</typeparam>
		/// <param name="key">字段名片</param>
		/// <param name="values">值</param>
		/// <returns></returns>
		public TBuilder WhereAny<TKey>(string key, IEnumerable<TKey> values) => WhereAny(key, values, false);

		private TBuilder WhereAny<TKey>(string key, IEnumerable<TKey> values, bool isNot)
		{
			if (!values?.Any() ?? true)
				throw new ArgumentNullException(nameof(values));
			if (values.Count() == 1)
			{
				AddParameter(out string index, values.ElementAt(0));
				return Where(string.Concat(key, isNot ? "<>" : "=", DbConverter.GetSqlDbParameterName(index)));
			}
			string[] parametersName;
			if (DbConverter.MergeArray)
			{
				AddParameter(out string index, values.ToArray());
				parametersName = new[] { DbConverter.GetSqlDbParameterName(index) };
			}
			else
			{
				var length = values.Count();
				parametersName = new string[length];
				for (int i = 0; i < length; i++)
				{
					AddParameter(out string index, values.ElementAt(i));
					parametersName[i] = DbConverter.GetSqlDbParameterName(index);
				}
			}
			return Where(DbConverter.ExplainAny(key, isNot, parametersName, false));
		}

		/// <summary>
		/// any 方法, optional字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereAny<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new() where TKey : struct
			=> WhereAny(Translator.GetSelectorSpecial(selector), values);

		/// <summary>
		/// any方法
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereAny<TKey>(Expression<Func<TModel, TKey>> selector, IEnumerable<TKey> values)
			=> WhereAny<TModel, TKey>(selector, values);

		/// <summary>
		/// any 方法, optional字段
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereAny<TKey>(Expression<Func<TModel, TKey?>> selector, IEnumerable<TKey> values) where TKey : struct
			=> WhereAny<TModel, TKey>(selector, values);

		/// <summary>
		/// not equals any 方法
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereNotAny<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new()
			=> WhereNotAny(Translator.GetSelectorSpecial(selector), values);

		/// <summary>
		/// not equals any 方法, optional字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		public TBuilder WhereNotAny<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new() where TKey : struct
			=> WhereNotAny(Translator.GetSelectorSpecial(selector), values);

		/// <summary>
		/// not equals any 方法
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="key"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public TBuilder WhereNotAny<TKey>(string key, IEnumerable<TKey> values) => WhereAny(key, values, true);

		/// <summary>
		/// where not in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TSource">语句中其他的表对象</typeparam>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereNotIn<TSource, TTarget>(Expression<Func<TSource, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TSource : ICreeperModel, new() where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			AddParameters(selectBuilder.Params);
			return Where($"{Translator.GetSelectorSpecial(selector)} NOT IN ({selectBuilder.CommandText})");
		}

		/// <summary>
		/// where in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <typeparam name="TSource">语句中其他的表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereIn<TSource, TTarget>(Expression<Func<TSource, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TSource : ICreeperModel, new() where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			AddParameters(selectBuilder.Params);
			return Where($"{Translator.GetSelectorSpecial(selector)} IN ({selectBuilder.CommandText})");
		}

		/// <summary>
		/// where not in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereNotIn<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
			=> WhereNotIn<TModel, TTarget>(selector, selectBuilderBinder);

		/// <summary>
		/// where in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereIn<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
			=> WhereIn<TModel, TTarget>(selector, selectBuilderBinder);

		/// <summary>
		/// where exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereExists<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			AddParameters(selectBuilder.Params);
			selectBuilder.Fields = "1";
			return Where($"EXISTS ({selectBuilder.CommandText})");
		}

		// public TBuilder WhereExists<TTarget>(Expression<Action<TModel, ISelectBuilder<TTarget>>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		// {
		// 	var result = Translator.GetExpression(selectBuilderBinder);
		// 	AddParameters(result.Parameters);
		// 	return Where($"EXISTS ({result.CmdText})");
		// }

		/// <summary>
		/// where not exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereExists(Action<ISelectBuilder<TModel>> selectBuilderBinder) => WhereExists<TModel>(selectBuilderBinder);

		/// <summary>
		/// where not exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereNotExists<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			AddParameters(selectBuilder.Params);
			selectBuilder.Fields = "1";
			return Where($"NOT EXISTS ({selectBuilder.CommandText})");
		}

		/// <summary>
		/// where not exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		public TBuilder WhereNotExists(Action<ISelectBuilder<TModel>> selectBuilderBinder) => WhereNotExists<TModel>(selectBuilderBinder);

		/// <summary>
		/// where any 如果values 是空或长度为0 直接返回空数据(无论 or and 什么条件)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public TBuilder WhereAnyOrDefault<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values)
			where TSource : ICreeperModel, new()
		{
			if (!values?.Any() ?? true) { IsReturnDefault = true; return This; }
			return WhereAny(selector, values);
		}

		/// <summary>
		/// where any 如果values 是空或长度为0 直接返回空数据(无论 or and 什么条件)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public TBuilder WhereAnyOrDefault<TKey>(Expression<Func<TModel, TKey>> selector, IEnumerable<TKey> values)
			=> WhereAnyOrDefault<TModel, TKey>(selector, values);

		/// <summary>
		/// where format 写法
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public TBuilder Where(string filter, params object[] values)
		{
			if (string.IsNullOrEmpty(filter))
				throw new ArgumentNullException(nameof(filter));
			if (!values?.Any() ?? true)
				return Where(DbConverter.GetNullSql(filter, @"\{\d\}"));

			for (int i = 0; i < values.Length; i++)
			{
				var index = string.Concat("{", i, "}");
				if (filter.IndexOf(index, StringComparison.Ordinal) == -1)
					throw new ArgumentException(nameof(filter));
				if (values[i] == null)
					filter = DbConverter.GetNullSql(filter, index.Replace("{", @"\{").Replace("}", @"\}"));
				else
				{
					AddParameter(out string pIndex, values[i]);
					filter = filter.Replace(index, DbConverter.GetSqlDbParameterName(pIndex));
				}
			}
			return Where(filter);
		}

		/// <summary>
		/// 主键查找
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public TBuilder Where(TModel model)
		{
			var pks = DbConverter.EntityUtils.GetPks<TModel>();
			if (pks.Length == 0)
				throw new CreeperNoPrimaryKeyException<TModel>();

			var filters = new string[pks.Length];
			var objs = new object[pks.Length];

			for (int i = 0; i < pks.Length; i++)
			{
				filters[i] = DbConverter.WithQuote(pks[i]) + $" = {{{i}}}";
				objs[i] = typeof(TModel).GetProperty(pks[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(model);
			}

			return Where(string.Join(" AND ", filters), objs);
		}

		/// <summary>
		/// 主键查找
		/// </summary>
		/// <param name="models"></param>
		/// <returns></returns>
		public TBuilder Where(IEnumerable<TModel> models)
		{
			if (!models?.Any() ?? true)
				throw new ArgumentNullException(nameof(models));

			var pks = DbConverter.EntityUtils.GetPks<TModel>();
			if (pks.Length == 0)
				throw new CreeperNoPrimaryKeyException<TModel>();

			var properties = new PropertyInfo[pks.Length];
			for (int i = 0; i < pks.Length; i++)
				properties[i] = typeof(TModel).GetProperty(pks[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			var filters = new string[properties.Length];
			var objs = new object[properties.Length];

			WhereOrStart();
			foreach (var m in models)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					filters[i] = DbConverter.WithQuote(pks[i]) + $" = {{{i}}}";
					objs[i] = properties[i].GetValue(m);
				}
				Where(string.Join(" AND ", filters), objs);

			}

			return WhereOrEnd();
		}
		#endregion
	}
}
