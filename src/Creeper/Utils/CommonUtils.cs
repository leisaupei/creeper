using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Utils
{
	internal class CommonUtils
	{

		public static object SetNewGuid(object value)
		{
			if (value is Guid g && (g == Guid.Empty || g == default))
				value = Guid.NewGuid();
			return value;
		}


		public static object SetDefaultDateTime(object value)
		{
			//不可空datetime类型赋值本地当前时间
			if (value is DateTime d && d == default)
				value = DateTime.Now;
			//不可空long类型时间戳赋值本地当前时间毫秒时间戳
			else if (value is long l && l == default)
				value = DateTimeOffset.Now.ToUnixTimeMilliseconds();

			return value;
		}
	}
}
