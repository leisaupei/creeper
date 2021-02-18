using Creeper.Driver;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Common
{
	public class CreeperGenerator : ICreeperGenerator
	{
		private readonly CreeperGeneratorProviderFactory _generatorProviderFactory;
		private readonly IConfiguration _cfg;

		public static string ModelSuffix { get; private set; }
		public static string Namespace { get; private set; }
		public static string DbStandardSuffix { get; private set; }
		private static readonly object _lock = new object();
		public static Action<StreamWriter> WriteAuthorHeader { get; private set; }
		public CreeperGenerator(CreeperGeneratorProviderFactory generatorProviderFactory, IConfiguration cfg)
		{
			_generatorProviderFactory = generatorProviderFactory;
			_cfg = cfg;
			ModelSuffix = _cfg["ModelSuffix"];
			Namespace = _cfg["ModelNamespace"];
			DbStandardSuffix = _cfg["DbStandardSuffix"];
			WriteAuthorHeader = AuthorWriter;
		}

		public void AuthorWriter(StreamWriter writer)
		{
			if (!Convert.ToBoolean(_cfg["AuthorHeader"])) return;
			writer.WriteLine("/* ################################################################################");
			writer.WriteLine(" * # 此文件由生成器创建或覆盖。see: https://github.com/leisaupei/creeper");
			writer.WriteLine(" * ################################################################################");
			writer.WriteLine(" */");
		}
		public void Gen(CreeperGenerateBuilder option)
		{
			var packageReference = option.Connections.GroupBy(a => a.DataBaseKind.ToString()).Select(a => a.Key)
				.Select(a => string.Format("\t\t<PackageReference Include=\"Creeper.{2}\" Version=\"{0}\" />{1}", _cfg["CreeperNugetVersion"], Environment.NewLine, a)).ToList();

			GenerateCsproj(option.OutputPath, option.ProjectName, packageReference);
			var modelPath = CreateDir(option.OutputPath, option.ProjectName);
			if (option.Sln)
				CreateSln(option.OutputPath, option.ProjectName);

			foreach (var connection in option.Connections)
			{
				_generatorProviderFactory[connection.DataBaseKind].ModelGenerator(modelPath, option, connection, option.Connections.Count > 1);
			}

			foreach (var kind in option.Connections.GroupBy(a => a.DataBaseKind).Select(a => a.Key))
			{
				_generatorProviderFactory[kind].GetFinallyGen().Invoke();
			}

		}
		/// <summary>
		/// 
		/// </summary>
		private void GenerateCsproj(string output, string projectName, List<string> packageReference)
		{
			var path = Path.Combine(output, projectName + "." + DbStandardSuffix);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			string csproj = Path.Combine(path, $"{projectName}.{DbStandardSuffix}.csproj");
			if (File.Exists(csproj))
				return;
			using StreamWriter writer = new StreamWriter(File.Create(csproj), Encoding.UTF8);
			writer.WriteLine(@"<Project Sdk=""Microsoft.NET.Sdk"">");
			writer.WriteLine();
			writer.WriteLine("\t<PropertyGroup>");
			writer.WriteLine("\t\t<TargetFramework>netstandard2.1</TargetFramework>");
			writer.WriteLine("\t</PropertyGroup>");
			writer.WriteLine();
			writer.WriteLine("\t<ItemGroup>");
			writer.WriteLine("\t\t<PackageReference Include=\"Creeper\" Version=\"{0}\" />", _cfg["CreeperNugetVersion"]);
			foreach (var item in packageReference)
			{
				writer.WriteLine(item);
			}
			writer.WriteLine("\t</ItemGroup>");
			writer.WriteLine();
			writer.WriteLine("</Project>");
		}
		/// <summary>
		/// 创建sln解决方案文件
		/// </summary>
		private void CreateSln(string output, string projectName)
		{
			if (Directory.GetFiles(output).Any(f => f.Contains(".sln")))
				return;
			string sln_file = Path.Combine(output, $"{projectName}.sln");

			if (File.Exists(sln_file)) return;

			using StreamWriter writer = new StreamWriter(File.Create(sln_file), Encoding.UTF8);
			writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			writer.WriteLine("# Visual Studio 15>");
			writer.WriteLine($"VisualStudioVersion = 15.0.26430.13");

			Guid dbId = Guid.NewGuid();
			writer.WriteLine("Project(\"{0}\") = \"{1}.{3}\", \"{1}.{3}\\{1}.{3}.csproj\", \"{2}\"", Guid.NewGuid(), projectName, dbId, DbStandardSuffix);
			writer.WriteLine($"EndProject");

			writer.WriteLine("Global");
			writer.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			writer.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
			writer.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
			writer.WriteLine("\tEndGlobalSection");

			writer.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			writer.WriteLine($"\t\t{dbId}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Debug|Any CPU.Build.0 = Debug|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Release|Any CPU.ActiveCfg = Release|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Release|Any CPU.Build.0 = Release|Any CPU");
			writer.WriteLine("\tEndGlobalSection");
			writer.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			writer.WriteLine("\t\tHideSolutionNode = FALSE");
			writer.WriteLine("\tEndGlobalSection");
			writer.WriteLine("EndGlobal");

		}
		/// <summary>
		/// 创建目录
		/// </summary>
		private string CreateDir(string output, string projectName)
		{
			var modelPath = Path.Combine(output, projectName + "." + DbStandardSuffix, Namespace, "Build");
			RecreateDir(modelPath);
			return modelPath;
		}

		public static void RecreateDir(string modelPath)
		{
			if (Directory.Exists(modelPath))
				Directory.Delete(modelPath, true);
			Directory.CreateDirectory(modelPath);
		}
	}
}
