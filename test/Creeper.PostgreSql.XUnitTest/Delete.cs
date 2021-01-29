using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(5)]
	public class Delete : BaseTest
	{
		[Fact]
		public void Union()
		{
			var tuple = (1, 2);
			var tt = tuple.GetType().BaseType;
			var tuple1 = tuple.GetType().GetGenericTypeDefinition().GetElementType();
			var tuple2 = typeof(ValueTuple<>);

			var x = tt == typeof(ValueTuple);
			var list = new List<string>();
			var arr = new string[0];
			var type = list.GetType().GetNestedTypes();
			var type2 = typeof(List<>);
			var type3 = arr.GetType() == typeof(Array);
			var a1 = list.GetType().GetGenericTypeDefinition();
			var a = list.GetType().GetGenericTypeDefinition() == typeof(List<>);
			var b = list.GetType() == typeof(List<string>);

			var xx = typeof(IList).IsAssignableFrom(list.GetType());
			var xx1 = typeof(IList).IsAssignableFrom(arr.GetType());

			var xx2 = typeof(IList).IsInstanceOfType(list);
			var xx3 = typeof(IList).IsInstanceOfType(arr);


		}
	

	}
}
