using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	/// <summary>
	/// 数据库缓存
	/// </summary>
	public interface ICreeperDbCache
	{
		/// <summary>
		/// key过期时间
		/// </summary>
		TimeSpan ExpireTime { get; }

		/// <summary>
		/// 获取key值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string Get(string key);

		/// <summary>
		/// 获取key值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<string> GetAsync(string key);

		/// <summary>
		/// 设置key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		string Set(string key, string value);

		/// <summary>
		/// 设置key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		Task SetAsync(string key, string value);

		/// <summary>
		/// 检查key存在状态
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool Exists(string key);

		/// <summary>
		/// 检查key存在状态
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<bool> ExistsAsync(string key);
	}
}
