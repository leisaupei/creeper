using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Creeper.MySql.Extensions
{
	internal class ConfigureHelper
	{
		public static Action<TTo> Copy<TFrom, TTo>(Action<TFrom> from) where TTo : new()
		{
			if (from is null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			var fromValue = Activator.CreateInstance<TFrom>();
			from.Invoke(fromValue);

			return new Action<TTo>(toValue => Copy(fromValue, toValue));
		}
		public static TTo Copy<TFrom, TTo>(TFrom fromValue) where TTo : new()
		{
			var toValue = Activator.CreateInstance<TTo>();
			Copy(fromValue, toValue);
			return toValue;
		}
		public static void Copy<TFrom, TTo>(TFrom fromValue, TTo toValue) where TTo : new()
		{
			if (fromValue is null)
			{
				throw new ArgumentNullException(nameof(fromValue));
			}

			var toProperties = typeof(TTo).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var fromType = typeof(TFrom);

			foreach (var toProperty in toProperties)
			{
				var fromProperty = fromType.GetProperty(toProperty.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (fromProperty.PropertyType == toProperty.PropertyType)
					fromProperty.SetValue(toValue, fromProperty.GetValue(fromValue));
			}
		}
	}
}
