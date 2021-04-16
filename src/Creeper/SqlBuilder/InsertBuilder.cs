using Creeper.Attributes;
using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// insert 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public sealed class InsertBuilder<TModel> : WhereBuilder<InsertBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private readonly Dictionary<string, string> _insertList = new Dictionary<string, string>();

		/// <summary>
		/// 插入更新
		/// </summary>
		private string upsertString = null;

		internal InsertBuilder(ICreeperDbContext dbContext) : base(dbContext) { }

		internal InsertBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }

		/// <summary>
		/// 插入更新
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		internal InsertBuilder<TModel> Upsert(TModel model)
		{
			var pks = new List<string>();
			var identityKey = new List<string>();

			EntityHelper.GetAllFields<TModel>(p =>
			{
				string name = DbConverter.WithQuotationMarks(p.Name.ToLower());

				object value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column != null)
				{
					//如果自增字段而且没有赋值, 那么忽略此字段
					if (column.Identity)
						identityKey.Add(name);

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.Primary)
					{
						pks.Add(name);
						value = SetNewGuid(value);
					}
				}

				value = SetDefaultDateTime(name, value);
				Set(name, value);
			});
			if (pks.Count == 0)
				throw new NoPrimaryKeyException<TModel>();

			if (identityKey.Count > 0)
			{
				//自增主键
				var exceptIdentityKey = _insertList.Keys.Except(identityKey);

				var pksWhere = string.Join(" AND ", pks.Select(a => $"{a} = {_insertList[a]}"));
				upsertString = @$"WITH upsert AS (
						UPDATE {MainTable} SET {string.Join(", ", exceptIdentityKey.Except(pks).Select(a => $"{a} = {_insertList[a]}"))} 
						WHERE {pksWhere} RETURNING {string.Join(", ", pks)}
					) 
					INSERT INTO {MainTable} ({string.Join(", ", exceptIdentityKey)})
					SELECT {string.Join(", ", exceptIdentityKey.Select(a => _insertList[a]))}
					WHERE NOT EXISTS(SELECT 1 FROM upsert WHERE {pksWhere})";
			}
			else
			{
				upsertString = @$"
	INSERT INTO {MainTable} ({string.Join(", ", _insertList.Keys)}) VALUES({string.Join(", ", _insertList.Values)})
	ON CONFLICT({string.Join(", ", pks)}) DO UPDATE
	SET {string.Join(", ", _insertList.Keys.Except(pks).Select(a => $"{a} = EXCLUDED.{a}"))}";

			}
			return this;
		}

		/// <summary>
		/// 根据实体类插入
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public InsertBuilder<TModel> Set(TModel model)
		{
			EntityHelper.GetAllFields<TModel>(p =>
			{
				string name = DbConverter.WithQuotationMarks(p.Name.ToLower());
				object value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column != null)
				{
					//如果自增字段而且没有赋值, 那么忽略此字段
					if (IngoreIdentity(column, value, p.PropertyType))
						return;

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.Primary)
						value = SetNewGuid(value);
				}

				value = SetDefaultDateTime(name, value);
				Set(name, value);
			});
			return this;
		}
		private static bool IngoreIdentity(CreeperDbColumnAttribute column, object value, Type propertyType)
		{
			if (column.Identity)
			{
				var def = Activator.CreateInstance(propertyType);

				if (def is null || value.ToString() == def.ToString())
					return true;
			}
			return false;
		}
		private static object SetNewGuid(object value)
		{
			if (value is Guid g && g == default)
				value = Guid.NewGuid();
			return value;
		}

		private object SetDefaultDateTime(string name, object value)
		{
			if (name == DbConverter.WithQuotationMarks("create_time"))
			{
				//不可空datetime类型赋值本地当前时间
				if (value is DateTime d && d == default)
					value = DateTime.Now;
				//不可空long类型时间戳赋值本地当前时间毫秒时间戳
				else if (value is long l && l == default)
					value = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}

			return value;
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
		private InsertBuilder<TModel> Set<TKey>(string key, TKey value)
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
		private InsertBuilder<TModel> Set(string key, object value)
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
		public new int ToAffectedRows() => base.ToAffectedRows();

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToAffectedRowsAsync(CancellationToken cancellationToken = default)
			=> base.ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public InsertBuilder<TModel> PipeToAffectedRows() => base.Pipe<int>(PipeReturnType.Rows);

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		public int ToAffectedRows<TResult>(out TResult info)
		{
			ReturnType = PipeReturnType.One;
			info = FirstOrDefault<TResult>();
			return info != null ? 1 : 0;
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public TModel FirstOrDefault()
		{
			ReturnType = PipeReturnType.One;
			return FirstOrDefault<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
		{
			ReturnType = PipeReturnType.One;
			return base.FirstOrDefaultAsync<TModel>(cancellationToken);
		}

		#region Override
		public override string ToString() => base.ToString();

		public override string GetCommandText()
		{
			if (!_insertList.Any())
				throw new ArgumentNullException(nameof(_insertList));

			string returning = null;
			if (ReturnType == PipeReturnType.One)
				returning = $"RETURNING {EntityHelper.GetFieldsAlias<TModel>(null, DbConverter)}";
			if (upsertString != null)
				return $"{upsertString} {returning}";
			if (WhereList.Count == 0)
				return $"INSERT INTO {MainTable} ({string.Join(", ", _insertList.Keys)}) VALUES({string.Join(", ", _insertList.Values)}) {returning}";
			return $"INSERT INTO {MainTable} ({string.Join(", ", _insertList.Keys)}) SELECT {string.Join(", ", _insertList.Values)} WHERE {string.Join(" AND ", WhereList)} {returning}";

		}
		#endregion
	}
}
