using Creeper.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.SqlBuilder
{
	public abstract class SqlBuilder<TSQL> : ISqlBuilder
		where TSQL : class, ISqlBuilder
	{
		#region Identity
		protected ICreeperDbExecute DbExecute { get; private set; }
		private readonly ICreeperDbContext _dbContext;

		/// <summary>
		/// 类型转换
		/// </summary>
		private TSQL This => this as TSQL;

		/// <summary>
		/// 主表
		/// </summary>
		protected string MainTable { get; set; }

		/// <summary>
		/// 主表别名, 默认为: "a"
		/// </summary>
		protected string MainAlias { get; set; } = "a";

		/// <summary>
		/// where条件列表
		/// </summary>
		protected List<string> WhereList { get; } = new List<string>();

		/// <summary>
		/// 设置默认数据库
		/// </summary>
		protected string DbName { get; set; }

		/// <summary>
		/// 是否返回默认值, 默认: false
		/// </summary>
		public bool IsReturnDefault { get; set; } = false;

		/// <summary>
		/// 返回实例类型
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// 参数列表
		/// </summary>
		public List<DbParameter> Params { get; } = new List<DbParameter>();

		/// <summary>
		/// 返回类型
		/// </summary>
		public PipeReturnType ReturnType { get; set; }

		/// <summary>
		/// 查询字段
		/// </summary>
		public string Fields { get; set; }

		/// <summary>
		/// where条件数量
		/// </summary>
		public int WhereCount => WhereList.Count;
		#endregion

		#region Constructor
		protected SqlBuilder(ICreeperDbContext dbContext, Type modelType)
		{
			var table = EntityHelper.GetDbTable(modelType);

			if (string.IsNullOrEmpty(MainTable))
				MainTable = table.TableName;

			DbName = table.DbName;

			if (CreeperDbContext.DbTypeStrategy != DataBaseTypeStrategy.OnlyMain)
				DbName += CreeperDbContext.SecondarySuffix;

			DbExecute = dbContext.GetExecute(DbName);
			_dbContext = dbContext;
		}
		protected SqlBuilder(ICreeperDbExecute dbExecute) => DbExecute = dbExecute;
		#endregion

		/// <summary>
		/// 查询指定数据库
		/// </summary>
		/// <typeparam name="TDbName">数据库名称</typeparam>
		/// <returns></returns>
		public TSQL By<TDbName>() where TDbName : struct, ICreeperDbName
		{
			DbExecute.Dispose();
			DbExecute = _dbContext.GetExecute<TDbName>();
			return This;
		}

		/// <summary>
		/// 选择主库还是从库
		/// </summary>
		/// <returns></returns>
		public TSQL By(DataBaseType dataBaseKind)
		{
			DbExecute.Dispose();
			DbExecute = _dbContext.GetExecute(dataBaseKind.ChangeDataBaseKind(DbName));
			return This;
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public TSQL AddParameter(string parameterName, object value)
			=> AddParameter(DbExecute.ConnectionOptions.GetDbParameter(parameterName, value));

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="value"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public TSQL AddParameter(out string parameterName, object value)
		{
			parameterName = EntityHelper.ParamsIndex;
			return AddParameter(parameterName, value);
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TSQL AddParameter(DbParameter ps)
		{
			Params.Add(ps);
			return This;
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TSQL AddParameters(IEnumerable<DbParameter> ps)
		{
			Params.AddRange(ps);
			return This;
		}

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected object ToScalar()
			=> DbExecute.ExecuteScalar(CommandText, CommandType.Text, Params.ToArray());

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<object> ToScalarAsync(CancellationToken cancellationToken)
			=> DbExecute.ExecuteScalarAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected TKey ToScalar<TKey>()
			=> DbExecute.ExecuteScalar<TKey>(CommandText, CommandType.Text, Params.ToArray());

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<TKey> ToScalarAsync<TKey>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteScalarAsync<TKey>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="T">model type</typeparam>
		/// <returns></returns>
		protected List<T> ToList<T>()
			=> DbExecute.ExecuteDataReaderList<T>(CommandText, CommandType.Text, Params.ToArray());

		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="T">model type</typeparam>
		/// <returns></returns>
		protected Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteDataReaderListAsync<T>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

		/// <summary>
		/// 返回一个Model
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected T ToOne<T>()
			=> DbExecute.ExecuteDataReaderModel<T>(CommandText, CommandType.Text, Params.ToArray());

		/// <summary>
		/// 返回一个Model
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected Task<T> ToOneAsync<T>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteDataReaderModelAsync<T>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected int ToRows()
			=> DbExecute.ExecuteNonQuery(CommandText, CommandType.Text, Params.ToArray());


		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected ValueTask<int> ToRowsAsync(CancellationToken cancellationToken)
			=> DbExecute.ExecuteNonQueryAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

		/// <summary>
		/// 输出管道元素
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected TSQL ToPipe<T>(PipeReturnType returnType)
		{
			Type = typeof(T);
			ReturnType = returnType;
			return This;
		}

		/// <summary>
		/// Override ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString() => ToString(null);

		/// <summary>
		/// 输出sql语句
		/// </summary>
		public string CommandText => GetCommandTextString();

		/// <summary>
		/// 调试或输出用
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public string ToString(string field)
		{
			if (!string.IsNullOrEmpty(field)) Fields = field;
			return CreeperDbContext.GetConvert(DbExecute.ConnectionOptions.DataBaseKind).ConvertSqlToString(this);
		}
		/// <summary>
		/// 设置sql语句
		/// </summary>
		/// <returns></returns>
		public abstract string GetCommandTextString();

		#region Implicit
		public static implicit operator string(SqlBuilder<TSQL> builder) => builder.ToString();
		#endregion
	}
}
