using Creeper.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Creeper.xUnitTest.Extensions
{
	/// <summary>
	/// 此方法只是免于测试环境必须连接redis临时创建的类, 请勿用于生产环境, 
	/// 示例: <see cref="RedisDbCache"/>
	/// </summary>
	public class HashtableTestDbCache : ICreeperCache
	{
		private readonly Hashtable _redisStorage = new Hashtable();

		public HashtableTestDbCache()
		{
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
		private static Type GetOriginalType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;

		public bool Exists(string key) => _redisStorage.ContainsKey(key);

		public Task<bool> ExistsAsync(string key) => Task.FromResult(Exists(key));

		public object Get(string key, Type type)
		{
			var value = _redisStorage[key]?.ToString();
			if (value == null) return value;
			return Deserialize(value, type);
		}

		public Task<object> GetAsync(string key, Type type) => Task.FromResult(Get(key, type));

		public void Remove(params string[] keys)
		{
			foreach (var key in keys)
			{
				_redisStorage.Remove(key);
			}
		}

		public Task RemoveAsync(params string[] keys) { Remove(keys); return Task.CompletedTask; }

		public bool Set(string key, object value, TimeSpan? expireTime = null)
		{
			_redisStorage[key] = Serialize(value);
			return true;
		}

		public Task<bool> SetAsync(string key, object value, TimeSpan? expireTime = null) => Task.FromResult(Set(key, value, expireTime));

		public void Dispose()
		{
			_redisStorage.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
