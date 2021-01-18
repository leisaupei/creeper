using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Candy.Common;
using Candy.DbHelper;

namespace Candy.SqlBuilder
{
	/// <summary>
	/// delete语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class DeleteBuilder<TModel> : WhereBuilder<DeleteBuilder<TModel>, TModel>
		where TModel : class, ICandyDbModel, new()
	{
		internal DeleteBuilder(ICandyDbContext dbContext) : base(dbContext) { }
		internal DeleteBuilder(ICandyDbExecute dbExecute) : base(dbExecute) { }

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

		#region Override
		public override string ToString() => base.ToString();
		public override string GetCommandTextString()
		{
			if (WhereList.Count == 0)
				throw new ArgumentNullException(nameof(WhereList));
			return $"DELETE FROM {MainTable} {MainAlias} WHERE {string.Join("\nAND", WhereList)}";
		}
		#endregion
	}
}
