using Creeper.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Driver
{
	public class CreeperOptions
	{
		/// <summary>
		/// 数据库转换器工厂
		/// </summary>
		internal CreeperConverterFactory ConverterFactory { get; } = new CreeperConverterFactory();

		/// <summary>
		/// 子项扩展
		/// </summary>
		internal IList<ICreeperOptionsExtension> Extensions { get; } = new List<ICreeperOptionsExtension>();

		/// <summary>
		/// 注册dbcontext
		/// </summary>
		/// <typeparam name="TContext"></typeparam>
		/// <param name="contextConfigure"></param>
		public void AddContext<TContext>(Action<CreeperContextOptions> contextConfigure) where TContext : class, ICreeperContext
			=> AddExtension(new ContextOptionsExtension<TContext>(contextConfigure));

		/// <summary>
		/// 注册扩展服务
		/// </summary>
		/// <param name="extension"></param>
		public void AddExtension(ICreeperOptionsExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			Extensions.Add(extension);
		}

		/// <summary>
		/// 添加db类型转换器
		/// </summary>
		/// <typeparam name="TConverter">转换器对象</typeparam>
		/// <typeparam name="TContext">对应的Context</typeparam>
		/// <param name="dbConverter">转换器对象的实例, 默认: 自动创建</param>
		public void AddConverter<TContext, TConverter>(TConverter dbConverter = default)
			where TConverter : CreeperConverter, new()
			where TContext : class, ICreeperContext
		{
			ConverterFactory.Add<TContext, TConverter>(dbConverter);
		}


		internal class ContextOptionsExtension<TContext> : ICreeperOptionsExtension where TContext : class, ICreeperContext
		{
			private readonly Action<CreeperContextOptions> _contextConfigure;

			public ContextOptionsExtension(Action<CreeperContextOptions> contextConfigure)
			{
				_contextConfigure = contextConfigure ?? throw new ArgumentNullException(nameof(contextConfigure));
			}

			public void AddServices(IServiceCollection services)
			{
				var options = new CreeperContextOptions();
				_contextConfigure.Invoke(options);

				//添加数据库缓存DbCache单例与集合
				if (options.CacheType != null)
				{
					services.TryAddSingleton(options.CacheType);
					services.AddSingleton(typeof(ICreeperCache), options.CacheType);
				}

				//添加Context配置与单例
				var contextType = typeof(TContext);
				services.Configure(contextType.FullName, _contextConfigure);
				services.TryAddSingleton(contextType);
				services.AddSingleton(typeof(ICreeperContext), contextType);
			}
		}
	}
}
