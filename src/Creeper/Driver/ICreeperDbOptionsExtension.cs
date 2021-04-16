using Microsoft.Extensions.DependencyInjection;

namespace Creeper.Driver
{
	public interface ICreeperDbOptionsExtension
	{
		/// <summary>
		/// 注册子项服务
		/// </summary>
		/// <param name="services"></param>
		void AddServices(IServiceCollection services);
	}
}