using System.Linq.Expressions;

namespace Candy.SqlExpression.XUnitTest
{
    public static class SqlExpressionExtensions
    {
        /// <summary>
        /// 成员表达式改成数据库字段 a.Xxx-> a."xxx"
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public static string ToDatebaseField(this MemberExpression mb)
        {
            return string.Concat(mb.ToString().ToLower().Replace(".", ".\""), "\"");
        }

        /// <summary>
		/// 递归member表达式, 针对optional字段, 从 a.xxx.Value->a.xxx
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static MemberExpression GetOriginalExpression(this MemberExpression node)
        {
            if (node.NodeType == ExpressionType.MemberAccess && node.Expression is MemberExpression me)
                return GetOriginalExpression(me);
            return node;
        }
    }
}