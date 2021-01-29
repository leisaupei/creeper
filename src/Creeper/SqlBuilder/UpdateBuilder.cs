using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// update 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class UpdateBuilder<TModel> : WhereBuilder<UpdateBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{
		#region Fields
		/// <summary>
		/// 设置列表
		/// </summary>
		private readonly List<string> _setList = new List<string>();

		/// <summary>
		/// 是否返回实体类
		/// </summary>
		private bool _isReturn = false;

		/// <summary>
		/// set 数量
		/// </summary>
		public int SetCount => _setList.Count;
		#endregion

		#region Constructor
		internal UpdateBuilder(ICreeperDbContext dbContext) : base(dbContext) { }

		internal UpdateBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }
		#endregion

		private UpdateBuilder<TModel> AddSetExpression(string exp)
		{
			_setList.Add(exp);
			return this;
		}

		/// <summary>
		/// 设置字段等于SQL
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="sqlBuilder">SQL语句</param>
		/// <returns></returns>
		public UpdateBuilder<TModel> Set(Expression<Func<TModel, object>> selector, [DisallowNull] ISqlBuilder sqlBuilder)
		{
			var exp = string.Concat(GetSelectorWithoutAlias(selector), " = ", $"({sqlBuilder.CommandText})");
			AddParameters(sqlBuilder.Params);
			return AddSetExpression(exp);
		}

		/// <summary>
		/// 设置一个字段值(非空类型)
		/// </summary>
		/// <typeparam name="TKey">字段类型</typeparam>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <param name="isSet">是否设置</param>
		/// <returns></returns>
		public UpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value, bool isSet = true)
		{
			if (!isSet) return this;
			var field = GetSelectorWithoutAlias(selector);
			if (value == null)
				return AddSetExpression(string.Format("{0} = null", field));

			AddParameter(out string valueIndex, value);
			return AddSetExpression(string.Format("{0} = @{1}", field, valueIndex));
		}

		/// <summary>
		/// 设置整型等于一个枚举
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		private UpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, [DisallowNull] Enum value) where TKey : IFormattable
			=> Set(selector, (TKey)Convert.ChangeType(value, typeof(TKey)));

		/// <summary>
		/// 设置整型等于一个枚举
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		private UpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, Enum value) where TKey : struct, IFormattable
		{
			var field = GetSelectorWithoutAlias(selector);
			if (value == null)
				return AddSetExpression(string.Format("{0} = null", field));

			AddParameter(out string valueIndex, Convert.ChangeType(value, typeof(TKey)));
			return AddSetExpression(string.Format("{0} = @{1}", field, valueIndex));
		}

		/// <summary>
		/// 设置一个字段值(可空类型)
		/// </summary>
		/// <typeparam name="TKey">字段类型</typeparam>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <param name="isSet"></param>
		/// <returns></returns>
		public UpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, TKey? value, bool isSet = true) where TKey : struct
		{
			if (!isSet) return this;
			var field = GetSelectorWithoutAlias(selector);
			if (value == null)
				return AddSetExpression(string.Format("{0} = null", field));

			AddParameter(out string valueIndex, value.Value);
			return AddSetExpression(string.Format("{0} = @{1}", field, valueIndex));
		}

		/// <summary>
		/// 数组连接一个数组
		/// </summary>
		/// <typeparam name="TKey">数组类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">数组</param>
		/// <returns></returns>
		public UpdateBuilder<TModel> Append<TKey>(Expression<Func<TModel, IEnumerable<TKey>>> selector, params TKey[] value)
		{
			AddParameter(out string valueIndex, value);
			return AddSetExpression(string.Format("{0} = {0} || @{1}", GetSelectorWithoutAlias(selector), valueIndex));
		}

		/// <summary>
		/// 数组移除某元素
		/// </summary>
		/// <typeparam name="TKey">数组的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">元素</param>
		/// <returns></returns>
		public UpdateBuilder<TModel> Remove<TKey>(Expression<Func<TModel, IEnumerable<TKey>>> selector, TKey value)
		{
			AddParameter(out string valueIndex, value);
			return AddSetExpression(string.Format("{0} = array_remove({0}, @{1})", GetSelectorWithoutAlias(selector), valueIndex));
		}

		/// <summary>
		/// 自增, 可空类型留默认值
		/// </summary>
		/// <typeparam name="TKey">COALESCE默认值类型</typeparam>
		/// <typeparam name="TTarget">增加值的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">增量</param>
		/// <param name="defaultValue">COALESCE默认值, 如果null, 则取default(TKey)</param>
		/// <exception cref="ArgumentNullException">增量为空</exception>
		/// <returns></returns>
		public UpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey?>> selector, TTarget value, TKey? defaultValue) where TKey : struct
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			var field = GetSelectorWithoutAlias(selector);
			AddParameter(out string valueIndex, value);
			AddParameter(out string defaultValueIndex, defaultValue ?? default);
			return AddSetExpression(string.Format("{0} = COALESCE({0}, @{1}) + @{2}", field, defaultValueIndex, valueIndex));
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
		public UpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey>> selector, TTarget value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			var field = GetSelectorWithoutAlias(selector);
			AddParameter(out string valueIndex, value);
			return AddSetExpression(string.Format("{0} = {0} + @{1}", field, valueIndex));
		}

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new int ToRows() => base.ToRows();

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToRowsAsync(CancellationToken cancellationToken = default)
			=> base.ToRowsAsync(cancellationToken);

		/// <summary>
		/// 返回修改行数, 并且ref实体类(一行)
		/// </summary>
		/// <returns></returns>
		public int ToRows(out TModel refInfo)
		{
			_isReturn = true;
			refInfo = base.ToOne<TModel>();
			if (refInfo == null) return 0;
			return 1;
		}

		/// <summary>
		/// 返回修改行数, 并且ref列表(多行)
		/// </summary>
		/// <param name="refInfo"></param>
		/// <returns></returns>
		public int ToRows(out List<TModel> refInfo)
		{
			_isReturn = true;
			refInfo = base.ToList<TModel>();
			return refInfo.Count;
		}

        /// <summary>
        /// 管道模式
        /// </summary>
        /// <returns></returns>
        public UpdateBuilder<TModel> ToRowsPipe() => base.ToPipe<int>(PipeReturnType.Rows);

        /// <summary>
        /// 插入数据库并返回数据
        /// </summary>
        /// <returns></returns>
        public TModel ToOne()
		{
			_isReturn = true;
			return base.ToOne<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public new Task<T> ToOneAsync<T>(CancellationToken cancellationToken = default)
		{
			_isReturn = true;
			return base.ToOneAsync<T>(cancellationToken);
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public new Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default)
		{
			_isReturn = true;
			return base.ToListAsync<T>(cancellationToken);
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public new List<T> ToList<T>()
		{
			_isReturn = true;
			return base.ToList<T>();
		}

		#region Override
		public override string ToString() => base.ToString();

		/// <summary>
		/// 获取sql语句
		/// </summary>
		/// <exception cref="ArgumentNullException">count of where or set is 0</exception>
		/// <returns></returns>
		public override string GetCommandTextString()
		{
			if (WhereList.Count == 0)
				throw new ArgumentNullException(nameof(WhereList));
			if (_setList.Count == 0)
				throw new ArgumentNullException(nameof(_setList));
			var ret = string.Empty;
			if (_isReturn)
			{
				Fields = EntityHelper.GetModelTypeFieldsString<TModel>(MainAlias);
				ret = $"RETURNING {Fields}";
			}
			return $"UPDATE {MainTable} {MainAlias} SET {string.Join(",", _setList)} WHERE {string.Join("\nAND", WhereList)} {ret}";
		}
		#endregion
	}
}
