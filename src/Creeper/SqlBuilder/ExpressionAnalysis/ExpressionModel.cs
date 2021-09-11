using System.Data.Common;
using System.Collections.Generic;
namespace Creeper.SqlBuilder.ExpressionAnalysis
{
    internal class ExpressionModel
    {
        public ExpressionModel(string cmdText, DbParameter[] parameters, string[] alias)
        {
            CmdText = cmdText;
            Parameters = parameters;
            Alias = alias;
        }
        /// <summary>
        /// 转换成的sql语句
        /// </summary>
        public string CmdText { get; }

        /// <summary>
        /// 参数化列表 union/where
        /// </summary>
        public DbParameter[] Parameters { get; }

        /// <summary>
        /// 数据库别名
        /// </summary>
        public string[] Alias { get; }
    }

}