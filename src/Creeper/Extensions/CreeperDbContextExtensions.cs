using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.SqlBuilder;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.Extensions
{
	public static class CreeperDbContextExtensions
	{
		public static SelectBuilder<TModel> Select<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new SelectBuilder<TModel>(dbContext);

		public static InsertBuilder<TModel> Insert<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbContext);

		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbContext);

		public static DeleteBuilder<TModel> Delete<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbContext);

		public static int Commit<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbContext).Set(model).ToRows();

		public static int CommitMany<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var table = EntityHelper.GetDbTable<TModel>();
			var sqlBuilders = models.Select(f => new InsertBuilder<TModel>(dbContext).Set(f).ToRowsPipe());
			return dbContext.GetExecute(table.DbName).ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}
		public static TModel Insert<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Set(model).ToRows(out TModel result) > 0 ? result : default;


	}
	public static class CreeperDbExecuteExtensions
	{
		public static SelectBuilder<TModel> Select<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new SelectBuilder<TModel>(dbExecute);

		public static InsertBuilder<TModel> Insert<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbExecute);

		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbExecute);

		public static DeleteBuilder<TModel> Delete<TModel>(this ICreeperDbExecute dbExecute) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbExecute);

		public static int Commit<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbExecute).Set(model).ToRows();

		public static int CommitMany<TModel>(this ICreeperDbExecute dbExecute, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var table = EntityHelper.GetDbTable<TModel>();
			var sqlBuilders = models.Select(f => new InsertBuilder<TModel>(dbExecute).Set(f).ToRowsPipe());
			return dbExecute.ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}
		public static TModel Insert<TModel>(this ICreeperDbExecute dbExecute, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbExecute.Insert<TModel>().Set(model).ToRows(out TModel result) > 0 ? result : default;
	}
}
