using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Candy.XUnitTest
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			//var tuple = (1, 2);
			//var tt = tuple.GetType().BaseType;
			//var tuple1 = tuple.GetType().GetGenericTypeDefinition().GetElementType();
			//var tuple2 = typeof(ValueTuple<>);

			//var x = tt == typeof(ValueTuple);
			//var list = new List<string>();
			//var type = list.GetType().GetNestedTypes();
			//var type2 = typeof(List<>);
			//var a = list.GetType().GetGenericTypeDefinition() == typeof(List<>);
			//var b = list.GetType() == typeof(List<string>);

			//var p = new People("xxx");
			//var date = DateTime.Now;
			//var timestamp = new DateTimeOffset(date).ToUnixTimeMilliseconds();
			//var orgDate = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;

			var jarr = new JArray();
			jarr.Add(new JObject
			{
				["amount"] = 1,
				["guserId"] = Guid.Empty,
				["percent"] = 0.01,
				["remark"] = "Ö÷²¥ÊÕÒæ"
			});
			var str = jarr.ToString(Newtonsoft.Json.Formatting.None);

		}
	}
	public class Human
	{
		public string name;
		public int age;

		public Human(string name)
		{
			this.name = name;
		}



	}
	public class People : Human
	{
		public People(string name) : base(name)
		{
			age = 2;
		}
	}
}
