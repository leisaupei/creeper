using Candy.Driver.Common;
using Candy.Driver.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Candy.Driver.DbHelper
{
	/// <summary>
	/// 
	/// </summary>
	public class DbContext : ICandyDbContext
	{
		/// <summary>
		/// 实例键值对
		/// </summary>
		private readonly Dictionary<string, List<ICandyDbConnectionOptions>> _executeDict = new Dictionary<string, List<ICandyDbConnectionOptions>>();
		private readonly CandyOptions _candyOptions;

		/// <summary>
		/// 从库后缀
		/// </summary>
		public const string SecondarySuffix = "Secondary";

		public bool SecondaryFirst { get; }

		/// <summary>
		/// 初始化一主多从数据库连接, 从库后缀默认:"Secondary"
		/// </summary>
		/// <param name="CandyOptionsAccessor"></param>
		/// <exception cref="ArgumentOutOfRangeException">options长度为0</exception>
		internal DbContext(IOptions<CandyOptions> CandyOptionsAccessor)
		{
			SecondaryFirst = _candyOptions.SecondaryFirst;
			_candyOptions = CandyOptionsAccessor.Value;
			if ((_candyOptions.DbOptions?.Count() ?? 0) == 0)
				throw new ArgumentOutOfRangeException(nameof(_candyOptions.DbOptions));

			foreach (var option in _candyOptions.DbOptions)
			{
				if (option.Main == null)
					throw new ArgumentNullException(nameof(option.Main), $"Connection string model is null");

				_executeDict[option.Main.DbName] = new List<ICandyDbConnectionOptions> { option.Main };

				if (option.Secondary == null) continue;

				foreach (var item in option.Secondary)
				{
					if (!_executeDict.ContainsKey(item.DbName))
						_executeDict[item.DbName] = new List<ICandyDbConnectionOptions> { item };
					else
						_executeDict[item.DbName].Add(item);
				}
			}
		}

		/// <summary>
		///  获取连接实例
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <returns></returns>
		public ICandyDbExecute GetExecute<TDbName>() where TDbName : struct, ICandyDbName
			=> new DbExecute(GetDbConnectionOptions(typeof(TDbName).Name));

		/// <summary>
		/// 获取连接实例
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ICandyDbExecute GetExecute(string name)
			=> new DbExecute(GetDbConnectionOptions(name));

		/// <summary>
		/// 获取连接配置
		/// </summary>
		/// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
		/// <returns>对应实例</returns>
		private ICandyDbConnectionOptions GetDbConnectionOptions<TDbName>() where TDbName : struct, ICandyDbName
			=> GetDbConnectionOptions(typeof(TDbName).Name);

		/// <summary>
		/// 获取连接配置
		/// </summary>
		/// <param name="name">数据库类型</param>
		/// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
		/// <returns>对应实例</returns>
		private ICandyDbConnectionOptions GetDbConnectionOptions(string name)
		{
			if (_executeDict.ContainsKey(name))
			{
				var execute = _executeDict[name];

				return execute.Count switch
				{
					0 when _candyOptions.UseMainIfSecondaryIsEmpty && name.EndsWith(SecondarySuffix) =>
						GetDbConnectionOptions(name.Replace(SecondarySuffix, string.Empty)),

					1 => execute[0],

					_ => execute[Math.Abs(Guid.NewGuid().GetHashCode() % execute.Count)],
				};
			}
			else if (_candyOptions.UseMainIfSecondaryIsEmpty && name.EndsWith(SecondarySuffix))
				return GetDbConnectionOptions(name.Replace(SecondarySuffix, string.Empty));

			// 从没有从库连接会查主库->如果没有连接会报错
			throw new ArgumentNullException("connectionstring", $"not exist {name} execute");
		}
		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		public void Transaction<TDbName>(Action<ICandyDbExecute> action) where TDbName : struct, ICandyDbName
			=> GetExecute<TDbName>().Transaction(action);

		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask TransactionAsync<TDbName>(Action<ICandyDbExecute> action, CancellationToken cancellationToken = default) where TDbName : struct, ICandyDbName
			=> GetExecute<TDbName>().TransactionAsync(action, cancellationToken);
	}
}
