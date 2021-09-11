using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generic
{
	public class AffrowsResult<T>
	{
		internal AffrowsResult(int affectedRows, T value)
		{
			AffectedRows = affectedRows;
			Value = value;
		}

		/// <summary>
		/// 受影响行数
		/// </summary>
		public int AffectedRows { get; }

		/// <summary>
		/// 执行语句后的结果。当AffectedRows > 0时有值，否则为default; 集合类型则是空集合
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// 隐式转换为T类型对象
		/// </summary>
		/// <param name="result"></param>
		public static implicit operator T(AffrowsResult<T> result) => result.Value;
	}
}
