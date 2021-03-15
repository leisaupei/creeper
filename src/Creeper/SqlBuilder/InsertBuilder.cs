using Creeper.Attributes;
using Creeper.DbHelper;
using Creeper.Driver;
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
			EntityHelper.GetAllFields<TModel>(p =>
			{
				var name = string.Concat('"', p.Name.ToLower(), '"');
				var value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column != null)
				{
					var def = Activator.CreateInstance(p.PropertyType);
					//如果自增字段而且没有赋值, 那么忽略此字段
					if (column.Identity && value.ToString() == def.ToString()) return;

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.Primary && value is Guid g && g == default) value = Guid.NewGuid();
				}

				if (name == "\"create_time\"")
				{
					//不可空datetime类型赋值本地当前时间
					if (value is DateTime d && d == default)
						value = DateTime.Now;
					//不可空long类型时间戳赋值本地当前时间毫秒时间戳
					else if (value is long l && l == default)
						value = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				}
				Set(name, value);
			});
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
			ReturnType = PipeReturnType.Rows;
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
			var field = string.Join(", ", _insertList.Keys);
			var ret = ReturnType == PipeReturnType.One ? $"RETURNING {field}" : "";
			if (WhereList.Count == 0)
				return $"INSERT INTO {MainTable} ({field}) VALUES({string.Join(", ", _insertList.Values)}) {ret}";
			return $"INSERT INTO {MainTable} ({field}) SELECT {string.Join(", ", _insertList.Values)} WHERE {string.Join("\nAND", WhereList)} {ret}";
		}
		#endregion
	}
}
