using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Driver
{
	internal class CreeperConverterFactory
	{
		private readonly Dictionary<string, CreeperConverter> _converters = new Dictionary<string, CreeperConverter>();

		/// <summary>
		/// 对Context添加数据库转换器
		/// </summary>
		/// <typeparam name="TContext"></typeparam>
		/// <typeparam name="TConverter"></typeparam>
		/// <param name="dbConverter"></param>
		public void Add<TContext, TConverter>(TConverter dbConverter = default)
			where TConverter : CreeperConverter, new()
			where TContext : class, ICreeperContext
		{
			_converters.Add(typeof(TContext).FullName, dbConverter ?? Activator.CreateInstance<TConverter>());
		}

		/// <summary>
		/// 获取Context名称的转换器
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public CreeperConverter Get(string name)
		{
			if (_converters.TryGetValue(name, out var result))
				return result;
			throw new CreeperConverterNotFoundException(name + "没有添加相应的数据库种类的转换器");
		}
	}
}
