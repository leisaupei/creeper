using Creeper.DbHelper;
using Creeper.Driver;
using System;
using System.Linq;

namespace Creeper.Generic
{
	/// <summary>
	/// 数据库分表对象
	/// </summary>
	/// <typeparam name="TDbName">数据库名称</typeparam>
	/// <typeparam name="TSplitTable">需要分表的对象</typeparam>
	/// <typeparam name="TReferenceTable">参照表</typeparam>
	/// <typeparam name="TReferenceField">参照表字段</typeparam>
	public class DbTableSplitStrategy<TDbName, TSplitTable, TReferenceTable, TReferenceField> : DbSplitStrategy
		where TDbName : struct, ICreeperDbName
		where TSplitTable : class, ICreeperDbModel, new()
		where TReferenceTable : class, ICreeperDbModel, new()
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="splitType"></param>
		/// <param name="accordingValue"></param>
		/// <param name="suffix">分割后缀 必须有{0}</param>
		public DbTableSplitStrategy(SplitType splitType, object[] accordingValue, string suffix = null) : base(typeof(TReferenceField), typeof(TDbName), typeof(TSplitTable), typeof(TReferenceTable), splitType, accordingValue, suffix)
		{
		}
	}
	public abstract class DbSplitStrategy
	{
		/// <summary>
		/// 是否根据当前表字段分割
		/// </summary>
		internal bool IsSelfTableSplited => SplitTable == ReferenceTable;
		/// <summary>
		/// 当什么时候分割
		/// </summary>
		public SplitType SplitType { get; set; }
		/// <summary>
		/// 参照字段的类型
		/// </summary>
		public virtual Type ReferenceFieldType { get; }
		/// <summary>
		/// 数据库名称
		/// </summary>
		public virtual Type DbNameType { get; }
		/// <summary>
		/// 参照字段的类型
		/// </summary>
		public virtual Type SplitTableType { get; }
		/// <summary>
		/// 数据库名称
		/// </summary>
		public virtual Type ReferenceTableType { get; }
		/// <summary>
		/// 
		/// </summary>
		public string DbName { get; }
		/// <summary>
		/// 分割的表
		/// </summary>
		public string SplitTable { get; }
		/// <summary>
		/// 参照表
		/// </summary>
		public string ReferenceTable { get; }
		/// <summary>
		/// 参照表字段
		/// </summary>
		public string ReferenceField { get; }
		/// <summary>
		/// 根据什么值分割
		/// </summary>
		public object[] AccordingValue { get; } = new object[0];
		/// <summary>
		/// 分割后缀 必须有{0}
		/// </summary>
		public string Suffix { get; } = "_split_{0}";

		private readonly DateTime _defaultDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

		internal DbSplitStrategy()
		{
			DbName = DbNameType.Name;
			SplitTable = EntityHelper.GetDbTable(SplitTableType).TableName;
			ReferenceTable = EntityHelper.GetDbTable(ReferenceTableType).TableName;
			ReferenceField = ReferenceFieldType.Name.ToLower();
		}

		protected DbSplitStrategy(Type referenceFieldType, Type dbNameType, Type splitTableType, Type referenceTableType, SplitType splitType, object[] accordingValue, string suffix = null) : base()
		{
			ReferenceFieldType = referenceFieldType ?? throw new ArgumentNullException(nameof(referenceFieldType));
			DbNameType = dbNameType ?? throw new ArgumentNullException(nameof(dbNameType));
			SplitTableType = splitTableType ?? throw new ArgumentNullException(nameof(splitTableType));
			ReferenceTableType = referenceTableType ?? throw new ArgumentNullException(nameof(referenceTableType));
			AccordingValue = accordingValue ?? new object[0];
			SplitType = splitType;
			if (suffix != null)
				Suffix = suffix.Contains("{0}") ? suffix : throw new ArgumentException("must be contains '{0}'", nameof(suffix));
		}

		public virtual string GetRealTable(object value)
		{
			var suffix = string.Empty;
			switch (SplitType)
			{
				case SplitType.DateTimeEveryYears:
					{
						CheckValue(value, out DateTime dt);
						GetSplitSeed(out int splitYears);
						// 一年分割一次
						if (splitYears == 1)
						{
							suffix = string.Format(Suffix, dt.Year);
							break;
						}
						// 若干年分割一次
						var offsetYears = dt.Year - _defaultDateTime.Year;
						var seed = offsetYears / splitYears;
						var since = _defaultDateTime.Year + seed * splitYears;
						var to = _defaultDateTime.Year + ((seed + 1) * splitYears - 1);
						suffix = string.Format(Suffix, string.Concat(since, "_", to));
					}
					break;
				case SplitType.DateTimeEveryMonths:
					{
						CheckValue(value, out DateTime dt);
						GetSplitSeed(out int splitMonths);
						// 一月分割一次
						if (splitMonths == 1)
						{
							suffix = string.Format(Suffix, DateTimeToMonthString(dt));
							break;
						}
						// 若干月分割一次
						var offsetMonths = (dt.Year - _defaultDateTime.Year) * 12 + dt.Month;
						var seed = offsetMonths / splitMonths;
						var since = _defaultDateTime.AddMonths(seed * splitMonths);
						var to = since.AddMonths(splitMonths - 1);
						suffix = string.Format(Suffix, string.Concat(DateTimeToMonthString(since), "_", DateTimeToMonthString(to)));
					}
					break;
				case SplitType.DateTimeEveryDays:
					{
						CheckValue(value, out DateTime dt);
						GetSplitSeed(out int splitDays);
						// 一天分割一次
						if (splitDays == 1)
						{
							suffix = string.Format(Suffix, DateTimeToDateString(dt));
							break;
						}
						// 若干天分割一次
						var offsetDays = (int)(dt - _defaultDateTime).TotalDays;
						int seed = offsetDays / splitDays;
						var since = _defaultDateTime.AddDays(seed * splitDays);
						var to = since.AddDays(splitDays - 1);
						suffix = string.Format(Suffix, string.Concat(DateTimeToDateString(since), "_", DateTimeToDateString(to)));
					}
					break;
				case SplitType.IntEveryValue:
					{
						CheckValue(value, out int i);
						suffix = string.Format(Suffix, i);
					}
					break;
				case SplitType.EnumEveryValue:
					{
						CheckValue(value, out Enum e);
						suffix = string.Format(Suffix, e);
					}
					break;
				case SplitType.UuidEveryFirstLetter:
					CheckValue(value, out Guid g);
					suffix = string.Format(Suffix, g.ToString().Substring(0, 1));
					break;
				case SplitType.IntEveryValues:
					{
						CheckValue(value, out int i);
						CheckArray(out int[][] map);
						var m = map.Where(f => f.Contains(i)).ToArray();
						if (m.Length != 1) ThrowArrayException();
						suffix = string.Format(Suffix, string.Join("_", m[0]));
					}
					break;
				case SplitType.EnumEveryValues:
					{
						CheckValue(value, out Enum e);
						CheckArray(out Enum[][] map);
						var m = map.Where(f => f.Contains(e)).ToArray();
						if (m.Length != 1) ThrowArrayException();
						suffix = string.Format(Suffix, string.Join<Enum>("_", m[0]));
					}
					break;
			}
			return string.Concat(SplitTable, suffix);
		}

		private static void ThrowArrayException()
		{
			throw new ArgumentException("it must be listed all situation when the enum of SplitWhen end with EveryValues. do not overlap and repeat");
		}

		private void CheckArray<T>(out T[][] map)
		{
			if (AccordingValue.Length == 0
				|| !(AccordingValue[0] is T[])
				|| AccordingValue.Any(f => ((T[])f).Length == 0))
				throw new ArgumentException($"then length of according value({typeof(T).Name}[][]) must be great than 0, the type of element is {typeof(T).Name}[], the length of every element great than 0.", nameof(AccordingValue));
			map = AccordingValue.OfType<T[]>().ToArray();
		}

		private string DateTimeToMonthString(DateTime dt) => dt.ToString("yyyyMM");

		private string DateTimeToDateString(DateTime dt) => dt.ToString("yyyyMMdd");

		private void GetSplitSeed(out int value)
		{
			if (AccordingValue.Length != 1
				|| AccordingValue[0] is not int splitYears
				|| splitYears <= 0)
				throw new ArgumentException("length of according value must be 1, the type is int[], int[0] great than 0.", nameof(AccordingValue));
			value = splitYears;
		}
		private void CheckValue<T>(object value, out T tValue)
			=> _ = value is T dt ? tValue = dt : throw new ArgumentException("value type is not " + ReferenceFieldType.Name, nameof(value));
	}
}
