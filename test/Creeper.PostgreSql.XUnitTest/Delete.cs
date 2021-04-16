using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;
using System.Data.Common;
using Npgsql;
using Creeper.SqlBuilder;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System.Reflection;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(5)]
	public class Delete //: BaseTest
	{
		[Fact]
		public void Union()
		{

			var a1 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var a2 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			var dt = (DateTime?)null;
			var type = dt.GetType();
			switch (dt)
			{
				case DateTime dt1:
					var b = dt1;
					break;
				default:
					var c = dt;
					break;
			}
		}


	}
}
