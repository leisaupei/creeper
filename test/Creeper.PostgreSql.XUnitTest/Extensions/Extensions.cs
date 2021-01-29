using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Meta.xUnitTest.Extensions
{
	static class Extensions
	{
		public static object[] ToObjectArray(this object obj) => obj as object[];

		public static bool IsNullOrEmpty<T>([NotNullWhen(true)]this IEnumerable<T> value) => !value.Any();
		
	}
}
