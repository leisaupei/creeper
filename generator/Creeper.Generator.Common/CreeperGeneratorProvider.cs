using Creeper.Annotations;
using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.Generator.Common
{
	public abstract class CreeperGeneratorProvider : ICreeperGeneratorProvider
	{
		/// <summary>
		/// 获取数据库连接字符串使用的构造, 在依赖注入使用
		/// </summary>
		protected CreeperGeneratorProvider() { }

		/// <summary>
		/// 使用生成逻辑的构造, 在派生实现使用
		/// </summary>
		/// <param name="generateRules"></param>
		/// <param name="options"></param>
		/// <param name="generateConnection"></param>
		protected CreeperGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection generateConnection)
		{
			GenerateRules = generateRules;
			Options = options;
			GenerateConnection = generateConnection;
		}

		/// <summary>
		/// 起始是数字的正则匹配实例
		/// </summary>
		protected static Regex StartWithNumberRegex { get; } = new Regex(@"^[0-9]");

		/// <summary>
		/// 数据库类型
		/// </summary>
		public abstract DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 排除规则
		/// </summary>
		protected GenerateRules GenerateRules { get; }

		/// <summary>
		/// 数据库生成器的全局配置
		/// </summary>
		protected CreeperGeneratorGlobalOptions Options { get; }

		/// <summary>
		/// 构建连接字符串实体
		/// </summary>
		protected CreeperGenerateConnection GenerateConnection { get; }

		/// <summary>
		/// 获取数据库连接
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		public abstract CreeperGenerateConnection GetDbConnectionOptionFromString(string build);

		/// <summary>
		/// 根据规则转换
		/// </summary>
		/// <param name="tableColumns"></param>
		public abstract List<TableInfo> GetAllColumns();

		/// <summary>
		/// 获取特性的表名
		/// </summary>
		/// <param name="tableView"></param>
		/// <returns></returns>
		public abstract string GetAttributeTableName(TableViewInfo tableView);

		/// <summary>
		/// 获取驼峰的表名
		/// </summary>
		/// <param name="tableView"></param>
		/// <returns></returns>
		public virtual string GetTableNameForCamel(TableViewInfo tableView) => GeneratorHelper.ExceptUnderlineToUpper(tableView.Name.ToUpperPascal());

		/// <summary>
		/// 生成Context.cs文件
		/// </summary>
		private void GenerateContextFile()
		{
			var fileName = Options.GetContextFileFullName(DataBaseKind);

			var dbConnectionOptionsUsingNamespace = GetDbConnectionOptionsUsingNamespace();

			var dbConnectionOptionsActionStrings = GetDbConnectionOptionsActionString();

			dbConnectionOptionsActionStrings = !string.IsNullOrWhiteSpace(dbConnectionOptionsActionStrings)
				? string.Format("{0}{0}{1}", Environment.NewLine, dbConnectionOptionsActionStrings) : null;

			var contextClassStr = string.Format(@"	public class {0}Context : {2}
	{{
		public {0}Context(IServiceProvider serviceProvider) : base(serviceProvider) {{ }}{1}
	}}
", GenerateConnection.Name, dbConnectionOptionsActionStrings, nameof(CreeperContextBase));
			if (!File.Exists(fileName))
			{
				using var writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
				writer.WriteLine("using Creeper.Driver;");
				writer.WriteLine("using Creeper.Generic;");
				writer.WriteLine("using System;");
				foreach (var item in dbConnectionOptionsUsingNamespace)
					writer.WriteLine(item);
				writer.WriteLine();
				writer.WriteLine("namespace {0}", Options.OptionsNamespace);
				writer.WriteLine("{");
				writer.WriteLine(contextClassStr);
				writer.WriteLine("}"); // namespace end
				return;
			}

			var lines = File.ReadAllLines(fileName).ToList();
			var namespaceLines = lines.GetRange(0, 20);
			lines.InsertRange(0, dbConnectionOptionsUsingNamespace.Where(a => !namespaceLines.Contains(a)));
			var writeLines = new List<string> { contextClassStr };
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(fileName, lines);
		}

		/// <summary>
		/// 获取数据库连接委托字符串
		/// </summary>
		/// <returns></returns>
		public virtual string GetDbConnectionOptionsActionString() => null;

		/// <summary>
		/// 获取数据库委托DbOptions需要引用的命名空间
		/// </summary>
		/// <returns></returns>
		public virtual List<string> GetDbConnectionOptionsUsingNamespace() => new List<string>();

		/// <summary>
		/// 数据库文件生成入口
		/// </summary>
		public void Generate()
		{
			var tableColumns = GetAllColumns();
			foreach (var tableColumn in tableColumns)
				GenerateModel(tableColumn);

			var enumTypes = GetEnumTypes();
			GenerateEnum(enumTypes);
			var compositeTypes = GetCompositeTypes();
			GenerateComposite(compositeTypes);
			GenerateContextFile();

		}

		/// <summary>
		/// 生成复合类型_Composite.cs文件
		/// </summary>
		/// <param name="compositeTypes"></param>
		private void GenerateComposite(List<CompositeType> compositeTypes)
		{
			if (compositeTypes.Count == 0) return;
			using StreamWriter writer = new StreamWriter(File.Create(Options.GetMultipleCompositesCsFullName(GenerateConnection.Name)), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader(writer);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", Options.GetModelNamespaceFullName(GenerateConnection.Name));
			writer.WriteLine("{");
			foreach (var item in compositeTypes)
			{
				writer.Write(GeneratorHelper.WriteSummary(item.Description, 1));
				writer.WriteLine($"\tpublic partial struct {item.CsharpClassName}");
				writer.WriteLine("\t{");
				foreach (var column in item.Columns)
				{
					writer.WriteLine($"\t\tpublic {column.CsharpTypeName} {column.NameUpCase} {{ get; set; }}");
				}
				writer.WriteLine("\t}");
			}
			writer.WriteLine("}");
		}

		/// <summary>
		/// 获取所有复合类型
		/// </summary>
		/// <returns></returns>
		public virtual List<CompositeType> GetCompositeTypes() => new List<CompositeType>();

		/// <summary>
		/// 生成枚举类型_Enum.cs文件
		/// </summary>
		/// <param name="enumTypes"></param>
		private void GenerateEnum(List<EnumType> enumTypes)
		{
			if (enumTypes.Count == 0)
				return;
			using StreamWriter writer = new StreamWriter(File.Create(Options.GetMultipleEnumCsFullName(GenerateConnection.Name)), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader(writer);
			writer.WriteLine("using System;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", Options.GetModelNamespaceFullName(GenerateConnection.Name));
			writer.WriteLine("{");
			foreach (var item in enumTypes)
			{
				if (item.Elements.Length == 0)
					continue;

				foreach (var e in item.Elements)
				{
					if (StartWithNumberRegex.IsMatch(e))
						throw new CreeperNotSupportedException("暂不支持起始为数字的枚举成员: " + item.CsharpTypeName + '.' + e);
				}

				item.Elements[0] += " = 1";

				writer.Write(GeneratorHelper.WriteSummary(item.Description, 1));
				writer.WriteLine("\tpublic enum {0}", item.CsharpTypeName);
				writer.WriteLine("\t{");
				writer.WriteLine($"\t\t{string.Join(", ", item.Elements)}");
				writer.WriteLine("\t}");

			}
			writer.WriteLine("}");
		}

		/// <summary>
		/// 获取所有枚举成员
		/// </summary>
		/// <param name="execute"></param>
		/// <returns></returns>
		public virtual List<EnumType> GetEnumTypes() => new List<EnumType>();

		/// <summary>
		/// 获取数据库所有列(字段), 一次性获取可调用此方法
		/// </summary>
		/// <returns></returns>
		protected List<TableInfo> GetAllColumns(string sql)
		{
			var tableColumns = new List<TableInfo>();
			GenerateConnection.DbExecute.ExecuteReader(dr => SetTableColumnValues(dr, tableColumns), sql);
			return tableColumns;
		}

		protected void SetTableColumnValues(DbDataReader dr, List<TableInfo> tableColumns)
		{
			var table = new TableViewInfo();
			var column = new ColumnInfo();

			for (int i = 0; i < dr.FieldCount; i++)
			{
				var name = dr.GetName(i).ToLower();
				switch (name)
				{
					case "tablename": table.Name = CheckString(dr[i]); break;
					case "schemaname": table.SchemaName = CheckString(dr[i]); break;
					case "tabledescription": table.Description = CheckString(dr[i]); break;
					case "isview": table.IsView = CheckBoolean(dr[i]) ?? false; break;

					case "datatype": column.DataType = CheckString(dr[i]); break;
					case "defaultvalue": column.DefaultValue = CheckString(dr[i]); break;
					case "columndescription": column.Description = CheckString(dr[i]); break;
					case "isidentity": column.IsIdentity = CheckBoolean(dr[i]) ?? false; break;
					case "isprimary": column.IsPrimary = CheckBoolean(dr[i]) ?? false; break;
					case "isnullable": column.IsNullable = CheckBoolean(dr[i]) ?? false; break;
					case "isunique": column.IsUnique = CheckBoolean(dr[i]) ?? false; break;
					case "length": column.Length = CheckInt(dr[i]) ?? -1; break;
					case "name": column.Name = CheckString(dr[i]); break;
					case "dimensions": column.Dimensions = CheckInt(dr[i]) ?? 0; break;
					case "typcategory": column.Typcategory = CheckString(dr[i]); break;
					case "datatypeschemaname": column.SchemaName = CheckString(dr[i]); break;
					case "precision": column.Precision = CheckInt(dr[i]) ?? -1; break;
					case "scale": column.Scale = CheckInt(dr[i]) ?? -1; break;
				}
			}
			if (column.Name.Contains(" "))
				throw new ArgumentException($"[{GenerateConnection.Name}]{table.Name}.{column.Name}列名包含空格");

			var tableColumn = tableColumns.FirstOrDefault(a => a.Table.ToString() == table.ToString());
			if (tableColumn == null)
			{
				tableColumn = new TableInfo { Table = table };
				tableColumn.Columns.Add(column);

				tableColumns.Add(tableColumn);
			}
			else
			{
				tableColumn.Columns.Add(column);
			}
		}

		/// <summary>
		/// 获取Model.cs文件名称
		/// </summary>
		private string GetModelClassName(TableViewInfo tableView) => GetTableNameForCamel(tableView) + (tableView.IsView ? "View" : null) + Options.ModelSuffix;

		public virtual List<string> GetUsingNamespace(List<ColumnInfo> tableColumn) => new List<string>();

		/// <summary>
		/// 生成Model.cs文件
		/// </summary>
		private void GenerateModel(TableInfo tableColumn)
		{
			var modelName = GetModelClassName(tableColumn.Table);
			string filename = Path.Combine(Options.GetMultipleModelPath(GenerateConnection.Name), modelName + ".cs");
			Console.WriteLine("Generating " + filename + "...");

			using StreamWriter writer = new StreamWriter(File.Create(filename), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader(writer);
			var namespaceStrings = GetUsingNamespace(tableColumn.Columns);
			foreach (var namespaceString in namespaceStrings)
				writer.WriteLine(namespaceString);

			if (tableColumn.Table.IsView)
				tableColumn.Table.Description += string.IsNullOrWhiteSpace(tableColumn.Table.Description) ? "视图仅支持Select操作" : " 视图仅支持Select操作";

			writer.WriteLine(@"using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace {0}
{{
{1}	[CreeperTable(""{3}"")]
	public partial class {2} : {4}
	{{",
		Options.GetModelNamespaceFullName(GenerateConnection.Name),
		GeneratorHelper.WriteSummary(tableColumn.Table.Description, 1),
		modelName,
		GetAttributeTableName(tableColumn.Table),
		nameof(ICreeperModel));

			for (int i = 0; i < tableColumn.Columns.Count; i++)
			{
				ColumnInfo item = tableColumn.Columns[i];

				#region CreeperDbColumn
				var element = new List<string>();
				if (item.IsPrimary) element.Add($"{nameof(CreeperColumnAttribute.IsPrimary)} = true");
				if (item.IsIdentity) element.Add($"{nameof(CreeperColumnAttribute.IsIdentity)} = true");

				//if (item.IsUnique) element.Add($"{nameof(CreeperDbColumnAttribute.IsUnique)} = true");

				#region Ignore
				var schemaName = string.IsNullOrWhiteSpace(tableColumn.Table.SchemaName) ? null : tableColumn.Table.SchemaName + '.';
				var fullFieldName = string.Concat(schemaName, tableColumn.Table.Name, '.', item.Name).ToLower();

				var ignores = new HashSet<string>();
				if (GenerateRules.FieldIgnore.Insert.Contains(fullFieldName)) ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Insert}");
				if (GenerateRules.FieldIgnore.Returning.Contains(fullFieldName)) ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Returning}");
				if (GenerateRules.FieldIgnore.Update.Contains(fullFieldName)) ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Update}");

				if (item.IsReadOnly)
				{
					ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Insert}");
					ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Update}");
				}
				if (item.IsNotSupported)
				{
					ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Insert}");
					ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Update}");
					ignores.Add($"{nameof(IgnoreWhen)}.{IgnoreWhen.Returning}");
				}
				if (ignores.Count > 0) element.Add($"{nameof(CreeperColumnAttribute.IgnoreFlags)} = " + string.Join(" | ", ignores));
				#endregion

				#endregion

				writer.Write(GeneratorHelper.WriteSummary(item.Description, 2));
				if (element.Count > 0) writer.WriteLine(@"{1}{1}[CreeperColumn({0})]", string.Join(", ", element), '\t');
				writer.WriteLine(@"{2}{2}public {0} {1} {{ get; set; }}", item.CsharpTypeName, item.NameUpCase, '\t');
				if (i != tableColumn.Columns.Count - 1)
					writer.WriteLine();
			}
			writer.WriteLine("\t}");
			writer.WriteLine("}");

			writer.Flush();
		}

		/// <summary>
		/// 检查字符串类型并返回
		/// </summary>
		/// <param name="drValue"></param>
		/// <returns></returns>
		protected string CheckString(object drValue)
			=> drValue == null || drValue == DBNull.Value ? null : drValue.ToString();

		/// <summary>
		/// 检查布尔类型并返回
		/// </summary>
		/// <param name="drValue"></param>
		/// <returns></returns>
		protected bool? CheckBoolean(object drValue)
		{
			if (drValue == null || drValue == DBNull.Value) return null;
			switch (drValue.ToString().ToLower())
			{
				case "y":
				case "yes":
					return true;
				case "n":
				case "no":
					return false;
				default:
					return Convert.ToBoolean(drValue);
			}
		}
		protected static string[] SetFileIgnore(string[] ignores, string ignore)
		{
			var hs = new HashSet<string>(ignores) { ignore.ToLower() };
			return hs.ToArray();
		}
		/// <summary>
		/// 检查整型并返回
		/// </summary>
		/// <param name="drValue"></param>
		/// <returns></returns>
		protected int? CheckInt(object drValue)
			=> drValue == null || drValue == DBNull.Value ? null : (int?)Convert.ToInt32(drValue);

		/// <summary>
		/// 创建派生实例
		/// </summary>
		/// <param name="generateRules"></param>
		/// <param name="options"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		public abstract ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection);
	}
}
