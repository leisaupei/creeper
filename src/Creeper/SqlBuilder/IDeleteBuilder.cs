using Creeper.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	public interface IDeleteBuilder<TModel> : IWhereBuilder<IDeleteBuilder<TModel>, TModel>, IGetAffrows where TModel : class, ICreeperModel, new()
	{

	}
}