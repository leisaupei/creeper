using Xunit;
using System.Collections;
using System.Linq;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Newtonsoft.Json;
using Creeper.Generic;

namespace Creeper.SqlExpression.XUnitTest
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
		}
		public class TestModel
		{
			public int? Age { get; set; }
			public static int GetSet => 1;
		}
	}
}
