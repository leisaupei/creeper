using Creeper.Driver;
using Creeper.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// Upsert（INSERT or UPDATE）实例。需要遵循自增主键必须大于0的规范。根据主键作为匹配条件<br/>
	/// 若最终结果是执行INSERT操作时，包含自增键的表取自增标识的值，而不是入参的值<br/>
	/// 注意：当前数据库是mysql时，因为使用ON DUPLICATE KEY语法, 此处非主键唯一键(UniqueKey)会成为匹配条件, 会存在唯一列冲突情况, 建议只留一个
	/// </summary>
	/// <typeparam name="TModel">操作对象</typeparam>
	public interface IUpsertBuilder<TModel> : IWhereBuilder<IUpsertBuilder<TModel>, TModel>, IGetAffrows, IGetAffrowsResult<TModel> where TModel : class, ICreeperModel, new()
	{

		/// <summary>
		/// 插入更新, 约定自增键必须大于0
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		IUpsertBuilder<TModel> Set(TModel model);
	}
}