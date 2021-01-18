using Candy.Common;
using System;
using System.Collections.Generic;

namespace Candy.Model
{
	public class CandyOptions
	{
		/// <summary>
		/// 数据库连接集合
		/// </summary>
		internal List<ICandyDbOption> DbOptions { get; private set; } = new List<ICandyDbOption>();

		/// <summary>
		/// 当没有从库的时候自动使用主库, 默认: true
		/// </summary>
		public bool UseMainIfSecondaryIsEmpty { get; set; } = true;

		/// <summary>
		/// 优先使用从库, 默认: false
		/// </summary>
		public bool SecondaryFirst { get; set; } = false;

		public void AddOption(ICandyDbOption dbOption)
		{
			DbOptions.Add(dbOption);
		}
	}
	
}
