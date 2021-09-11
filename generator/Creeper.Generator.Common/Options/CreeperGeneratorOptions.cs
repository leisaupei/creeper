using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Options
{
	public class CreeperGeneratorOptions
	{
		internal IList<ICreeperOptionsExtension> Extensions { get; }
		public CreeperGeneratorOptions()
		{
            Extensions = new List<ICreeperOptionsExtension>();
		}

		public void AddExtension(ICreeperOptionsExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			Extensions.Add(extension);
		}

	}
}
