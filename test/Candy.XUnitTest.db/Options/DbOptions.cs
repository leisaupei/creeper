using Meta.xUnitTest.Model;
using System;
using Meta.Driver.Model;
using Meta.Driver.DbHelper;
using Newtonsoft.Json.Linq;
using Npgsql.TypeMapping;
using Meta.Driver.Extensions;
using Npgsql;
using Candy.Driver.Common;
using Candy.Driver.Model;
using Candy.Driver.PostgreSql;
using Candy.Driver.Extensions;

namespace Candy.XUnitTest.Options
{
	#region DbName
	/// <summary>
	/// 主库
	/// </summary>
	public struct DbMain : IDbName { }
	/// <summary>
	/// 从库
	/// </summary>
	public struct DbSecondary : IDbName { }
	#endregion
	/// <summary>
	/// 由生成器生成, 会覆盖
	/// </summary>
	public static class DbOptions
	{

		#region Main
		public class MainDbOption : BasePostgreSqlDbOption<DbMain, DbSecondary>
		{
			public MainDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings)
			{
				Options.MapAction = conn =>
				{
					conn.TypeMapper.UseJsonNetForJtype();
					conn.TypeMapper.UseCustomXml();
					conn.TypeMapper.MapEnum<Model.EDataState>("public.e_data_state", _translator);
					conn.TypeMapper.MapComposite<Model.Info>("public.info");
				};
			}
		}
		#endregion

		#region Private
		private static readonly NpgsqlNameTranslator _translator = new NpgsqlNameTranslator();
		private class NpgsqlNameTranslator : INpgsqlNameTranslator
		{
			public string TranslateMemberName(string clrName) => clrName;
			public string TranslateTypeName(string clrName) => clrName;
		}
		#endregion
	}
}
