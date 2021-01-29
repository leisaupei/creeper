using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Creeper.Driver
{
	public interface ICreeperDbTypeConverter
	{
		/// <summary>
		/// 数据库种类
		/// </summary>
		DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 转化数据库返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T ConvertDbData<T>(object value);

		/// <summary>
		/// 转化数据库返回值
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		object ConvertDbData(object value, Type convertType);

		/// <summary>
		/// 数据库返回数据转化为可用的实体模型
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		object ConvertDataReader(IDataReader reader, Type convertType);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objReader"></param>
		/// <returns></returns>
		T ConvertDataReader<T>(IDataReader objReader);

		/// <summary>
		/// 把sql语句转化成string, debug的时候使用看结果
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		string ConvertSqlToString(ISqlBuilder sqlBuilder);
	}
}
