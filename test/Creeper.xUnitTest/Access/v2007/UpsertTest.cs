using Xunit;
using System.Collections;
using System.Linq;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Newtonsoft.Json;
using Creeper.Generic;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.Access.v2007
{
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact(Skip = "Access��֧��Upsert�﷨")]
		public void DoubleUniqueCompositePk()
		{
		}
		[Fact(Skip = "Access��֧��Upsert�﷨")]
		public void UidPk()
		{
		}
		[Fact(Skip = "Access��֧��Upsert�﷨")]
		public void IdentityPk()
		{
		}
		[Fact(Skip = "Access��֧��Upsert�﷨")]
		public void SinglePkAndUniqueField()
		{
		}
		[Fact(Skip = "Access��֧��Upsert�﷨")]
		public void UniqueAndIdentityCompositePk()
		{
		}
	}
}
