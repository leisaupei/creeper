using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Creeper.Utils;
using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.SqlBuilder.Impi
{
	/// <summary>
	/// delete语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class DeleteBuilder<TModel> : WhereBuilder<IDeleteBuilder<TModel>, TModel>, IDeleteBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		internal DeleteBuilder(ICreeperContext context) : base(context) { }
		internal DeleteBuilder(ICreeperExecute execute) : base(execute) { }

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new int ToAffrows() => base.ToAffrows();

		/// <summary>
		/// 管道模式
		/// </summary>
		/// <returns></returns>
		public ISqlBuilder ToAffrowsPipe() => Pipe<int>(PipeReturnType.Affrows);

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken = default)
			=> base.ToAffrowsAsync(cancellationToken);

		#region Override
		public override string ToString() => base.ToString();

		protected override string GetCommandText()
		{
			if (WhereList.Count == 0)
				throw new ArgumentNullException(nameof(WhereList));
			return DbConverter.GetDeleteSql(MainTable, MainAlias, WhereList);
		}
		#endregion
	}
}
