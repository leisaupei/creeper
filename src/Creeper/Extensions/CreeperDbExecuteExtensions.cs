using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Extensions
{
	public static class CreeperDbExecuteExtensions
	{
		#region Select
		/// <summary>
		/// 查询数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns></returns>
		public static SelectBuilder<TModel> Select<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new SelectBuilder<TModel>(dbExecute);
		#endregion

		#region Insert
		/// <summary>
		/// 插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns></returns>
		public static InsertBuilder<TModel> Insert<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbExecute);

		/// <summary>
		/// 仅插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int InsertOnly<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbExecute).Set(model).ToAffectedRows();

		/// <summary>
		/// 仅插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> InsertOnlyAsync<TModel>(this ICreeperDbExecute dbExecute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbExecute).Set(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 插入多条数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int Insert<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var table = EntityHelper.GetDbTable<TModel>();
			var sqlBuilders = models.Select(model => new InsertBuilder<TModel>(dbExecute).Set(model).PipeToAffectedRows());
			return dbExecute.ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 插入多条数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="models"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static async ValueTask<int> InsertAsync<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
		{
			var table = EntityHelper.GetDbTable<TModel>();
			var sqlBuilders = models.Select(model => new InsertBuilder<TModel>(dbExecute).Set(model).PipeToAffectedRows());
			var affrows = await dbExecute.ExecuteDataReaderPipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 插入单条数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <returns>插入的数据</returns>
		public static TModel Insert<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbExecute.Insert<TModel>().Set(model).ToAffectedRows(out TModel result) > 0 ? result : default;

		/// <summary>
		/// 插入单条数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>插入的数据</returns>
		public static Task<TModel> InsertAsync<TModel>(this ICreeperDbExecute dbExecute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbExecute.Insert<TModel>().Set(model).FirstOrDefaultAsync(cancellationToken);
		#endregion

		#region Update
		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbExecute);

		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbExecute).WherePk(model);

		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbExecute).WherePk(models);
		#endregion

		#region Delete
		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <returns>受影响行数</returns>
		public static DeleteBuilder<TModel> Delete<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute);

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int Delete<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute).WherePk(model).ToAffectedRows();

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteAsync<TModel>(this ICreeperDbExecute dbExecute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute).WherePk(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int DeleteMany<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute).WherePk(models).ToAffectedRows();

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteManyAsync<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute).WherePk(models).ToAffectedRowsAsync(cancellationToken);
		#endregion

		#region InsertOrUpdate
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		[Obsolete]
		public static int InsertOrUpdate<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbExecute.Insert<TModel>().Set(model).ToAffectedRows();

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbExecute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		[Obsolete]
		public static ValueTask<int> InsertOrUpdateAsync<TModel>(this ICreeperDbExecute dbExecute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
				=> dbExecute.Insert<TModel>().Set(model).ToAffectedRowsAsync(cancellationToken);
		#endregion
	}
}
