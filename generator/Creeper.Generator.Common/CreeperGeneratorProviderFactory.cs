using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Common
{
	public class CreeperGeneratorProviderFactory
	{
		private readonly IEnumerable<ICreeperGeneratorProvider> _generators;

		public ICreeperGeneratorProvider this[DataBaseKind kind] => _generators.FirstOrDefault(a => a.DataBaseKind == kind) ?? throw new NotSupportedException($"暂时不支持{kind}构建器");

		public CreeperGeneratorProviderFactory(IEnumerable<ICreeperGeneratorProvider> generators) => _generators = generators;

	}
}