using Creeper.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Creeper.PostgreSql.XUnitTest.Extensions
{
	public class CopyCustomDbCache : ICreeperDbCache, IDisposable
	{
		private readonly Hashtable _redisStorage = new Hashtable();

		public CopyCustomDbCache()
		{
		}
		/// <summary>
		/// 当类型是Nullable&lt;T&gt;,则返回T, 否则返回传入类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type GetOriginalType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;

		private static readonly TimeSpan _expireTime = TimeSpan.FromMinutes(10);

		public bool Exists(string key) => _redisStorage.ContainsKey(key);

		public Task<bool> ExistsAsync(string key) => Task.FromResult(Exists(key));

		public object Get(string key, Type type)
		{
			var value = _redisStorage["x"]?.ToString();
			if (value == null) return value;
			return JsonSerializer.Deserialize(value, type);
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

		public bool Set(string key, object value)
		{
			_redisStorage[key] = value;
			return true;
		}

		public Task<bool> SetAsync(string key, object value) => Task.FromResult(Set(key, value));

		public void Dispose()
		{
			_redisStorage.Clear();
		}
	}
}
