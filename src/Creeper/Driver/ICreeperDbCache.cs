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
		/// 获取key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type">返回值的类型</param>
		/// <returns></returns>
		object Get(string key, Type type);

		/// <summary>
		/// 获取key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type">返回值的类型</param>
		/// <returns></returns>
		Task<object> GetAsync(string key, Type type);

		/// <summary>
		/// 设置key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>是否设置成功</returns>
		bool Set(string key, object value);

		/// <summary>
		/// 设置key值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>是否设置成功</returns>
		Task<bool> SetAsync(string key, object value);

		/// <summary>
		/// 检查key存在状态
		/// </summary>
		/// <param name="key"></param>
		/// <returns>是否存在</returns>
		bool Exists(string key);

		/// <summary>
		/// 检查key存在状态
		/// </summary>
		/// <param name="key"></param>
		/// <returns>是否存在</returns>
		Task<bool> ExistsAsync(string key);

		/// <summary>
		/// 移除key
		/// </summary>
		/// <param name="keys"></param>
		void Remove(params string[]  keys);

		/// <summary>
		/// 移除key
		/// </summary>
		/// <param name="keys"></param>
		Task RemoveAsync(params string[] keys);
	}
}
