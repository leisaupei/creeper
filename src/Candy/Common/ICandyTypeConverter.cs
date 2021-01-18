using Candy.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Candy.Common
{
	public interface ICandyTypeConverter
	{
		T Convert<T>(object value);

		object Convert(object value, Type convertType);

		object ConvertDataReader(IDataReader reader, Type convertType);

		T ConvertDataReader<T>(IDataReader objReader);

		string ConvertSqlToString(ISqlBuilder sqlBuilder);
	}
}
