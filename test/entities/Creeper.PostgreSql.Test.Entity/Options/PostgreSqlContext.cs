using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Data.Common;
using Npgsql;
using Creeper.PostgreSql.Extensions;

namespace Creeper.PostgreSql.Test.Entity.Options
{
	public class PostgreSqlContext : CreeperContextBase
	{
		public PostgreSqlContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

		protected override Action<DbConnection> ConnectionOptions => connection =>
		{
			var c = (NpgsqlConnection)connection;
			c.TypeMapper.UseNewtonsoftJson();
			c.TypeMapper.UseSystemXmlDocument();
			c.TypeMapper.UseLegacyPostgis();
			c.TypeMapper.MapEnum<Model.CreeperDataState>("creeper.data_state", PostgreSqlTranslator.Instance);
			c.TypeMapper.MapComposite<Model.CreeperInfo>("creeper.info");
		};
	}

}
