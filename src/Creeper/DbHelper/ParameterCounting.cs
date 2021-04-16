using System.Threading;

namespace Creeper.DbHelper
{
	internal static class ParameterCounting
	{
		/// <summary>
		/// 参数计数器
		/// </summary>
		static int _paramsCount = 0;

		private static object _paraLock = new object();
		/// <summary>
		/// 参数后缀
		/// </summary>
		public static string Index
		{
			get
			{
				var i = 0;
				lock (_paraLock)
				{

					if (_paramsCount == int.MaxValue)
						_paramsCount = 0;

					i = _paramsCount++;
				}
				return "p" + i.ToString().PadLeft(6, '0');
			}
		}
	}
}