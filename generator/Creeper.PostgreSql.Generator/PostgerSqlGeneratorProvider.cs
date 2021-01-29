using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	public class PostgerSqlGeneratorProvider : ICreeperGeneratorProvider
	{
		private readonly PostgresExcepts _postgresExcepts;

		public PostgerSqlGeneratorProvider(IOptions<PostgresExcepts> optionsAccessor)
		{
			_postgresExcepts = optionsAccessor.Value;
		}

		public DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		public void ModelGenerator(string modelPath, GenerateOption option, ICreeperDbConnectionOption dbOption, bool folder)
		{
			if (folder) modelPath = Path.Combine(modelPath, dbOption.DbName);

			CreeperGenerator.RecreateDir(modelPath);

			var execute = new CreeperDbExecute(dbOption);
			var schemaList = GetSchemas(execute);
			foreach (var schemaName in schemaList)
			{
				List<TableViewModel> tableList = GetTables(execute, schemaName);
				foreach (var item in tableList)
				{
					TableViewGenerator td = new TableViewGenerator(execute, folder);
					td.Generate(option.ProjectName, modelPath, schemaName, item, dbOption.DbName);
					td.ModelGenerator();
				}
			}
			var enumsDal = new PostgreSqlDbOptionsGenerator(execute, _postgresExcepts, folder);
			enumsDal.Generate(Path.Combine(option.OutputPath, option.ProjectName + "." + CreeperGenerator.DbStandardSuffix), modelPath, option.ProjectName, dbOption.DbName);
		}

		public ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn)
		{
			var strings = conn.Split(';');
			var connectionString = string.Empty;
			string dbName = null;
			foreach (var item in strings)
			{
				var sp = item.Split('=');
				var left = sp[0];
				var right = sp[1];

				switch (left.ToLower())
				{
					case "host": connectionString += $"host={right};"; break;
					case "port": connectionString += $"port={right};"; break;
					case "user": connectionString += $"username={right};"; break;
					case "pwd": connectionString += $"password={right};"; break;
					case "db": connectionString += $"database={right};"; break;
					case "name": dbName = ToUpperPascal(string.IsNullOrEmpty(right) ? GenerateOption.MASTER_DATABASE_TYPE_NAME : right); break;
				}
			}
			connectionString += $"maximum pool size=32;pooling=true;CommandTimeout=300";
			dbName = string.IsNullOrEmpty(dbName) ? GenerateOption.MASTER_DATABASE_TYPE_NAME : dbName;
			ICreeperDbConnectionOption connections = new PostgreSqlDbConnectionOptions(connectionString, dbName, null);
			return connections;
		}
		private static string ToUpperPascal(string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToUpper()}{s[1..]}";

		/// <summary>
		/// 获取模式名称
		/// </summary>
		/// <returns></returns>
		private List<string> GetSchemas(ICreeperDbExecute execute)
		{
			string sql = $@"
				SELECT SCHEMA_NAME AS schemaname 
				FROM information_schema.schemata a  
				WHERE SCHEMA_NAME NOT IN ({Types.ConvertArrayToSql(_postgresExcepts.Global.Schemas)})  
				ORDER BY SCHEMA_NAME";

			return execute.ExecuteDataReaderList<string>(sql);
		}

		/// <summary>
		/// 获取所有表
		/// </summary>
		/// <param name="schemaName"></param>
		/// <returns></returns>
		private List<TableViewModel> GetTables(ICreeperDbExecute execute, string schemaName)
		{
			var sql = $@"
				SELECT tablename AS name, 'table' AS type, CAST(obj_description(b.oid,'pg_class') AS VARCHAR) AS description
				FROM pg_tables a  
				LEFT JOIN pg_class b on a.tablename = b.relname AND b.relkind = 'r' 
				INNER JOIN pg_namespace c on c.oid = b.relnamespace AND c.nspname = a.schemaname
				WHERE tablename NOT IN ({Types.ConvertArrayToSql(_postgresExcepts.Global.Tables)})
				AND schemaname = '{schemaName}'
				AND tablename NOT LIKE '%copy%'  
				UNION (
					SELECT viewname AS name,'view' AS type, CAST(obj_description(b.oid,'pg_class') AS VARCHAR) AS description
					FROM pg_views a  
					LEFT JOIN pg_class b on a.viewname = b.relname AND b.relkind = 'v' 
					INNER JOIN pg_namespace c on c.oid = b.relnamespace AND c.nspname = a.schemaname
					WHERE viewname NOT IN ({Types.ConvertArrayToSql(_postgresExcepts.Global.Views)})
					AND schemaname = '{schemaName}'
				)  
			";
			return execute.ExecuteDataReaderList<TableViewModel>(sql);
		}

		public Action GetFinallyGen()
		{
			return PostgreSqlDbOptionsGenerator.WritePostgreSqlDbOptions;
		}
	}
}
