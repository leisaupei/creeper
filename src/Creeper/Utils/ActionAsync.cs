using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public delegate Task ActionAsync<in T>(T obj);
}
