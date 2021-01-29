using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public interface ICreeperGeneratorProvider
	{
		DataBaseKind DataBaseKind { get; }

		ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn);

		void ModelGenerator(string modelPath, GenerateOption option, ICreeperDbConnectionOption dbOption, bool folder = false);

		Action GetFinallyGen();
	}
}
