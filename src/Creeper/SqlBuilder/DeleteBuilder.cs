using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// delete语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class DeleteBuilder<TModel> : WhereBuilder<DeleteBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{
		internal DeleteBuilder(ICreeperDbContext dbContext) : base(dbContext) { }
		internal DeleteBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new int ToAffectedRows() => base.ToAffectedRows();

        /// <summary>
        /// 管道模式
        /// </summary>
        /// <returns></returns>
        public DeleteBuilder<TModel> PipeToAffectedRows() => base.Pipe<int>(PipeReturnType.Rows);

        /// <summary>
        /// 返回修改行数
        /// </summary>
        /// <returns></returns>
        public new ValueTask<int> ToAffectedRowsAsync(CancellationToken cancellationToken = default)
			=> base.ToAffectedRowsAsync(cancellationToken);

		#region Override
		public override string ToString() => base.ToString();

		public override string GetCommandText()
		{
			if (WhereList.Count == 0)
				throw new ArgumentNullException(nameof(WhereList));
			return $"DELETE FROM {MainTable} {MainAlias} WHERE {string.Join("\nAND", WhereList)}";
		}
		#endregion
	}
}
