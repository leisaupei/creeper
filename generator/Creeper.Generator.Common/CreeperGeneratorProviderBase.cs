using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.Generator.Common
{
	public abstract class CreeperGeneratorProviderBase : ICreeperGeneratorProvider
	{
		public abstract DataBaseKind DataBaseKind { get; }

		public abstract ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn);
		public abstract Action GetFinallyGen();

		public virtual void ModelGenerator(string modelPath, GenerateOption option, ICreeperDbConnectionOption dbOption, bool folder = false)
		{
			if (folder) modelPath = Path.Combine(modelPath, dbOption.DbName);

			CreeperGenerator.RecreateDir(modelPath);

			var execute = new CreeperDbExecute(dbOption);
			Generate(modelPath, option, dbOption, folder, execute);
		}
		public abstract void Generate(string modelPath, GenerateOption option, ICreeperDbConnectionOption dbOption, bool folder, CreeperDbExecute execute);
	}
}
