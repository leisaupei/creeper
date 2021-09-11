using Creeper.Annotations;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.PostgreSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Creeper.xUnitTest
{
	public class ExpressionTest : BaseTest
	{
		private static readonly Guid _staticId = Guid.Parse("da58b577-414f-4875-a890-f11881ce6341");
		private const string ConstName = "xxx";
		[Fact]
		public void StaticField()
		{
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.Id == _staticId);

			Assert.Single(builder.Params);
			Assert.Equal(_staticId, builder.Params[0].Value);
		}

		[Fact]
		public void InternalMember()
		{
			var id = Guid.NewGuid();
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.People_id == id);

			Assert.Single(builder.Params);
			Assert.Equal(id, builder.Params[0].Value);
		}

		[Fact]
		public void Exists()
		{
			var builder = Context.Select<CreeperStudentModel>()
				.WhereExists<CreeperPeopleModel>(binder => binder.Field(b => b.Id).Where(b => b.Id == _staticId));

			Assert.Single(builder.Params);
			Assert.Equal(_staticId, builder.Params[0].Value);

			//此种写法过长后面有待优化
			var builder1 = Context.Select<CreeperStudentModel>()
				.WhereExists<CreeperPeopleModel>(binder => binder.Field(b => b.Id).Where<CreeperStudentModel, CreeperPeopleModel>((a, b) => b.Id == a.People_id));
			var sql = builder1.ToString();
		}

		/// <summary>
		/// 此处测试的是递归获取值
		/// </summary>
		[Fact]
		public void ClassProperty()
		{
			var model = new TestModel { Id = Guid.NewGuid() };
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.People_id == model.Id);

			Assert.Single(builder.Params);
			Assert.Equal(model.Id, builder.Params[0].Value);
		}
		[Fact]
		public void Null()
		{
			var info = Context.Select<CreeperPeopleModel>().Where(a => a.Sex == null);

			Assert.Matches(@"IS\s+NULL", info.ToString());
		}
		[Fact]
		public void Default()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Id == default);

			Assert.Single(builder.Params);
			Assert.Equal(default(Guid), builder.Params[0].Value);
		}

		/// <summary>
		/// 此处测试的是递归获取值
		/// </summary>
		[Fact]
		public void InnerClassPeoperty()
		{
			var model = new ParentTestModel { Info = new TestModel { Id = Guid.NewGuid() } };
			var model1 = new ParentPreantTestModel { Info = new ParentTestModel { Info = new TestModel { Id = Guid.NewGuid() } } };
			var builder1 = Context.Select<CreeperStudentModel>().Where(a => a.People_id == model.Info.Id);

			Assert.Single(builder1.Params);
			Assert.Equal(model.Info.Id, builder1.Params[0].Value);

			var builder2 = Context.Select<CreeperStudentModel>().Where(a => a.People_id == model1.Info.Info.Id);

			Assert.Single(builder2.Params);
			Assert.Equal(model1.Info.Info.Id, builder2.Params[0].Value);
		}
		[Fact]
		public void ClassStaticMember()
		{
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.People_id == TestModel.StaticIdField);

			Assert.Single(builder.Params);
			Assert.Equal(TestModel.StaticIdField, builder.Params[0].Value);
		}

		[Fact]
		public void Const()
		{
			var builder1 = Context.Select<CreeperPeopleModel>().Where(a => a.Id == Guid.Empty);
			Assert.Single(builder1.Params);
			Assert.Equal(Guid.Empty, builder1.Params[0].Value);

			var builder2 = Context.Select<CreeperPeopleModel>().Where(a => a.Name == ConstName);
			Assert.Single(builder2.Params);
			Assert.Equal(ConstName, builder2.Params[0].Value);
		}

		[Fact]
		public void MethodLast()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Name == ConstName.ToString());

			Assert.Single(builder.Params);
			Assert.Equal(ConstName, builder.Params[0].Value);
		}

		/// <summary>
		/// 输出方法表达式返回值
		/// </summary>
		[Fact]
		public void MethodCallExpressionGetReturnValue()
		{
			var time = DateTime.Now;
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Create_time < time.AddDays(-1));

			Assert.Single(builder.Params);
			Assert.Equal(time.AddDays(-1), builder.Params[0].Value);
		}

		/// <summary>
		/// 输出方法表达式返回值
		/// </summary>
		[Fact]
		public void MethodCallExpression()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Id == Guid.Parse(_staticId.ToString()));

			Assert.Single(builder.Params);
			Assert.Equal(_staticId, builder.Params[0].Value);
		}

		[Fact]
		public void NewArray()
		{
			var builder1 = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type == new[] { 1 });

			Assert.Single(builder1.Params);
			Assert.IsType<int[]>(builder1.Params[0].Value);
			Assert.Equal(1, ((int[])builder1.Params[0].Value)[0]);

			var builder2 = Context.Select<CreeperTypeTestModel>().Where(a => a.Uuid_array_type == new[] { Guid.Empty });

			Assert.Single(builder2.Params);
			Assert.IsType<Guid[]>(builder2.Params[0].Value);
			Assert.Equal(Guid.Empty, ((Guid[])builder2.Params[0].Value)[0]);
		}

		[Fact]
		public void NewClass()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Id == new TestModel() { Id = _staticId }.Id);

			Assert.Single(builder.Params);
			Assert.Equal(_staticId, builder.Params[0].Value);
		}

		[Fact]
		public void NewStruct()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Id == new Guid());

			Assert.Single(builder.Params);
			Assert.Equal(new Guid(), builder.Params[0].Value);
		}

		[Fact]
		public void Multiple()
		{
			var builder = Context.Select<CreeperPeopleModel>().Where(a => (a.Id == new Guid() && a.Id == StuPeopleId1) || a.Id == StuPeopleId2);

			Assert.Equal(3, builder.Params.Count);
			Assert.Equal(new Guid(), builder.Params[0].Value);
			Assert.Equal(StuPeopleId1, builder.Params[1].Value);
			Assert.Equal(StuPeopleId2, builder.Params[2].Value);
		}

		[Fact]
		public void UnionMultiple()
		{
			var builder = Context.Select<CreeperPeopleModel>()
				.InnerJoin<CreeperStudentModel>((a, b) => a.Id == b.People_id && (b.People_id == StuPeopleId1 || a.Id == StuPeopleId2))
				.Where<CreeperStudentModel>(b => b.People_id == StuPeopleId1);

			Assert.Equal(3, builder.Params.Count);
			Assert.Equal(StuPeopleId1, builder.Params[0].Value);
			Assert.Equal(StuPeopleId2, builder.Params[1].Value);
			Assert.Equal(StuPeopleId1, builder.Params[2].Value);
		}

		[Fact]
		public void OperationExpression()
		{
			var result = Context.Select<CreeperTypeTestModel>()
				.Where(a => a.Int8_type + (int)TimeSpan.FromHours(1).TotalSeconds < DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString();
			var builder = Context.Select<CreeperPeopleModel>().Where(a => DateTime.Today - a.Create_time > TimeSpan.FromDays(2));

			Assert.Equal(2, builder.Params.Count);
			Assert.Equal(DateTime.Today, builder.Params[0].Value);
			Assert.Equal(TimeSpan.FromDays(2), builder.Params[1].Value);
		}

		[Fact]
		public void IndexParameter()
		{
			Guid[] ids = new[] { StuPeopleId1, Guid.Empty };
			var builder = Context.Select<CreeperPeopleModel>().Where(a => a.Id == ids[0]);

			Assert.Single(builder.Params);
			Assert.Equal(ids[0], builder.Params[0].Value);
		}

		/// <summary>
		/// 数据库成员的索引
		/// </summary>
		[Fact]
		public void DbArrayFieldIndexParameter()
		{
			var arr = new[] { 1 };
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type[1] == arr[0]);

			Assert.Single(builder.Params);
			Assert.Matches(@"\[2\]", builder.ToString());//此处使用的是postgresql规则, 数组索引从1开始, C#数组索引从0开始, 所以此处输出为sql语句是2
			Assert.Equal(arr[0], builder.Params[0].Value);
		}
		[Fact]
		public void DbArrayFieldLength()
		{
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type.Length == 2);

			Assert.Single(builder.Params);
			Assert.Matches(@"array_length\(", builder.ToString());//此处使用的是postgresql规则, 方法是array_length([column], 1) = @length
			Assert.Equal(2, builder.Params[0].Value);
		}

		/// <summary>
		/// 此处解析lambda表达式枚举类型有个转换表达式的问题, 所以要测得非常细;
		/// </summary>
		[Fact]
		public void Enum()
		{
			CreeperDataState value = CreeperDataState.正常;
			var builder1 = Context.Select<CreeperTypeTestModel>().Where(a => a.Enum_type == value);
			Assert.Single(builder1.Params);
			Assert.Equal(value, builder1.Params[0].Value);

			var builder2 = Context.Select<CreeperTypeTestModel>().Where(a => a.Enum_type == CreeperDataState.正常);
			Assert.Single(builder2.Params);
			Assert.Equal(CreeperDataState.正常, builder2.Params[0].Value);

			var builder3 = Context.Select<CreeperTypeTestModel>().Where(a => a.Int4_type == (int)value);
			Assert.Single(builder3.Params);
			Assert.Equal((int)value, builder3.Params[0].Value);

			var builder4 = Context.Select<CreeperTypeTestModel>().Where(a => a.Int4_type == (int)CreeperDataState.正常);
			Assert.Single(builder4.Params);
			Assert.Equal((int)CreeperDataState.正常, builder4.Params[0].Value);
		}

		/// <summary>
		/// 数组的包含查询
		/// </summary>
		[Fact]
		public void CollectionContains()
		{
			var xx = new CreeperPeopleModel() { Id = Guid.Empty };
			var b = new CreeperPeopleModel() { Id = Guid.Empty };

			var builder1 = Context.Select<CreeperTypeTestModel>().Where(a => new[] { xx.Id, b.Id }.Select(a => a).Contains(a.Id));
			Assert.Single(builder1.Params);
			Assert.Matches(@"ANY\(", builder1.ToString());
			Assert.IsType<Guid[]>(builder1.Params[0].Value);
			Assert.Equal(xx.Id, ((Guid[])builder1.Params[0].Value)[0]);

			var builder2 = Context.Select<CreeperTypeTestModel>().Where(a => new[] { xx.Id, b.Id }.Contains(a.Id));

			Assert.Single(builder2.Params);
			Assert.Matches(@"ANY\(", builder2.ToString());
			Assert.IsType<Guid[]>(builder2.Params[0].Value);
			Assert.Equal(xx.Id, ((Guid[])builder2.Params[0].Value)[0]);

			var builder3 = Context.Select<CreeperTypeTestModel>().Where(a => new[] { 1, 3, 4 }.Contains(a.Int4_type.Value));
			Assert.Single(builder3.Params);
			Assert.Matches(@"ANY\(", builder3.ToString());
			Assert.IsType<int[]>(builder3.Params[0].Value);
			Assert.Equal(1, ((int[])builder3.Params[0].Value)[0]);

			//3 <> ALL(a."array_type")
			var builder4 = Context.Select<CreeperTypeTestModel>().Where(a => !a.Array_type.Contains(3));
			Assert.Single(builder4.Params);
			Assert.Matches(@"ALL\(", builder4.ToString());
			Assert.Equal(3, builder4.Params[0].Value);

			//3 = any(a.array_type)
			var builder5 = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type.Contains(3));
			Assert.Single(builder5.Params);
			Assert.Matches(@"ANY\(", builder5.ToString());
			Assert.Equal(3, builder5.Params[0].Value);

			//var ints = new int[] { 2, 3 }.Select(f => f).ToList();
			//a.int_type = any(array[2,3])
			var builder6 = Context.Select<CreeperTypeTestModel>().Where(a => new int[] { 2, 3 }.Select(f => f).Contains(a.Int4_type.Value));
			Assert.Single(builder6.Params);
			Assert.Matches(@"ANY\(", builder6.ToString());
			Assert.IsType<int[]>(builder6.Params[0].Value);
			Assert.Equal(2, ((int[])builder6.Params[0].Value)[0]);

			var builder7 = Context.Select<CreeperTypeTestModel>().Where(a => new[] { (int)CreeperDataState.删除, (int)CreeperDataState.正常 }.Contains(a.Int4_type.Value));
			Assert.Single(builder7.Params);
			Assert.Matches(@"ANY\(", builder7.ToString());
			Assert.IsType<int[]>(builder7.Params[0].Value);
			Assert.Equal((int)CreeperDataState.删除, ((int[])builder7.Params[0].Value)[0]);
		}

		[Fact]
		public void StringLike()
		{
			var str = "xxx";

			//'xxx' like a.Varchar_type || '%'
			var builder1 = Context.Select<CreeperTypeTestModel>().Where(a => str.StartsWith(a.Varchar_type));
			Assert.Single(builder1.Params);
			Assert.Equal(str, builder1.Params[0].Value);

			//a.varchar_type like '%xxx%'
			var builder2 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.Contains(str));
			Assert.Single(builder2.Params);
			Assert.Equal(str, builder2.Params[0].Value);

			//a.varchar_type like 'xxx%'
			var builder3 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.StartsWith(str));
			Assert.Single(builder3.Params);
			Assert.Equal(str, builder3.Params[0].Value);

			//a.varchar_type like '%xxx'
			var builder4 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.EndsWith(str));
			Assert.Single(builder4.Params);
			Assert.Equal(str, builder4.Params[0].Value);

			//a.varchar_type ilike '%xxx%'
			var builder5 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.Contains(str, StringComparison.OrdinalIgnoreCase));
			Assert.Single(builder5.Params);
			Assert.Equal(str, builder5.Params[0].Value);

			//a.varchar_type ilike 'xxx%'
			var builder6 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.StartsWith(str, StringComparison.OrdinalIgnoreCase));
			Assert.Single(builder6.Params);
			Assert.Equal(str, builder6.Params[0].Value);

			//a.varchar_type ilike '%xxx'
			var builder7 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.EndsWith(str, StringComparison.OrdinalIgnoreCase));
			Assert.Single(builder7.Params);
			Assert.Equal(str, builder7.Params[0].Value);
		}

		[Fact]
		public void ToStringMethod()
		{
			//a.varchar_type::text = 'xxxx'
			var builder1 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.ToString() == "xxxx");
			Assert.Single(builder1.Params);
			Assert.Equal("xxxx", builder1.Params[0].Value);

			ParentPreantTestModel model = new ParentPreantTestModel { Name = "xxxx" };
			//a.varchar_type::text = 'xxxx'
			var builder2 = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.ToString() == model.Name.ToString());
			Assert.Single(builder2.Params);
			Assert.Equal("xxxx", builder2.Params[0].Value);

		}

		[Fact]
		public void EqualsFunction()
		{
			Func<string, string> fuc = (str) => "xxxx";
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type == fuc("xxxx"));
			Assert.Single(builder.Params);
			Assert.Equal("xxxx", builder.Params[0].Value);
		}

		[Fact]
		public void Block()
		{
			var judge = 0;
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type == (judge == 0 ? "xxxx" : ""));
			Assert.Single(builder.Params);
			Assert.Equal("xxxx", builder.Params[0].Value);
		}

		[Fact]
		public void Coalesce()
		{
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => (a.Int4_type ?? 3) == 3);
			var sql = builder.ToString();
			Assert.Equal(2, builder.Params.Count);
			Assert.Matches(@"COALESCE\(", builder.ToString());
			Assert.Equal(3, builder.Params[0].Value);
			Assert.Equal(3, builder.Params[1].Value);

		}
		[Fact]
		public void FieldWithNamespace()
		{
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.People_id == Creeper.xUnitTest.PostgreSql.BaseTest.StuPeopleId1);

			Assert.Single(builder.Params);
			Assert.Equal(StuPeopleId1, builder.Params[0].Value);
		}

		[Fact]
		public void CompareSelf()
		{
			var builder = Context.Select<CreeperStudentModel>().Where(a => a.People_id == a.Id || a.People_id == StuPeopleId1);

			Assert.Single(builder.Params);
			Assert.Matches(@"a\.\Speople_id\S\s+=\s+a\.\Sid\S", builder.ToString());
			Assert.Equal(StuPeopleId1, builder.Params[0].Value);
		}

		[Fact]
		public void In()
		{
			var builder = Context.Select<CreeperStudentModel>().WhereIn<CreeperStudentModel>(a => a.People_id, binder => binder.Field(a => a.People_id).Where(a => a.People_id == StuPeopleId1));

			Assert.Single(builder.Params);
			Assert.Equal(StuPeopleId1, builder.Params[0].Value);
		}

		[Fact]
		public void DotValue()
		{
			var builder = Context.Select<CreeperTypeTestModel>().Where(a => a.Int2_type.Value == 0);

			Assert.Single(builder.Params);
			Assert.Matches(@"a\.\Sint2_type\S\s+=\s+", builder.ToString());
			Assert.Equal(0, builder.Params[0].Value);
		}

		public class ParentPreantTestModel
		{
			public string Name { get; set; }
			public ParentTestModel Info { get; set; }
		}
		public class ParentTestModel
		{
			public TestModel Info { get; set; }
		}
		public class TestModel
		{
			public static Guid StaticIdField = StuPeopleId1;
			public Guid Id { get; set; }
		}
	}
}
