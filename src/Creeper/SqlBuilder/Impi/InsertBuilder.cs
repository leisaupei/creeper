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
	/// insert 语句实例, 需要遵循自增整型主键不能小于0的规范
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class InsertBuilder<TModel> : WhereBuilder<IInsertBuilder<TModel>, TModel>, IInsertBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private Dictionary<string, string> InsertSets { get; } = new Dictionary<string, string>();

		internal InsertBuilder(ICreeperContext context) : base(context) { }

		internal InsertBuilder(ICreeperExecute execute) : base(execute) { }

		/// <summary>
		/// 是否有插入赋值
		/// </summary>
		public bool HasSet => InsertSets.Count > 0;

		/// <summary>
		/// 根据实体类插入
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public IInsertBuilder<TModel> Set(TModel model)
		{
			EntityUtils.PropertiesEnumerator<TModel>(p =>
			{
				string name = DbConverter.WithQuote(DbConverter.CaseInsensitiveTranslator(p.Name));
				object value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();
				if (column != null)
				{
					if ((column.IgnoreFlags & IgnoreWhen.Insert) != 0)
						return;

					//约定自增键必须大于0
					if (column.IsIdentity && (value == null || Convert.ToInt64(value) <= 0))
					{
						if (DbConverter.IdentityKeyDefault != null)
							InsertSets[name] = DbConverter.IdentityKeyDefault;
						return;
					}

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.IsPrimary)
						value = CommonUtils.SetNewGuid(value);
				}

				//value = SetDefaultDateTime(name, value);
				Set(name, value);
			});
			return this;
		}


		/// <summary>
		/// 设置一个结果 调用Field方法定义
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public IInsertBuilder<TModel> Set<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var key = Translator.GetSelectorWithoutAlias(selector);
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			InsertSets[key] = selectBuilder.CommandText;
			return AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 设置语句 不可空参数
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public IInsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value)
			=> Set(Translator.GetSelectorWithoutAlias(selector), value);

		/// <summary>
		/// 设置语句 可空重载
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public IInsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, TKey? value) where TKey : struct
		{
			var key = Translator.GetSelectorWithoutAlias(selector);
			if (value != null) return Set(key, value.Value);

			InsertSets[key] = "null";
			return this;
		}

		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private IInsertBuilder<TModel> Set(string key, object value)
		{
			if (value == null)
			{
				InsertSets[key] = "null";
				return this;
			}
			var isSpecial = DbConverter.TrySetSpecialDbParameter(out string format, ref value);

			AddParameter(out string index, value);

			var pName = DbConverter.GetSqlDbParameterName(index);

			InsertSets[key] = !isSpecial ? pName : string.Format(format, pName);
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

		/// <summary>
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public ISqlBuilder ToAffrowsPipe() => Pipe<int>(PipeReturnType.Affrows);

		#region Override
		public override string ToString() => base.ToString();

		protected override string GetCommandText()
		{
			if (!InsertSets.Any())
				throw new ArgumentNullException(nameof(InsertSets));

			return DbConverter.GetInsertSql<TModel>(MainTable, InsertSets, WhereList, ReturnType != PipeReturnType.Affrows);

		}
		#endregion
	}
}
