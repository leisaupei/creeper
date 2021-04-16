using Creeper.Extensions;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.PostgreSql.XUnitTest
{
	public class Performance : BaseTest
	{
		[Fact]
		public void InertTenThousandData()
		{
			for (int i = 0; i < 10000; i++)
			{
				_dbContext.GetExecute<DbMain>().Transaction(_transDbContext =>
				{
					var total = _transDbContext.Select<TypeTestModel>().Sum(a => a.Int8_type, 0);
				});
			}
		}
		[Fact]
		public async Task TransactionAsync()
		{
			await _dbContext.GetExecute<DbMain>().TransactionAsync(_transDbContext =>
			{
				var total = _transDbContext.Select<TypeTestModel>().Sum(a => a.Int8_type, 0);
				var affrows = _transDbContext.Update<TypeTestModel>().Set(a => a.Int8_type, 0).Where(a => a.Id == Guid.Empty).ToAffectedRows();
			}, CancellationToken.None);
		}
		[Fact]
		public void TestAsync()
		{
			var affrows = _dbContext.GetExecute<DbMain>().ExecuteScalar("update people set age = 2 where id = '5ef5a598-e4a1-47b3-919e-4cc1fdd97757';");
			_dbContext.GetExecute<DbMain>().ExecuteScalar("update people set age = 2 where id = '5ef5a598-e4a1-47b3-919e-4cc1fdd97757';");
			var transDbContext = _dbContext.BeginTransaction();
			transDbContext.CommitTransaction();
		}
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
