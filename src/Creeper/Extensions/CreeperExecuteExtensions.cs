using Creeper.Driver;
using Creeper.Generic;
using Creeper.SqlBuilder;
using Creeper.SqlBuilder.Impi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public static class CreeperExecuteExtensions
	{
		#region Select
		/// <summary>
		/// 查询数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static ISelectBuilder<TModel> Select<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new SelectBuilder<TModel>(execute);

		/// <summary>
		/// 查询数据, 等同于Select&lt;TModel&gt;().Where(Expression&lt;Func&lt;TModel, bool&gt;&gt;)
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static ISelectBuilder<TModel> Select<TModel>(this ICreeperExecute execute, Expression<Func<TModel, bool>> predicate) where TModel : class, ICreeperModel, new()
			=> execute.Select<TModel>().Where(predicate);
		#endregion

		#region Insert
		/// <summary>
		/// 插入数据builder
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static IInsertBuilder<TModel> Insert<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new InsertBuilder<TModel>(execute);

		/// <summary>
		/// 批量插入数据builder
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static IInsertRangeBuilder<TModel> InsertRange<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new InsertRangeBuilder<TModel>(execute);

		/// <summary>
		/// 仅插入数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int Insert<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Insert<TModel>().Set(model).ToAffrows();

		/// <summary>
		/// 仅插入数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> InsertAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Insert<TModel>().Set(model).ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 插入多条数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="models"></param>
		/// <param name="singleCommand">
		/// 是否使用单命令插入, 默认(true)使用INSERT INTO [Table] VALUES([Values1]),([Values2]) 方式插入数据库<br/>
		/// 否则(false)使用INSERT INTO [Table] VALUES([Values1]);INSERT INTO [Table] VALUES([Values2]);<br/>
		/// 方式区别在于前者皆插入或皆不插入(抛出异常); 后者抛出异常会中断, 但已执行的语句生效, 未执行语句不生效<br/>
		/// Access/Oracle不支持false
		/// </param>
		/// <returns>受影响行数</returns>
		public static int InsertRange<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models, bool singleCommand = true) where TModel : class, ICreeperModel, new()
		{
			if (singleCommand) return execute.InsertRange<TModel>().Set(models).ToAffrows();
			var sqlBuilders = models.Select(model => execute.Insert<TModel>().Set(model).ToAffrowsPipe());
			return execute.ExecutePipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 插入多条数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="models"></param>
		/// <param name="singleCommand">
		/// 是否使用单命令插入, 默认(true)使用INSERT INTO [Table] VALUES([Values1]),([Values2]) 方式插入数据库<br/>
		/// 否则(false)使用INSERT INTO [Table] VALUES([Values1]);INSERT INTO [Table] VALUES([Values2]);<br/>
		/// 方式区别在于前者皆插入或皆不插入(抛出异常); 后者抛出异常会中断, 但已执行的语句生效, 未执行语句不生效<br/>
		/// Access/Oracle不支持false
		/// </param>
		/// <returns>受影响行数</returns>
		public static async ValueTask<int> InsertRangeAsync<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models, bool singleCommand = true, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
		{
			if (singleCommand) return await execute.InsertRange<TModel>().Set(models).ToAffrowsAsync(cancellationToken);
			var sqlBuilders = models.Select(model => execute.Insert<TModel>().Set(model).ToAffrowsPipe());
			var affrows = await execute.ExecutePipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 插入单条数据, 返回插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>插入的数据</returns>
		public static AffrowsResult<TModel> InsertResult<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Insert<TModel>().Set(model).ToAffrowsResult();

		/// <summary>
		/// 插入单条数据, 返回插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>插入的数据</returns>
		public static Task<AffrowsResult<TModel>> InsertResultAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Insert<TModel>().Set(model).ToAffrowsResultAsync(cancellationToken);
		#endregion

		#region Update
		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static IUpdateBuilder<TModel> Update<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new UpdateBuilder<TModel>(execute);

		/// <summary>
		/// 删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="predicate"></param>
		/// <returns>受影响行数</returns>
		public static IUpdateBuilder<TModel> Update<TModel>(this ICreeperExecute execute, Expression<Func<TModel, bool>> predicate) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Where(predicate);

		/// <summary>
		/// 更新数据, 自动使用主键条件
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static IUpdateBuilder<TModel> Update<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Where(model);

		/// <summary>
		/// 更新数据, 自动使用主键条件
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static IUpdateBuilder<TModel> UpdateRange<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Where(models);

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static int UpdateSave<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Set(model).ToAffrows();

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static ValueTask<int> UpdateSaveAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Set(model).ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static int UpdateSaveRange<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models) where TModel : class, ICreeperModel, new()
		{
			var sqlBuilders = models.Select(model => execute.Update<TModel>().Set(model).ToAffrowsPipe());
			return execute.ExecutePipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns></returns>
		public static async ValueTask<int> UpdateSaveRangeAsync<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
		{
			var sqlBuilders = models.Select(model => execute.Update<TModel>().Set(model).ToAffrowsPipe());
			var affrows = await execute.ExecutePipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 以主键为条件更新数据, 返回受影响行和修改后的数据; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行和修改后的数据</returns>
		public static AffrowsResult<TModel> UpdateSaveResult<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Set(model).ToAffrowsResult();

		/// <summary>
		/// 以主键为条件更新数据, 返回受影响行和修改后的数据; 此处程序覆盖数据库方法, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行和修改后的数据</returns>
		public static Task<AffrowsResult<TModel>> UpdateSaveResultAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Update<TModel>().Set(model).ToAffrowsResultAsync(cancellationToken);
		#endregion

		#region Delete
		/// <summary>
		/// 删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns>受影响行数</returns>
		public static IDeleteBuilder<TModel> Delete<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new DeleteBuilder<TModel>(execute);

		/// <summary>
		/// 删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="predicate"></param>
		/// <returns>受影响行数</returns>
		public static int Delete<TModel>(this ICreeperExecute execute, Expression<Func<TModel, bool>> predicate) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(predicate).ToAffrows();

		/// <summary>
		/// 删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="predicate"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> Delete<TModel>(this ICreeperExecute execute, Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(predicate).ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int Delete<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(model).ToAffrows();

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(model).ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int DeleteRange<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(models).ToAffrows();

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteRangeAsync<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Delete<TModel>().Where(models).ToAffrowsAsync(cancellationToken);
		#endregion

		#region Upsert
		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		private static UpsertBuilder<TModel> Upsert<TModel>(this ICreeperExecute execute) where TModel : class, ICreeperModel, new()
			=> new UpsertBuilder<TModel>(execute);

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static int Upsert<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Upsert<TModel>().Set(model).ToAffrows();

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static ValueTask<int> UpsertAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Upsert<TModel>().Set(model).ToAffrowsAsync(cancellationToken);

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="models"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static int UpsertRange<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models) where TModel : class, ICreeperModel, new()
		{
			var sqlBuilders = models.Select(model => execute.Upsert<TModel>().Set(model).ToAffrowsPipe());
			return execute.ExecutePipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="models"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static async ValueTask<int> UpsertRangeAsync<TModel>(this ICreeperExecute execute, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
		{
			var sqlBuilders = models.Select(model => execute.Upsert<TModel>().Set(model).ToAffrowsPipe());
			var affrows = await execute.ExecutePipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static AffrowsResult<TModel> UpsertResult<TModel>(this ICreeperExecute execute, TModel model) where TModel : class, ICreeperModel, new()
			=> execute.Upsert<TModel>().Set(model).ToAffrowsResult();

		/// <summary>
		/// 根据数据库主键更新/插入，仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 1. 此处非主键唯一键(UniqueKey)不会并列为匹配条件，MySQL除外, 使用的是ON DUPLICATE KEY语法, 所以包含唯一键的表，可能执行Insert操作时，需要保证数据库不包含此行。<br/>
		/// 2. 若主键条件行不存在，或主键值赋值为default(不赋值或忽略)时，必定是插入；以下是插入规则：<br/>
		///	--a) 整型自增主键：根据数据库自增标识；注意：不是Upsert传入的主键值<br/>
		///	--b) 随机唯一主键：使用传入参数。若Guid类型值为default，框架会使用Guid.NewGuid()自动生成，其他算法需要自主赋值。<br/>
		/// 3. 若主键条件行存在，则更新该行
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="execute"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>返回行数遵循数据库规则，如: mysql使用此方法更新时返回受影响行数返回2，插入/更新值与数据库一致时返回1，但其他数据库可能没有此规则</returns>
		public static Task<AffrowsResult<TModel>> UpsertResultAsync<TModel>(this ICreeperExecute execute, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperModel, new()
			=> execute.Upsert<TModel>().Set(model).ToAffrowsResultAsync(cancellationToken);
		#endregion
	}
}
