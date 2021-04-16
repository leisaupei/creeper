using Creeper.Driver;
using Creeper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.Generic
{
	public class CreeperOptions
	{
		/// <summary>
		/// 数据库连接集合
		/// </summary>
		internal IList<ICreeperDbOption> DbOptions { get; } = new List<ICreeperDbOption>();

		/// <summary>
		/// 数据库转换器
		/// </summary>
		internal IList<ICreeperDbTypeConverter> CreeperDbTypeConverters { get; } = new List<ICreeperDbTypeConverter>();

		/// <summary>
		/// 子项扩展
		/// </summary>
		internal IList<ICreeperDbOptionsExtension> Extensions { get; } = new List<ICreeperDbOptionsExtension>();

		/// <summary>
		/// 数据库缓存类
		/// </summary>
		internal Type DbCacheType { get; private set; } = null;

		/// <summary>
		/// 主从策略
		/// </summary>
		public DataBaseTypeStrategy DbTypeStrategy { get; set; } = DataBaseTypeStrategy.SecondaryFirstOfMainIfEmpty;

		/// <summary>
		/// 默认使用的数据库配置struct
		/// </summary>
		public Type DefaultDbOptionName { get; set; }

		/// <summary>
		/// 添加数据库配置
		/// </summary>
		/// <param name="dbOption"></param>
		public void AddOption(ICreeperDbOption dbOption)
			=> DbOptions.Add(dbOption);

		/// <summary>
		/// 添加db类型转换器
		/// </summary>
		/// <typeparam name="TDbTypeConvert"></typeparam>
		public void TryAddDbTypeConvert<TDbTypeConvert>() where TDbTypeConvert : ICreeperDbTypeConverter, new()
		{
			var convert = Activator.CreateInstance<TDbTypeConvert>();
			if (!CreeperDbTypeConverters.Any(a => a.DataBaseKind != convert.DataBaseKind))
				CreeperDbTypeConverters.Add(convert);
		}

		/// <summary>
		/// 添加DbCache
		/// </summary>
		/// <typeparam name="TDbCache"></typeparam>
		public void UseCache<TDbCache>() where TDbCache : ICreeperDbCache
		{
			DbCacheType = typeof(TDbCache);
		}

		/// <summary>
		/// 注册扩展服务
		/// </summary>
		/// <param name="extension"></param>
		public void RegisterExtension(ICreeperDbOptionsExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			Extensions.Add(extension);
		}
	}

}
