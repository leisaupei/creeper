using System;
using Creeper.Access;
using Creeper.Access.Extensions;
using Creeper.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperAccessExtensions
	{
		/// <summary>
		/// 添加Access数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddAccessContext<TContext>(this CreeperOptions option, Action<CreeperContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			option.AddContext<TContext>(action);
			option.AddConverter<TContext, AccessConverter>();
			option.AddExtension(new CreeperAccessOptionsExtension());
		}
	}
}
