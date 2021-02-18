using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// insert 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class InsertBuilder<TModel> : WhereBuilder<InsertBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private readonly Dictionary<string, string> _insertList = new Dictionary<string, string>();

		internal InsertBuilder(ICreeperDbContext dbContext) : base(dbContext) { }

		internal InsertBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public InsertBuilder<TModel> Set(TModel model)
		{
			EntityHelper.GetAllFields<TModel>(p => Set(string.Concat("\"", p.Name.ToLower(), "\""), p.GetValue(model)));
			return this;
		}

		/// <summary>
		/// 设置一个结果 调用Field方法定义
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="sqlBuilder">sql</param>
		/// <returns></returns>
		public InsertBuilder<TModel> Set(Expression<Func<TModel, object>> selector, ISqlBuilder sqlBuilder)
		{
			var key = GetSelectorWithoutAlias(selector);
			_insertList[key] = sqlBuilder.CommandText;
			return AddParameters(sqlBuilder.Params);
		}

		/// <summary>
		/// 设置语句 不可空参数
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public InsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value)
			=> Set(GetSelectorWithoutAlias(selector), value);

		/// <summary>
		/// 设置语句 可空重载
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public InsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, TKey? value) where TKey : struct
		{
			var key = GetSelectorWithoutAlias(selector);
			if (value != null) return Set(key, value.Value);

			_insertList[key] = "null";
			return this;
		}

		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected InsertBuilder<TModel> Set<TKey>(string key, TKey value)
		{
			if (value == null)
			{
				_insertList[key] = "null";
				return this;
			}
			AddParameter(out string index, value);
			_insertList[key] = string.Concat("@", index);
			return this;
		}
		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected InsertBuilder<TModel> Set(string key, object value)
		{
			if (value == null)
			{
				_insertList[key] = "null";
				return this;
			}
			AddParameter(out string index, value);
			_insertList[key] = string.Concat("@", index);
			return this;
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
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public InsertBuilder<TModel> ToRowsPipe() => base.ToPipe<int>(PipeReturnType.Rows);

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public int ToRows<T>(out T info)
		{
			ReturnType = PipeReturnType.Rows;
			info = ToOne<T>();
			return info != null ? 1 : 0;
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public TModel ToOne()
		{
			ReturnType = PipeReturnType.One;
			return ToOne<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public Task<TModel> ToOneAsync(CancellationToken cancellationToken = default)
		{
			ReturnType = PipeReturnType.One;
			return base.ToOneAsync<TModel>(cancellationToken);
		}
		#region Override
		public override string ToString() => base.ToString();

		public override string GetCommandText()
		{
			if (!_insertList.Any())
				throw new ArgumentNullException(nameof(_insertList));
			var field = string.Join(", ", _insertList.Keys);
			var ret = ReturnType == PipeReturnType.One ? $"RETURNING {field}" : "";
			if (WhereList.Count == 0)
				return $"INSERT INTO {MainTable} ({field}) VALUES({string.Join(", ", _insertList.Values)}) {ret}";
			return $"INSERT INTO {MainTable} ({field}) SELECT {string.Join(", ", _insertList.Values)} WHERE {string.Join("\nAND", WhereList)} {ret}";
		}
		#endregion
	}
}
