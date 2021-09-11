using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Creeper.xUnitTest.Extensions
{
	public class RedisDbCache : ICreeperCache
	{
		private readonly CSRedis.CSRedisClient _redisClient = null;
		public RedisDbCache()
		{
			_redisClient = new CSRedis.CSRedisClient("192.168.1.15:6379,defaultDatabase=13,password=123456,prefix=test,abortConnect=false");
		}
		private static object Deserialize(string value, Type type)
		{
			if (value == null) return null;
			if (type == typeof(string)) return value;
			return JsonSerializer.Deserialize(value, GetOriginalType(type));
		}
		private static string Serialize(object value)
		{
			if (value == null) return null;
			if (value is string str) return str;
			return JsonSerializer.Serialize(value);
		}
		/// <summary>
		/// 当类型是Nullable&lt;T&gt;,则返回T, 否则返回传入类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type GetOriginalType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;

		private static readonly TimeSpan _globalExpireTime = TimeSpan.FromMinutes(10);

		public bool Exists(string key) => _redisClient.Exists(key);

		public Task<bool> ExistsAsync(string key) => _redisClient.ExistsAsync(key);

		public object Get(string key, Type type) => Deserialize(_redisClient.Get(key), type);

		public async Task<object> GetAsync(string key, Type type) => Deserialize(await _redisClient.GetAsync(key), type);

		public void Remove(params string[] keys) => _redisClient.Del(keys);

		public Task RemoveAsync(params string[] keys) => _redisClient.DelAsync(keys);

		public bool Set(string key, object value, TimeSpan? expireTime) => _redisClient.Set(key, Serialize(value), expireTime ?? _globalExpireTime);

		public Task<bool> SetAsync(string key, object value, TimeSpan? expireTime) => _redisClient.SetAsync(key, Serialize(value), expireTime ?? _globalExpireTime);

		public void Dispose()
		{
			_redisClient.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
