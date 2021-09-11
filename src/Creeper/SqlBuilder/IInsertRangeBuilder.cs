using Creeper.Driver;
using Creeper.Generic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{	
	/// <summary>
	/// INSERT Range语句实例。需要遵循自增整型主键必须大于0的规范
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public interface IInsertRangeBuilder<TModel> : ISqlBuilder<IInsertRangeBuilder<TModel>>, IGetAffrows, IGetAffrowsResults<TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 根据实体类插入
		/// </summary>
		/// <param name="models"></param>
		/// <returns></returns>
		IInsertRangeBuilder<TModel> Set(IEnumerable<TModel> models);
	}
}