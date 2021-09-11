namespace Creeper.Generic
{
	internal class JoinInfo
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="aliasName">表别名</param>
		/// <param name="table">表名</param>
		/// <param name="expression">on 表达式</param>
		/// <param name="joinType">联表类型</param>
		/// <param name="isReturn">是否返回关联表字段, 如果是请给Field赋值</param>
		public JoinInfo(string aliasName, string table, string expression, string joinType, bool isReturn)
		{
			AliasName = aliasName;
			Table = table;
			Expression = expression;
			JoinType = joinType;
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
		public string JoinType { get; }
		/// <summary>
		/// 是否添加返回字段
		/// </summary>
		public bool IsReturn { get; }
		/// <summary>
		/// 字段
		/// </summary>
		public string Fields { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1} {2} ON {3}", JoinType, Table, AliasName, Expression);
		}
	}


}

