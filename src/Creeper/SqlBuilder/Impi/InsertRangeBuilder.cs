using Creeper.Annotations;
using Creeper.Utils;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Creeper.SqlBuilder.Impi
{
	/// <summary>
	/// insert range 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class InsertRangeBuilder<TModel> : SqlBuilder<IInsertRangeBuilder<TModel>, TModel>, IInsertRangeBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private IDictionary<string, string>[] _insertSets;

		internal InsertRangeBuilder(ICreeperContext context) : base(context) { }

		internal InsertRangeBuilder(ICreeperExecute execute) : base(execute) { }

		/// <summary>
		/// 根据实体类插入
		/// </summary>
		/// <param name="models"></param>
		/// <returns></returns>
		public IInsertRangeBuilder<TModel> Set(IEnumerable<TModel> models)
		{
			if (!models?.Any() ?? true)
			{
				throw new ArgumentNullException(nameof(models));
			}

			var columnInfos = new List<(string name, PropertyInfo propertyInfo, CreeperColumnAttribute column)>();
			foreach (var p in typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				string name = DbConverter.WithQuote(DbConverter.CaseInsensitiveTranslator(p.Name));
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();

				if (column == null) { columnInfos.Add((name, p, null)); continue; }

				//如果字段含有忽略返回自增标识
				if ((column.IgnoreFlags & IgnoreWhen.Insert) == 0)
					columnInfos.Add((name, p, column));
			}
			_insertSets = new Dictionary<string, string>[models.Count()];
			for (int i = 0; i < models.Count(); i++)
			{
				_insertSets[i] = new Dictionary<string, string>();
				foreach (var (name, propertyInfo, column) in columnInfos)
				{
					object value = propertyInfo.GetValue(models.ElementAt(i));

					if (column != null)
					{
						//约定自增键必须大于0
						if (column.IsIdentity && (value == null || Convert.ToInt64(value) <= 0))
						{
							if (DbConverter.IdentityKeyDefault != null)
								_insertSets[i][name] = DbConverter.IdentityKeyDefault;
							continue;
						}

						if (column.IsPrimary)
							value = CommonUtils.SetNewGuid(value);
					}
					Set(i, name, value);
				}
			}
			return this;
		}

		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private InsertRangeBuilder<TModel> Set(int i, string key, object value)
		{
			if (value == null)
			{
				_insertSets[i][key] = "null";
				return this;
			}
			var isSpecial = DbConverter.TrySetSpecialDbParameter(out string format, ref value);

			AddParameter(out string index, value);

			var pName = DbConverter.GetSqlDbParameterName(index);

			_insertSets[i][key] = !isSpecial ? pName : string.Format(format, pName);
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
		public new ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken = default)
			=> base.ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public ISqlBuilder PipeToAffrows() => Pipe<int>(PipeReturnType.Affrows);

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public AffrowsResult<List<TModel>> ToListAffrowsResult()
		{
			SetReturnType(PipeReturnType.First);
			return ToListAffrowsResult<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public Task<AffrowsResult<List<TModel>>> ToListAffrowsResultAsync(CancellationToken cancellationToken = default)
		{
			SetReturnType(PipeReturnType.First);
			return ToListAffrowsResultAsync<TModel>(cancellationToken);
		}

		protected override string GetCommandText()
		{
			if (!_insertSets.Any())
				throw new ArgumentNullException(nameof(_insertSets));

			return DbConverter.GetInsertRangeSql<TModel>(MainTable, _insertSets, ReturnType != PipeReturnType.Affrows);

		}
	}
}
