namespace Creeper.Generic
{
	internal class UnionModel
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="aliasName">表别名</param>
		/// <param name="table">表名</param>
		/// <param name="expression">on 表达式</param>
		/// <param name="unionType">联表类型</param>
		/// <param name="isReturn">是否返回关联表字段, 如果是请给Field赋值</param>
		public UnionModel(string aliasName, string table, string expression, UnionEnum unionType, bool isReturn)
		{
			AliasName = aliasName;
			Table = table;
			Expression = expression;
			UnionType = unionType;
			IsReturn = isReturn;
		}

		/// <summary> 
		/// 别名
		/// </summary>
		public string AliasName { get; }
		/// <summary>
		/// 标明
		/// </summary>
		public string Table { get; }
		/// <summary>
		/// on表达式
		/// </summary>
		public string Expression { get; }
		/// <summary>
		/// 联表类型
		/// </summary>
		public UnionEnum UnionType { get; }
		/// <summary>
		/// 是否添加返回字段
		/// </summary>
		public bool IsReturn { get; }
		public string UnionTypeString => UnionType.ToString().Replace("_", " ");
		/// <summary>
		/// 字段
		/// </summary>
		public string Fields { get; set; }
	}


}

