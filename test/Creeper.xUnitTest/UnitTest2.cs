using Creeper.Annotations;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Creeper.xUnitTest
{
	public class UnitTest2
	{
		[Fact]
		public void GetYearSection()
		{
			var defaultDatetime = new DateTime(1970, 1, 1, 0, 0, 0);
			var datetime = new DateTime(2020, 1, 1);
			var offsetYears = datetime.Year - defaultDatetime.Year;
			var splitYears = 2;
			var seed = offsetYears / splitYears;
			var begin = defaultDatetime.Year + (seed * splitYears);
			var end = defaultDatetime.Year + ((seed + 1) * splitYears - 1);

		}
		[Fact]
		public void GetMonthSection()
		{
			var defaultDatetime = new DateTime(1970, 1, 1, 0, 0, 0);
			var datetime = new DateTime(1970, 4, 12);
			var splitMonths = 3;
			var offsetMonths = (datetime.Year - defaultDatetime.Year) * 12 + datetime.Month;
			var seed = offsetMonths / splitMonths;
			var begin = defaultDatetime.AddMonths(seed * splitMonths);
			var end = begin.AddMonths(splitMonths - 1);
			var beginStr = begin.ToString("yyyyMM");
			var endStr = end.ToString("yyyyMM");
		}
		[Fact]
		public void GetDateSection()
		{
			var defaultDatetime = new DateTime(1970, 1, 1, 0, 0, 0);
			var datetime = new DateTime(1970, 4, 2);
			var splitDays = 100;
			var offsetDays = (int)(datetime - defaultDatetime).TotalDays;
			var seed = offsetDays / splitDays;
			var begin = defaultDatetime.AddDays(seed * splitDays);
			var end = begin.AddDays(splitDays - 1);
			var beginStr = begin.ToString("yyyyMMdd");
			var endStr = end.ToString("yyyyMMdd");
		}
		[Fact]
		public void SplitTest()
		{
			object[] value = new Enum[][] {
				new Enum[] { SplitType.DateTimeEveryDays, SplitType.DateTimeEveryMonths},
				new Enum[] { SplitType.DateTimeEveryDays }
			};
			var map = value.OfType<int[]>().ToArray();
		}
	}
}
