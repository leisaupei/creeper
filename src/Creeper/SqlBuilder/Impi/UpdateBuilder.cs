using Creeper.Annotations;
using Creeper.Utils;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder.Impi
{
	/// <summary>
	/// update 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class UpdateBuilder<TModel> : WhereBuilder<IUpdateBuilder<TModel>, TModel>, IUpdateBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		#region Fields
		/// <summary>
		/// 是否有更新赋值
		/// </summary>
		public bool HasSet => SetList.Count > 0;

		private List<string> SetList { get; } = new List<string>();
		#endregion

		#region Constructor
		internal UpdateBuilder(ICreeperContext context) : base(context) { }

		internal UpdateBuilder(ICreeperExecute execute) : base(execute) { }
		#endregion

		public IUpdateBuilder<TModel> Set(string exp)
		{
			SetList.Add(exp);
			return this;
		}
		/// <summary>
		/// 根据实体类主键, 更新数据
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set(TModel model)
		{
			EntityUtils.PropertiesEnumerator<TModel>(p =>
			{
				object value = p.GetValue(model);
				string name = DbConverter.WithQuote(DbConverter.CaseInsensitiveTranslator(p.Name));
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();
				if (column?.IsPrimary ?? false) Where(name + " = {0}", value);
				else Set(name, value);
			});

			if (!HasWhere)
				throw new CreeperNoPrimaryKeyException<TModel>();
			return this;
		}

		/// <summary>
		/// 设置字段等于SQL
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			var exp = string.Concat(Translator.GetSelectorWithoutAlias(selector), " = ", '(', selectBuilder.CommandText, ')');
			AddParameters(selectBuilder.Params);
			return Set(exp);
		}

		/// <summary>
		/// 设置一个字段值(非空类型)
		/// </summary>
		/// <typeparam name="TKey">字段类型</typeparam>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value) => Set(Translator.GetSelectorWithoutAlias(selector), value);

		private IUpdateBuilder<TModel> Set(string field, object value)
		{
			if (value == null)
				return Set(string.Format("{0} = NULL", field));

			var isSpecial = DbConverter.TrySetSpecialDbParameter(out string format, ref value);

			AddParameter(out string index, value);
			var pName = DbConverter.GetSqlDbParameterName(index);
			return Set(string.Format("{0} = {1}", field, !isSpecial ? pName : string.Format(format, pName)));
		}

		/// <summary>
		/// 设置整型等于一个枚举
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">enum value, disallow null</param>
		/// <exception cref="ArgumentNullException">if value is null</exception>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set(Expression<Func<TModel, int>> selector, Enum value)
		{
			if (value is null) throw new ArgumentNullException(nameof(value));

			return Set(selector, Convert.ToInt32(value));
		}

		/// <summary>
		/// 设置整型等于一个枚举
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set(Expression<Func<TModel, int?>> selector, Enum value)
		{
			return Set(selector, Convert.ToInt32(value));
		}

		/// <summary>
		/// 设置一个字段值(可空类型)
		/// </summary>
		/// <typeparam name="TKey">字段类型</typeparam>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, TKey? value) where TKey : struct => Set(Translator.GetSelectorWithoutAlias(selector), value);

		/// <summary>
		/// 数组连接一个数组, 目前只支持postgresql
		/// </summary>
		/// <typeparam name="TKey">数组类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">数组</param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Append<TKey>(Expression<Func<TModel, TKey[]>> selector, params TKey[] value) where TKey : struct
		{
			AddParameter(out string index, value);
			return Set(string.Format("{0} = {0} {2} {1}", Translator.GetSelectorWithoutAlias(selector), DbConverter.GetSqlDbParameterName(index), DbConverter.StringConnectWord));
		}

		/// <summary>
		/// 数组移除某元素, 目前只支持postgresql
		/// </summary>
		/// <typeparam name="TKey">数组的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">元素</param>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Remove<TKey>(Expression<Func<TModel, TKey[]>> selector, TKey value) where TKey : struct
		{
			AddParameter(out string index, value);
			return Set(string.Format("{0} = array_remove({0}, {1})", Translator.GetSelectorWithoutAlias(selector), DbConverter.GetSqlDbParameterName(index)));
		}

		/// <summary>
		/// 自增, 可空类型留默认值, [column] = COALESCE([column], @defaultValue) + @value
		/// </summary>
		/// <typeparam name="TKey">COALESCE默认值类型</typeparam>
		/// <typeparam name="TTarget">增加值的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">增量</param>
		/// <param name="defaultValue">COALESCE默认值, 如果null, 则取default(TKey)</param>
		/// <exception cref="ArgumentNullException">增量为空</exception>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey?>> selector, TTarget value, TKey? defaultValue) where TKey : struct
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			var field = Translator.GetSelectorWithoutAlias(selector);

			AddParameter(out string valueIndex, value);
			AddParameter(out string defaultValueIndex, defaultValue ?? default);
			return Set(string.Format("{0} = {1} + {2}", field, DbConverter.CallCoalesce(field, DbConverter.GetSqlDbParameterName(defaultValueIndex), null), DbConverter.GetSqlDbParameterName(valueIndex)));
		}

		/// <summary>
		/// 自增, 不可空类型不留默认值
		/// </summary>
		/// <typeparam name="TTarget">增加值的类型</typeparam>
		/// <typeparam name="TKey">原类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">增量</param>
		/// <exception cref="ArgumentNullException">增量为空</exception>
		/// <returns></returns>
		public IUpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey>> selector, TTarget value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			var field = Translator.GetSelectorWithoutAlias(selector);
			AddParameter(out string index, value);
			return Set(string.Format("{0} = {0} + {1}", field, DbConverter.GetSqlDbParameterName(index)));
		}

		/// <summary>
		/// 返回修改行数, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <returns></returns>
		public new int ToAffrows() => base.ToAffrows();

		/// <summary>
		/// 返回修改行数, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken = default) => base.ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 插入数据库并返回数据, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <returns></returns>
		public AffrowsResult<TModel> ToAffrowsResult()
		{
			SetReturnType(PipeReturnType.First);
			return ToAffrowsResult<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <returns></returns>
		public Task<AffrowsResult<TModel>> ToAffrowsResultAsync(CancellationToken cancellationToken = default)
		{
			SetReturnType(PipeReturnType.First);
			return ToAffrowsResultAsync<TModel>(cancellationToken);
		}

		/// <summary>
		/// 管道模式
		/// </summary>
		/// <returns></returns>
		public ISqlBuilder ToAffrowsPipe() => Pipe<int>(PipeReturnType.Affrows);

		/// <summary>
		/// 插入数据库并返回数据, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <remarks>
		///	1. mysql使用更新并返回时, 且只能返回一行, 需要在ConnectionString加上[Allow User Variables=True]参数
		/// </remarks>
		/// <returns></returns>
		public Task<AffrowsResult<List<TModel>>> ToListAffrowsResultAsync(CancellationToken cancellationToken = default)
		{
			SetReturnType(PipeReturnType.List);
			return ToListAffrowsResultAsync<TModel>(cancellationToken);
		}

		/// <summary>
		/// 插入数据库并返回数据, 此处受影响行数根据数据库规则返回. 
		/// 如: 该行存在的情况下, mysql若修改的值与数据库一致会返回0, 但postgresql/sqlserver则会返回1
		/// </summary>
		/// <remarks>
		///	1. mysql使用更新并返回时, 且只能返回一行, 需要在ConnectionString加上[Allow User Variables=True]参数
		/// </remarks>
		/// <returns></returns>
		public AffrowsResult<List<TModel>> ToListAffrowsResult()
		{
			SetReturnType(PipeReturnType.List);
			return ToListAffrowsResult<TModel>();
		}

		#region Override
		public override string ToString() => base.ToString();

		/// <summary>
		/// 获取sql语句
		/// </summary>
		/// <exception cref="ArgumentNullException">count of where or set is 0</exception>
		/// <returns></returns>
		protected override string GetCommandText()
		{
			if (WhereList.Count == 0)
				throw new ArgumentNullException(nameof(WhereList));
			if (SetList.Count == 0)
				throw new ArgumentNullException(nameof(SetList));
			return DbConverter.GetUpdateSql<TModel>(MainTable, MainAlias, SetList, WhereList, ReturnType != PipeReturnType.Affrows);
		}
		#endregion
	}
}
