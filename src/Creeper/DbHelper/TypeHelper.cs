using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.DbHelper
{
	internal class TypeHelper
	{
		/// <summary>
		/// 静态数据库类型转换器
		/// </summary>
		public static IReadOnlyDictionary<DataBaseKind, ICreeperDbTypeConverter> DbTypeConverts;

		/// <summary>
		/// 静态数据库类型转换器, dbname为key
		/// </summary>
		public static IReadOnlyDictionary<string, ICreeperDbTypeConverter> DbTypeConvertsName;

		/// <summary>
		/// 实例键值对
		/// </summary>
		public static IReadOnlyDictionary<string, List<ICreeperDbConnectionOption>> ExecuteOptions;

		/// <summary>
		/// 通过数据库类型获取转换器
		/// </summary>
		/// <param name="dataBaseKind"></param>
		/// <returns></returns>
		public static ICreeperDbTypeConverter GetConvert(DataBaseKind dataBaseKind)
			=> DbTypeConverts.TryGetValue(dataBaseKind, out var convert)
			? convert : throw new ArgumentException("没有添加相应的数据库类型转换器");

		/// <summary>
		/// 根据字符串类型的dbname获取转换器
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public static ICreeperDbTypeConverter GetConvert(string dbName)
			=> DbTypeConvertsName.TryGetValue(dbName, out var convert)
			? convert : throw new ArgumentException("没有添加相应的数据库类型转换器");

		/// <summary>
		/// 根据type类型的dbname获取转换器
		/// </summary>
		/// <param name="dbNameType"></param>
		/// <returns></returns>
		public static ICreeperDbTypeConverter GetConvert(Type dbNameType)
			=> GetConvert(dbNameType.Name);

		/// <summary>
		/// 根据dbname泛型获取转换器
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <returns></returns>
		public static ICreeperDbTypeConverter GetConvert<TDbName>()
			=> GetConvert(typeof(TDbName));
	}
}
