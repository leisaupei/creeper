using Creeper.Generic;
using System;
using System.Data.Common;
using System.Linq;

namespace Creeper.Driver
{
	public class CreeperContextOptions
	{
		/// <summary>
		/// 数据库缓存类
		/// </summary>
		internal Type CacheType { get; set; } = null;

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		internal Action<DbConnection> ConnectionOptions { get; set; } = null;

		/// <summary>
		/// 从库连接字符串集合
		/// </summary>
		internal string[] Secondary { get; set; }

		/// <summary>
		/// 主库连接字符串
		/// </summary>
		internal string Main { get; set; }

		/// <summary>
		/// 主从策略, 针对Select语句, 默认是优先从库
		/// </summary>
		internal DataBaseTypeStrategy Strategy { get; set; } = DataBaseTypeStrategy.MainIfSecondaryEmpty;

		/// <summary>
		/// 添加DbCache, 在执行返回FirstOrDefault与Scalar时使用缓存
		/// </summary>
		/// <typeparam name="TCache"></typeparam>
		public void UseCache<TCache>() where TCache : ICreeperCache
		{
			CacheType = typeof(TCache);
		}

		/// <summary>
		/// 设置数据库主从策略, 默认是<see cref="DataBaseTypeStrategy.MainIfSecondaryEmpty"/>
		/// </summary>
		/// <param name="strategy"></param>
		public void UseStrategy(DataBaseTypeStrategy strategy)
		{
			Strategy = strategy;
		}

		/// <summary>
		/// 设置从数据库
		/// </summary>
		/// <param name="secondary"></param>
		public void UseSecondaryConnectionString(params string[] secondary)
		{
			if (!secondary?.Any() ?? true)
			{
				throw new ArgumentNullException(nameof(secondary));
			}
			Secondary = secondary;
		}

		/// <summary>
		/// 设置主数据库
		/// </summary>
		/// <param name="main"></param>
		public void UseConnectionString(string main)
		{
			if (string.IsNullOrWhiteSpace(main))
			{
				throw new ArgumentException(nameof(main));
			}
			Main = main;
		}

		/// <summary>
		/// 设置数据库链接
		/// </summary>
		/// <param name="action"></param>
		public void UseConnectionOptions(Action<DbConnection> action)
		{
			ConnectionOptions = action;
		}
	}
}
