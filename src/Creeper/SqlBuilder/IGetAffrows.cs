using Creeper.Generic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	public interface IGetAffrows
	{
		/// <summary>
		/// 管道模式，返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <returns></returns>
		ISqlBuilder PipeToAffrows();

		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <returns></returns>
		int ToAffrows();

		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <returns></returns>
		ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken = default);
	}
	public interface IGetAffrowsResult<TModel>
	{
		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <returns></returns>
		AffrowsResult<TModel> ToAffrowsResult();

		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <returns></returns>
		Task<AffrowsResult<TModel>> ToAffrowsResultAsync(CancellationToken cancellationToken = default);
	}
	public interface IGetAffrowsResults<TModel>
	{
		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <remarks>
		///	1. mysql使用更新并返回时, 且只能返回一行, 需要在ConnectionString加上[Allow User Variables=True]参数
		/// </remarks>
		/// <returns></returns>
		AffrowsResult<List<TModel>> ToListAffrowsResult();

		/// <summary>
		/// 返回修改行数，此处受影响行数根据数据库规则返回。<br/>
		/// 注意：<br/>
		/// mysql执行update操作若修改的值与数据库一致会返回0（postgresql/sqlserver等则会返回1）；执行upsert语句时，插入操作返回1，更新操作返回2（其余只返回1）
		/// </summary>
		/// <remarks>
		///	1. mysql使用更新并返回时, 且只能返回一行, 需要在ConnectionString加上[Allow User Variables=True]参数
		/// </remarks>
		/// <returns></returns>
		Task<AffrowsResult<List<TModel>>> ToListAffrowsResultAsync(CancellationToken cancellationToken = default);
	}
}