using Candy.Common;
using Candy.DbHelper;
using Candy.Extensions;
using Candy.SqlBuilder.AnalysisExpression;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace Candy.Model
{
	/// <summary>
	/// 联表实体类
	/// </summary>
	internal class UnionCollection
	{
		public List<UnionModel> List { get; } = new List<UnionModel>();
		private readonly string _mainAlias;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="mainAlias">主表别名</param>
		public UnionCollection(string mainAlias) => _mainAlias = mainAlias;

		public UnionCollection() { }

		/// <summary>
		/// 添加联表
		/// </summary>
		/// <typeparam name="TSource">原表</typeparam>
		/// <typeparam name="TTarget">目标表</typeparam>
		/// <param name="predicate">lambda表达式</param>
		/// <param name="unionType">联表类型</param>
		/// <param name="isReturn">是否需要返回目标表字段</param>
		/// <returns></returns>
		public List<DbParameter> Add<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, UnionEnum unionType, bool isReturn)
			where TSource : ICandyDbModel, new() where TTarget : ICandyDbModel, new()
		{
			var model = SqlExpressionVisitor.Instance.VisitUnion(predicate, List.Select(f => f.AliasName).Append(_mainAlias));
			var info = new UnionModel(model.Alias, EntityHelper.GetDbTable(model.UnionType).TableName, model.SqlText, unionType, isReturn);
			if (info.IsReturn)
				info.Fields = EntityHelper.GetModelTypeFieldsString(model.Alias, model.UnionType);
			List.Add(info);
			return model.Paras;
		}

		/// <summary>
		/// 添加联表
		/// </summary>
		/// <typeparam name="TTarget">目标表</typeparam>
		/// <param name="unionType">联表类型</param>
		/// <param name="aliasName">表别名</param>
		/// <param name="on">on 字符串</param>
		/// <param name="isReturn">是否返回目标表字段</param>
		public void Add<TTarget>(UnionEnum unionType, string aliasName, string on, bool isReturn = false) where TTarget : ICandyDbModel, new()
		{
			var info = new UnionModel(aliasName, EntityHelper.GetDbTable<TTarget>().TableName, on, unionType, isReturn);
			if (info.IsReturn)
				info.Fields = EntityHelper.GetModelTypeFieldsString<TTarget>(aliasName);
			List.Add(info);
		}
	}

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

