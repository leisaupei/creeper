using Candy.Common;
using Candy.SqlBuilder;

namespace Candy.Extensions
{
	public static class DbContextExtensions
	{
		public static SelectBuilder<TModel> Select<TModel>(this ICandyDbContext dbContext) where TModel : class, ICandyDbModel, new() => new SelectBuilder<TModel>(dbContext);
		public static InsertBuilder<TModel> Insert<TModel>(this ICandyDbContext dbContext) where TModel : class, ICandyDbModel, new() => new InsertBuilder<TModel>(dbContext);
		public static UpdateBuilder<TModel> Update<TModel>(this ICandyDbContext dbContext) where TModel : class, ICandyDbModel, new() => new UpdateBuilder<TModel>(dbContext);
		public static DeleteBuilder<TModel> Delete<TModel>(this ICandyDbContext dbContext) where TModel : class, ICandyDbModel, new() => new DeleteBuilder<TModel>(dbContext);
		public static int Insert<TModel>(this ICandyDbContext dbContext, TModel model) where TModel : class, ICandyDbModel, new() => new InsertBuilder<TModel>(dbContext).Set(model).ToRows();

	}
	public static class DbExecuteExtensions
	{
		public static SelectBuilder<TModel> Select<TModel>(this ICandyDbExecute dbExecute) where TModel : class, ICandyDbModel, new() => new SelectBuilder<TModel>(dbExecute);
		public static InsertBuilder<TModel> Insert<TModel>(this ICandyDbExecute dbExecute) where TModel : class, ICandyDbModel, new() => new InsertBuilder<TModel>(dbExecute);
		public static UpdateBuilder<TModel> Update<TModel>(this ICandyDbExecute dbExecute) where TModel : class, ICandyDbModel, new() => new UpdateBuilder<TModel>(dbExecute);
		public static DeleteBuilder<TModel> Delete<TModel>(this ICandyDbExecute dbExecute) where TModel : class, ICandyDbModel, new() => new DeleteBuilder<TModel>(dbExecute);
		public static int Insert<TModel>(this ICandyDbExecute dbExecute, TModel model) where TModel : class, ICandyDbModel, new() => new InsertBuilder<TModel>(dbExecute).Set(model).ToRows();
		public static UpdateBuilder<TModel> InsertOrUpdate<TModel>(this ICandyDbExecute dbExecute) where TModel : class, ICandyDbModel, new() => new UpdateBuilder<TModel>(dbExecute);

	}
}
