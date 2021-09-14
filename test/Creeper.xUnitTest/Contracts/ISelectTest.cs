namespace Creeper.xUnitTest.Contracts
{
	public interface ISelectTest
	{
		/// <summary>
		/// 取平均值
		/// </summary>
		void Avg();
		/// <summary>
		/// 返回总条数
		/// </summary>
		void Count();
		/// <summary>
		/// 返回总条数
		/// </summary>
		void CountDistinct();
		/// <summary>
		/// 返回总条数
		/// </summary>
		void Distinct();
		/// <summary>
		/// 返回首条数据, 没有返回default
		/// </summary>
		void FirstOrDefault();
		/// <summary>
		/// 返回首条数据, 没有抛出异常
		/// </summary>
		void Frist();
		/// <summary>
		/// 聚合查询
		/// </summary>
		void GroupBy();

		/// <summary>
		/// 聚合条件
		/// </summary>
		void Having();
		/// <summary>
		/// 前x条数据
		/// </summary>
		void Limit();
		/// <summary>
		/// 取最大值
		/// </summary>
		void Max();
		/// <summary>
		/// 取最小值
		/// </summary>
		void Min();
		/// <summary>
		/// 排序
		/// </summary>
		void OrderBy();
		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="index"></param>
		/// <param name="size"></param>
		void Page(int index, int size);

		/// <summary>
		/// Execute scalar，表达式入参请使用FirstOrDefault方法
		/// </summary>
		void Scalar();
		/// <summary>
		/// 数据库缓存
		/// </summary>
		void SelectByDbCache();
		/// <summary>
		/// 跳过x条
		/// </summary>
		void Skip();
		/// <summary>
		/// 求和
		/// </summary>
		void Sum();
		/// <summary>
		/// 输出集合数据
		/// </summary>
		void ToList();
		/// <summary>
		/// 单sql语句管道模式
		/// </summary>
		void ToPipe();
		/// <summary>
		/// 关联表输出
		/// </summary>
		void ToUnion();
		/// <summary>
		/// 关联查询
		/// </summary>
		void Join();
		/// <summary>
		/// 数据比较
		/// </summary>
		void WhereArrayEqual();
		/// <summary>
		/// 合并 COALESCE/ISNULL数据库方法
		/// </summary>
		void WhereCoalesce();
		/// <summary>
		/// 集合包含
		/// </summary>
		void WhereCollectionContains();
		/// <summary>
		/// exists语法
		/// </summary>
		void WhereExists();
		/// <summary>
		/// 数据库数组字段长度比较
		/// </summary>
		void WhereDbArrayFieldLength();
		/// <summary>
		/// 数据库数组字段索引
		/// </summary>
		void WhereDbArrayFieldIndexParameter();
		/// <summary>
		/// in语法
		/// </summary>
		void WhereIn();
		/// <summary>
		/// 表达式中使用加减乘除操作
		/// </summary>
		void WhereOperationExpression();
		/// <summary>
		/// 以数据库模型中的主键为条件
		/// </summary>
		void WherePk();
		/// <summary>
		/// 测试字符串Contains方法转化为like语句
		/// </summary>
		void WhereStringLike();
		/// <summary>
		/// 数据库字段转化为数据库字符串类型字段
		/// </summary>
		void WhereDbFieldToString();

		/// <summary>
		/// 并表
		/// </summary>
		void Union();
		/// <summary>
		/// 排除
		/// </summary>
		void Except();
		/// <summary>
		/// 相交
		/// </summary>
		void Intersect();
	}
}