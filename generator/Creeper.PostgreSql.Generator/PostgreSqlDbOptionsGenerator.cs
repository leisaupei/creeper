using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class PostgreSqlDbOptionsGenerator
	{
		/// <summary>
		/// 项目名称
		/// </summary>
		private static string _projectName = string.Empty;

		/// <summary>
		/// model目录
		/// </summary>
		private static string _modelPath = string.Empty;

		/// <summary>
		/// 根目录
		/// </summary>
		private static string _rootPath = string.Empty;

		/// <summary>
		/// 数据库类别名称
		/// </summary>
		private static string _typeName = string.Empty;

		/// <summary>
		/// 命名空间后缀
		/// </summary>
		private static string NamespaceSuffix => _folder ? "." + _typeName : "";

		private static readonly StringBuilder _sbConstTypeName = new StringBuilder();
		private static readonly StringBuilder _sbNamespace = new StringBuilder();
		private static readonly StringBuilder _sbConstTypeConstrutor = new StringBuilder();
		private readonly ICreeperDbExecute _dbExecute;
		private readonly PostgresExcepts _postgresExcepts;
		private static bool _folder { get; set; }

		public PostgreSqlDbOptionsGenerator(ICreeperDbExecute dbExecute, PostgresExcepts postgresExcepts, bool folder)
		{
			_dbExecute = dbExecute;
			_postgresExcepts = postgresExcepts;
			_folder = folder;
		}

		/// <summary>
		/// 生成枚举数据库枚举类型(覆盖生成)
		/// </summary>
		/// <param name="rootPath">根目录</param>
		/// <param name="modelPath">Model目录</param>
		/// <param name="projectName">项目名称</param>
		/// <param name="typeName">多库标签</param>
		public void Generate(string rootPath, string modelPath, string projectName, string typeName)
		{
			_typeName = typeName;
			_rootPath = rootPath;
			_modelPath = modelPath;
			_projectName = projectName;
			var listEnum = GenerateEnum();
			var listComposite = GenerateComposites();

			GenerateMapping(listEnum, listComposite);
		}


		private List<EnumTypeInfo> GenerateEnum()
		{
			var sql = $@"
SELECT a.oid, a.typname, b.nspname FROM pg_type a  
INNER JOIN pg_namespace b ON a.typnamespace = b.oid 
WHERE a.typtype='e'  
ORDER BY oid asc  
";
			var list = _dbExecute.ExecuteDataReaderList<EnumTypeInfo>(sql);
			string fileName = Path.Combine(_modelPath, $"_Enums.cs");
			using (StreamWriter writer = new StreamWriter(File.Create(fileName), Encoding.UTF8))
			{
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.{CreeperGenerator.Namespace}{NamespaceSuffix}");
				writer.WriteLine("{");
				foreach (var item in list)
				{
					var sqlEnums = $@"SELECT enumlabel FROM pg_enum a  WHERE enumtypid=@oid ORDER BY oid asc";
					var enums = _dbExecute.ExecuteDataReaderList<string>(sqlEnums, System.Data.CommandType.Text, new[] { new NpgsqlParameter("oid", item.Oid) });
					if (enums.Count > 0)
						enums[0] += " = 1";
					writer.WriteLine($"\tpublic enum {Types.DeletePublic(item.Nspname, item.Typname)}");
					writer.WriteLine("\t{");
					writer.WriteLine($"\t\t{string.Join(", ", enums)}");
					writer.WriteLine("\t}");

				}
				writer.WriteLine("}");
			}
			return list;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public List<CompositeTypeInfo> GenerateComposites()
		{
			var sql = $@"
SELECT ns.nspname, a.typname as typename, c.attname, d.typname, c.attndims, d.typtype
FROM pg_type a 
INNER JOIN pg_class b on b.reltype = a.oid and b.relkind = 'c'
INNER JOIN pg_attribute c on c.attrelid = b.oid and c.attnum > 0
INNER JOIN pg_type d on d.oid = c.atttypid
INNER JOIN pg_namespace ns on ns.oid = a.typnamespace
LEFT JOIN pg_namespace ns2 on ns2.oid = d.typnamespace
WHERE ns.nspname || '.' || a.typname not in ({Types.ConvertArrayToSql(_postgresExcepts.Global.Composites)})
";
			Dictionary<string, string> dic = new Dictionary<string, string>();
			List<CompositeTypeInfo> composites = new List<CompositeTypeInfo>();
			var isFoot = false;
			_dbExecute.ExecuteDataReader(dr =>
		   {
			   var composite = new CompositeTypeInfo
			   {
				   Nspname = dr["nspname"]?.ToString(),
				   Typname = dr["typename"]?.ToString(),
			   };
			   var temp = $"{composite.Nspname}.{composite.Typname}";

			   if (!dic.ContainsKey(temp))
			   {
				   var str = "";
				   if (isFoot)
				   {
					   str += "\t}\n";
					   isFoot = false;
				   }
				   str += $"\t[JsonObject(MemberSerialization.OptIn)]\n";
				   str += $"\tpublic partial struct {Types.DeletePublic(composite.Nspname, composite.Typname)}\n";
				   str += "\t{";
				   dic.Add(temp, str);
				   composites.Add(composite);
			   }
			   else isFoot = true;
			   var isArray = Convert.ToInt16(dr["attndims"]) > 0;
			   string _type = Types.ConvertPgDbTypeToCSharpType(dr["typtype"].ToString(), dr["typname"].ToString());
			   var _notnull = string.Empty;
			   if (_type != "string" && _type != "JToken" && _type != "byte[]" && !isArray && _type != "object" && _type != "IPAdress")
				   _notnull = "?";
			   string _array = isArray ? "[]" : "";
			   var relType = $"{_type}{_notnull}{_array}";
			   dic[temp] += $"\n\t\t[JsonProperty] public {relType} {dr["attname"].ToString().ToUpperPascal()} {{ get; set; }}";

		   }, sql);

			if (dic.Count > 0)
			{
				string fileName = Path.Combine(_modelPath, $"_{TypeName}Composites.cs");
				using StreamWriter writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine("using Newtonsoft.Json;");
				writer.WriteLine();
				writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.{CreeperGenerator.Namespace}{NamespaceSuffix}");
				writer.WriteLine("{");
				using (var e = dic.GetEnumerator())
				{
					while (e.MoveNext())
						writer.WriteLine(e.Current.Value);
					if (dic.Keys.Count > 0)
						writer.WriteLine("\t}");
				}
				writer.WriteLine("}");
			}
			return composites;
		}

		private string TypeName => _typeName == GenerateOption.MASTER_DATABASE_TYPE_NAME ? "" : _typeName;

		/// <summary>
		/// 生成初始化文件(覆盖生成)
		/// </summary>
		/// <param name="list"></param>
		/// <param name="listComposite"></param>
		public void GenerateMapping(List<EnumTypeInfo> list, List<CompositeTypeInfo> listComposite)
		{
			_sbNamespace.AppendLine($"using {_projectName}.{CreeperGenerator.DbStandardSuffix}.{CreeperGenerator.Namespace}{NamespaceSuffix};");

			_sbConstTypeName.AppendLine("\t/// <summary>");
			_sbConstTypeName.AppendLine($"\t/// {TypeName}主库");
			_sbConstTypeName.AppendLine("\t/// </summary>");
			_sbConstTypeName.AppendLine($"\tpublic struct Db{_typeName.ToUpperPascal()} : ICreeperDbName {{ }}");
			_sbConstTypeName.AppendLine("\t/// <summary>");
			_sbConstTypeName.AppendLine($"\t/// {TypeName}从库");
			_sbConstTypeName.AppendLine("\t/// </summary>");
			_sbConstTypeName.AppendLine($"\tpublic struct Db{TypeName + CreeperDbContext.SecondarySuffix} : ICreeperDbName {{ }}");

			_sbConstTypeConstrutor.AppendLine($"\t\t#region {_typeName}");
			_sbConstTypeConstrutor.AppendLine(string.Format("\t\tpublic class {0}PostgreSqlDbOption : BasePostgreSqlDbOption<Db{0}, Db{1}>", _typeName.ToUpperPascal(), TypeName + CreeperDbContext.SecondarySuffix));
			_sbConstTypeConstrutor.AppendLine("\t\t{");
			_sbConstTypeConstrutor.AppendLine(string.Format("\t\t\tpublic {0}PostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings)  {{ }}", _typeName.ToUpperPascal(), TypeName));
			_sbConstTypeConstrutor.AppendLine("\t\t\tpublic override DbConnectionOptions Options => new DbConnectionOptions()");
			_sbConstTypeConstrutor.AppendLine("\t\t\t{");
			_sbConstTypeConstrutor.AppendLine("\t\t\t\tMapAction = conn =>");
			_sbConstTypeConstrutor.AppendLine("\t\t\t\t{");
			_sbConstTypeConstrutor.AppendLine("\t\t\t\t\tconn.TypeMapper.UseJsonNetForJtype();");
			if (MappingOptions.XmlTypeName.Contains(_typeName))
				_sbConstTypeConstrutor.AppendLine("\t\t\t\t\tconn.TypeMapper.UseCustomXml();");
			if (MappingOptions.GeometryTableTypeName.Contains(_typeName))
				_sbConstTypeConstrutor.AppendLine("\t\t\t\t\tconn.TypeMapper.UseLegacyPostgis();");
			foreach (var item in list)
				_sbConstTypeConstrutor.AppendLine($"\t\t\t\t\tconn.TypeMapper.MapEnum<{CreeperGenerator.Namespace}{NamespaceSuffix}.{Types.DeletePublic(item.Nspname, item.Typname)}>(\"{item.Nspname}.{item.Typname}\", PostgreSqlTranslator.Instance);");
			foreach (var item in listComposite)
				_sbConstTypeConstrutor.AppendLine($"\t\t\t\t\tconn.TypeMapper.MapComposite<{CreeperGenerator.Namespace}{NamespaceSuffix}.{Types.DeletePublic(item.Nspname, item.Typname)}>(\"{item.Nspname}.{item.Typname}\");");
			_sbConstTypeConstrutor.AppendLine("\t\t\t\t}");
			_sbConstTypeConstrutor.AppendLine("\t\t\t};");
			_sbConstTypeConstrutor.AppendLine("\t\t}");
			_sbConstTypeConstrutor.AppendLine("\t\t#endregion");
			_sbConstTypeConstrutor.AppendLine();
			//Write();

		}

		public static void WritePostgreSqlDbOptions()
		{
			var startupRoot = Path.Combine(_rootPath, "Options");
			if (!Directory.Exists(startupRoot))
				Directory.CreateDirectory(startupRoot);
			var fileName = Path.Combine(startupRoot, $"PostgreSqlDbOptions.cs");
			using StreamWriter writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.Write(_sbNamespace);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json.Linq;");
			writer.WriteLine("using Npgsql.TypeMapping;");
			writer.WriteLine("using Creeper.PostgreSql.Extensions;");
			writer.WriteLine("using Npgsql;");
			writer.WriteLine("using Creeper.PostgreSql;");
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.Options");
			writer.WriteLine("{");
			writer.WriteLine("\t#region DbName");
			writer.Write(_sbConstTypeName);
			writer.WriteLine("\t#endregion");
			writer.WriteLine($"\tpublic static class PostgreSqlDbOptions");
			writer.WriteLine("\t{");
			writer.WriteLine();
			writer.Write(_sbConstTypeConstrutor);
			writer.WriteLine("\t}");
			writer.WriteLine("}"); // namespace end
		}
	}
}