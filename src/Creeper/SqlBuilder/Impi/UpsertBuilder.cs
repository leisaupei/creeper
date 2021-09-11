using Creeper.Annotations;
using Creeper.Utils;
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

namespace Creeper.SqlBuilder.Impi
{
	/// <summary>
	/// upsert 语句实例, 需要遵循自增整型主键不能小于0的规范
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class UpsertBuilder<TModel> : WhereBuilder<IUpsertBuilder<TModel>, TModel>, IUpsertBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private Dictionary<string, string> UpsertSets { get; } = new Dictionary<string, string>();

		internal UpsertBuilder(ICreeperContext context) : base(context) { }

		internal UpsertBuilder(ICreeperExecute execute) : base(execute) { }

		/// <summary>
		/// 插入更新, 约定自增键必须大于0
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public IUpsertBuilder<TModel> Set(TModel model)
		{
			bool hasPk = false;
			EntityUtils.PropertiesEnumerator<TModel>(p =>
			{
				string name = DbConverter.WithQuote(DbConverter.CaseInsensitiveTranslator(p.Name));
				object value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();
				if (column != null)
				{
					if (column.IsPrimary) hasPk = true;
					if ((column.IgnoreFlags & (IgnoreWhen.Insert | IgnoreWhen.Update)) != 0)
						return;

					//约定自增键必须大于0
					if (column.IsIdentity && (value == null || Convert.ToInt64(value) <= 0))
					{
						UpsertSets[name] = DbConverter.IdentityKeyDefault;
						return;
					}

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.IsPrimary)
						value = CommonUtils.SetNewGuid(value);
				}
				Set(name, value);
			});
			if (!hasPk)
				throw new CreeperNoPrimaryKeyException<TModel>();

			return this;
		}

		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private UpsertBuilder<TModel> Set(string key, object value)
		{
			if (value == null)
			{
				UpsertSets[key] = "null";
				return this;
			}
			var isSpecial = DbConverter.TrySetSpecialDbParameter(out string format, ref value);

			AddParameter(out string index, value);

			var pName = DbConverter.GetSqlDbParameterName(index);

			UpsertSets[key] = !isSpecial ? pName : string.Format(format, pName);
			return this;
		}

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new int ToAffrows() => base.ToAffrows();

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken = default) => base.ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public ISqlBuilder PipeToAffrows() => Pipe<int>(PipeReturnType.Affrows);

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public AffrowsResult<TModel> ToAffrowsResult()
		{
			SetReturnType(PipeReturnType.First);
			return ToAffrowsResult<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public Task<AffrowsResult<TModel>> ToAffrowsResultAsync(CancellationToken cancellationToken = default)
		{
			SetReturnType(PipeReturnType.First);
			return ToAffrowsResultAsync<TModel>(cancellationToken);
		}

		#region Override
		public override string ToString() => base.ToString();

		protected override string GetCommandText()
		{
			if (!UpsertSets.Any())
				throw new ArgumentNullException(nameof(UpsertSets));

			return DbConverter.GetUpsertSql<TModel>(MainTable, UpsertSets, ReturnType == PipeReturnType.First);
		}
		#endregion
	}
}
