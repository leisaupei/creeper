using Creeper.Generator.Common.Extensions;
using Creeper.Generic;
using System;
using System.IO;

namespace Creeper.Generator.Common.Options
{
	/// <summary>
	/// 项目构建路径配置参数
	/// </summary>
	public class CreeperGeneratorGlobalOptions
	{
		/// <summary>
		/// model(build)文件目录
		/// </summary>
		public string ModelPath { get; set; }
		/// <summary>
		/// dboptions文件目录
		/// </summary>
		public string DbOptionsPath { get; set; }
		/// <summary>
		/// db层根目录
		/// </summary>
		public string RootPath { get; set; }
		/// <summary>
		/// 基础配置
		/// </summary>
		public CreeperGenerateOption BaseOptions { get; }
		/// <summary>
		/// 项目命名空间
		/// </summary>
		public string ModelNamespace { get; }
		/// <summary>
		/// 类库名称后缀
		/// </summary>
		public string DbStandardSuffix { get; }
		/// <summary>
		/// 类名后缀
		/// </summary>
		public string ModelSuffix { get; }
		/// <summary>
		/// 是否多个数据库生成
		/// </summary>
		public bool Multiple { get; }
		/// <summary>
		/// .csproj文件名称
		/// </summary>
		public string CsProjFileName { get; }
		/// <summary>
		/// .csproj文件名称(包含路径)
		/// </summary>
		public string CsProjFileFullName { get; }
		/// <summary>
		/// .sln名称包含路径
		/// </summary>
		public string SlnFileFullName { get; }
		/// <summary>
		/// 枚举文件_Enum.cs全称(包含文件路径)
		/// </summary>
		public string EnumFileFullName { get; }
		/// <summary>
		/// _Composites.cs全称(包含文件路径)
		/// </summary>
		public string CompositesFileFullName { get; }
		/// <summary>
		/// options目录.cs文件命名空间
		/// </summary>
		public string OptionsNamespace { get; }
		private const string ContextFileName = "Context.cs";
		private const string EnumFileName = "_Enums.cs";
		private const string CompositesFileName = "_Composites.cs";
		private const string DbOptionsDirectory = "Options";

		public CreeperGeneratorGlobalOptions(CreeperGenerateOption baseOptions, string modelNamespace, string dbStandardSuffix, string modelSuffix, bool multiple)
		{
			BaseOptions = baseOptions;
			ModelNamespace = modelNamespace;
			DbStandardSuffix = dbStandardSuffix;
			ModelSuffix = modelSuffix;
			Multiple = multiple;
			RootPath = Path.Combine(BaseOptions.OutputPath, BaseOptions.ProjectName + "." + DbStandardSuffix);
			DbOptionsPath = Path.Combine(RootPath, DbOptionsDirectory);
			ModelPath = Path.Combine(RootPath, ModelNamespace, "Build");
			CsProjFileName = string.Format("{0}.{1}.csproj", BaseOptions.ProjectName, DbStandardSuffix);
			CsProjFileFullName = Path.Combine(RootPath, CsProjFileName);
			SlnFileFullName = Path.Combine(BaseOptions.OutputPath, $"{BaseOptions.ProjectName}.sln");
			EnumFileFullName = Path.Combine(ModelPath, EnumFileName);
			CompositesFileFullName = Path.Combine(ModelPath, CompositesFileName);
			OptionsNamespace = string.Format("{0}.{1}.{2}", BaseOptions.ProjectName, DbStandardSuffix, DbOptionsDirectory);
			CreateDir(RootPath);
			RecreateDir(DbOptionsPath);
			RecreateDir(ModelPath);
		}

		/// <summary>
		/// 获取多库符合类型文件名(包含路径)
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public string GetMultipleCompositesCsFullName(string dbName = null)
		{
			if (!Multiple)
				return CompositesFileFullName;
			else
			{
				CheckDbName(dbName);
				return Path.Combine(Path.GetDirectoryName(CompositesFileFullName), dbName.ToUpperPascal(), CompositesFileName);
			}

		}
		/// <summary>
		/// 获取多库枚举类型文件名(包含路径)
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public string GetMultipleEnumCsFullName(string dbName = null)
		{
			if (!Multiple)
				return EnumFileFullName;
			else
			{
				CheckDbName(dbName);
				return Path.Combine(Path.GetDirectoryName(EnumFileFullName), dbName.ToUpperPascal(), EnumFileName);
			}

		}

		/// <summary>
		/// 获取多库Model路径
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public string GetMultipleModelPath(string dbName = null)
		{
			var path = ModelPath;
			if (Multiple)
			{
				CheckDbName(dbName);
				path = Path.Combine(ModelPath, dbName.ToUpperPascal());
			}
			CreateDir(path);
			return path;
		}

		private static void CheckDbName(string dbName)
		{
			if (string.IsNullOrEmpty(dbName))
				throw new ArgumentNullException(nameof(dbName), "实体类二级目录为空");
		}

		/// <summary>
		/// 获取放在dboptions的枚举/符合类型命名空间前缀
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public string GetMappingNamespaceName(string typeName = null)
		{
			var namespaceName = ModelNamespace;
			if (Multiple)
			{
				CheckDbName(typeName);
				namespaceName += "." + typeName.ToUpperPascal();
			}
			return namespaceName;
		}

		/// <summary>
		/// 获取model的命名空间全程
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public string GetModelNamespaceFullName(string typeName = null)
		{
			var namespaceName = string.Format("{0}.{1}.{2}", BaseOptions.ProjectName, DbStandardSuffix, ModelNamespace);
			if (Multiple)
			{
				CheckDbName(typeName);
				namespaceName += "." + typeName.ToUpperPascal();
			}
			return namespaceName;
		}

		/// <summary>
		/// 删除目录重新创建
		/// </summary>
		/// <param name="path"></param>
		private static void RecreateDir(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			Directory.CreateDirectory(path);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		/// <param name="path"></param>
		private static void CreateDir(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		/// 获取Context文件名称
		/// </summary>
		/// <param name="dataBaseKind"></param>
		/// <returns></returns>
		public string GetContextFileFullName(DataBaseKind dataBaseKind)
		{
			return Path.Combine(DbOptionsPath, dataBaseKind.ToString() + ContextFileName);
		}
	}
}
