using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;
using System.Data.Common;
using Npgsql;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(5)]
	public class Delete //: BaseTest
	{
		[Fact]
		public void Union()
		{
			//Guid id = default;
			//var tuple = (1, 2);
			//var tt = tuple.GetType().BaseType;
			//var tuple1 = tuple.GetType().GetGenericTypeDefinition().GetElementType();
			//var tuple2 = typeof(ValueTuple<>);

			//var x = tt == typeof(ValueTuple);
			//var list = new List<string>();
			//var arr = new string[0];
			//var type = list.GetType().GetNestedTypes();
			//var type2 = typeof(List<>);
			//var type3 = arr.GetType() == typeof(Array);
			//var a1 = list.GetType().GetGenericTypeDefinition();
			//var a = list.GetType().GetGenericTypeDefinition() == typeof(List<>);
			//var b = list.GetType() == typeof(List<string>);

			//var xx = typeof(IList).IsAssignableFrom(list.GetType());
			//var xx1 = typeof(IList).IsAssignableFrom(arr.GetType());

			//var xx2 = typeof(IList).IsInstanceOfType(list);
			//var xx3 = typeof(IList).IsInstanceOfType(arr);

			//var testInt = System.Text.Json.JsonSerializer.Serialize(6);

			//var testDeInt = (long)System.Text.Json.JsonSerializer.Deserialize("6", typeof(long));


			//var testGuid = System.Text.Json.JsonSerializer.Serialize(Guid.Empty);
			//var testString = System.Text.Json.JsonSerializer.Deserialize("\"7567\"", typeof(string));

			//var testDeGuid = (Guid)System.Text.Json.JsonSerializer.Deserialize(testGuid, typeof(Guid));

			//var objValue = Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1");
			//var returnType = typeof(Guid);
			//Guid result = Guid.Empty;
			//var converter = TypeDescriptor.GetConverter(returnType);
			//if (converter.CanConvertFrom(objValue.GetType()))
			//{
			//	result = (Guid)converter.ConvertFrom(objValue);
			//}
			//else
			//{
			//	result = (Guid)Convert.ChangeType(objValue, returnType);
			//}
			//var hash = new Hashtable();
			//var value = hash["a"];
			//hash.Remove("x");

		}


	}
}
