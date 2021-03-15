
# Creeper介绍

[![Creeper.PostgreSql on fuget.org](https://www.fuget.org/packages/Creeper.PostgreSql/badge.svg)](https://www.fuget.org/packages/Creeper.PostgreSql)

1. 基于.NetStandard2.1的RabbitMQ的轻量级ORM框架。
2. 适配多种支持的不同种类数据库开发场景。
3. 自带Entity生成器，一键即可生成数据库模型代码。
4. 支持单条记录的Redis数据缓存，可自定义缓存策略。
5. 支持一主多从数据库策略，自由切换主从。
6. 所有查询支持异步方法。
7. [使用指南](./docs/README.md)。
---

# 如何开始？

引用Nuget包 [Creeper.PostgreSql](https://www.nuget.org/packages/Creeper.PostgreSql/)
> 最终版本会有Creeper.SqlServer与Creeper.MySql

## 配置代码生成器
### 参数

- -o 输出路径
- -p 项目名称
- -s 是否在目标目录创建.sln解决方案文件
- --b 构建连接字符串，params参数，需要写在末尾
  - host 数据库地址
  - port 数据库端口
  - pwd password
  - user username
  - name 数据库名称，用于生成名称参数，留空默认Main
  - type 数据库类型，postgresql/sqlserver/mysql
### Build
- 单个
``` 
dotnet bin\Debug\net5.0\Creeper.Generator.dll -o D:\TestCreeper -p TestCreeper -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql
```
- 多个
```
dotnet bin\Debug\net5.0\Creeper.Generator.dll -o D:\TestCreeper -p TestCreeper -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql host=localhost;port=3306;user=mysql;pwd=123456;db=mysql_db;name=MySqlTest;type=mysql
```
> 更多忽略配置可查看generator/Creeper.Generator/appsettings.json
## 配置DbContext

### Startup.cs
``` C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddCreeperDbContext(options =>
    {
        //默认使用哪个数据库的配置, 某些地方没有标记数据库的情况下会默认使用此配置
        options.DefaultDbOptionName = typeof(DbMain);
        //数据库主从策略 从库优先,没有会报错;从库优先,没有会使用主库;只使用主库
        options.DbTypeStrategy = DataBaseTypeStrategy.SecondaryFirstOfMainIfEmpty;
        //数据库缓存策略, 详细看自定义配置
        options.UseCache<CustomDbCache>();
        //添加数据库配置
        options.AddPostgreSql(new PostgreSqlDbOptions.MainPostgreSqlDbOption("MainDbConnectionString", new[] { "SecondaryDbConnectionStrings" }));
    });
}
```
### Controller或其他注入类
``` C#
public class SomeController : Controller
{
    private readonly Creeper.Driver.ICreeperDbContext _dbContext;
    public SomeController(Creeper.Driver.ICreeperDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task SomeAction()
    {
        _dbContext.Select<DbModel>().Where(a => a.DbField == SomeValue).FirstOrDefault();
    }
}
```

