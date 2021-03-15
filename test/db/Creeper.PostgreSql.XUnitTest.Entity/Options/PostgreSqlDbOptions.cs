/* ################################################################################
 * # 此文件由生成器创建或覆盖。see: https://github.com/leisaupei/creeper
 * ################################################################################
 */
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System;
using Newtonsoft.Json.Linq;
using Npgsql.TypeMapping;
using Creeper.PostgreSql.Extensions;
using Npgsql;
using Creeper.PostgreSql;
using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest.Entity.Options
{
	#region DbName
	/// <summary>
	/// 主库
	/// </summary>
	public struct DbMain : ICreeperDbName { }
	/// <summary>
	/// 从库
	/// </summary>
	public struct DbSecondary : ICreeperDbName { }
	#endregion
	public static class PostgreSqlDbOptions
	{

		#region Main
		public class MainPostgreSqlDbOption : BasePostgreSqlDbOption<DbMain, DbSecondary>
		{
			public MainPostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings) { }
			public override DbConnectionOptions Options => new DbConnectionOptions()
			{
				MapAction = conn =>
				{
					conn.TypeMapper.UseJsonNetForJtype();
					conn.TypeMapper.UseCustomXml();
					conn.TypeMapper.MapEnum<Model.EtDataState>("public.et_data_state", PostgreSqlTranslator.Instance);
					conn.TypeMapper.MapEnum<Model.EDataState>("public.e_data_state", PostgreSqlTranslator.Instance);
					conn.TypeMapper.MapComposite<Model.Info>("public.info");
				}
			};
		}
		#endregion

	}
}
