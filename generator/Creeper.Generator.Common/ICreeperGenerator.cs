using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public interface ICreeperGenerator
	{
		void Gen(CreeperGenerateBuilder option);
	}
}
