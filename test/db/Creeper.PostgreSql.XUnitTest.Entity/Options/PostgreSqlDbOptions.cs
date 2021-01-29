/* ##########################################################
 * #   此文件由 https://github.com/leisaupei/creeper 生成    #
 * ##########################################################
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
	/// <summary>
	/// 由生成器生成, 会覆盖
	/// </summary>
	public static class PostgreSqlDbOptions
	{

		#region Main
		public class MainPostgreSqlDbOption : BasePostgreSqlDbOption<DbMain, DbSecondary>
		{
			public MainPostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings)  { }
			public override DbConnectionOptions Options => new DbConnectionOptions()
			{
				MapAction = conn =>
				{
					conn.TypeMapper.UseJsonNetForJtype();
					conn.TypeMapper.UseCustomXml();
					conn.TypeMapper.MapEnum<Model.EDataState>("public.e_data_state", PostgreSqlTranslator.Instance);
					conn.TypeMapper.MapComposite<Model.Info>("public.info");
				}
			};
		}
		#endregion

	}
}
