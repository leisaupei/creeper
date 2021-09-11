
# 文档完善中...
# Creeper介绍
[![Creeper](https://img.shields.io/nuget/v/Creeper.svg?label=Creeper&logo=nuget)](https://www.nuget.org/packages/Creeper)
[![Creeper.Access](https://img.shields.io/nuget/v/Creeper.Access.svg?label=Creeper.Access&logo=nuget)](https://www.nuget.org/packages/Creeper.Access)
[![Creeper.MySql](https://img.shields.io/nuget/v/Creeper.MySql.svg?label=Creeper.MySql&logo=nuget)](https://www.nuget.org/packages/Creeper.MySql)
[![Creeper.Oracle](https://img.shields.io/nuget/v/Creeper.Oracle.svg?label=Creeper.Oracle&logo=nuget)](https://www.nuget.org/packages/Creeper.Oracle)
[![Creeper.PostgreSql](https://img.shields.io/nuget/v/Creeper.PostgreSql.svg?label=Creeper.PostgreSql&logo=nuget)](https://www.nuget.org/packages/Creeper.PostgreSql)
[![Creeper.Sqlite](https://img.shields.io/nuget/v/Creeper.Sqlite.svg?label=Creeper.Sqlite&logo=nuget)](https://www.nuget.org/packages/Creeper.Sqlite)
[![Creeper.SqlServer](https://img.shields.io/nuget/v/Creeper.SqlServer.svg?label=Creeper.SqlServer&logo=nuget)](https://www.nuget.org/packages/Creeper.SqlServer)
1. 基于.NetStandard2.1的轻量级ORM框架。
2. 适配多种支持的不同种类数据库开发场景。
3. 自带Entity生成器，一键即可生成数据库模型代码。
4. 支持单条记录的自定义数据库缓存，可自定义缓存策略。
5. 支持一主多从数据库策略，自由切换主从。
6. 所有查询支持异步方法。
---

# 如何开始？

引用对应数据库的Nuget包

## 配置代码生成器
### 参数

- ``-o`` 输出路径
- ``-p`` 项目名称
- ``-s`` 是否在目标目录创建.sln解决方案文件
- ``--b`` 构建连接字符串，params参数，需要写在末尾
  - ``source`` 数据库ip+端口/路径/地址
  - ``pwd`` 数据库密码
  - ``user`` 数据库登录用户
  - ``db`` 数据库名称，不需要指定数据库的可留空。如：access/oracle/sqlite
  - ``name`` 数据库名称，用于生成名称参数，留空默认是@type的值
  - ``type`` 数据库类型，postgresql/mysql/sqlserver/access/sqlite/oracle
  - ``provider`` access需要填写其余忽略
### Build
- 单个
``` 
dotnet bin\Debug\net5.0\Creeper.Generator.dll -o D:\TestCreeper -p TestCreeper -s t --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;type=postgresql
```
- 多个
```
dotnet bin\Debug\net5.0\Creeper.Generator.dll -o D:\TestCreeper -p TestCreeper -s t --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;name=MyPgTest;type=postgresql source=localhost:3306;user=mysql;pwd=123456;db=mysql_db;name=MySqlTest;type=mysql
```
> 更多忽略配置可查看[generator/Creeper.Generator/appsettings.json](https://github.com/leisaupei/creeper/blob/main/generator/Creeper.Generator/appsettings.json)或参阅[代码生成器配置说明](docs/CodeMakerDiscription.md)
## 配置DbContext

### Startup
``` C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddCreeper(options =>
    {
        //PostgreSqlContext可由生成器生成或继承CreeperContextBase实现即可
        options.AddPostgreSqlContext<PostgreSqlContext>(context =>
        {
            context.UseCache<RedisDbCache>();
            //数据库主从策略 从库优先,没有会报错/从库优先,没有会使用主库/只使用主库
            context.UseStrategy(DataBaseTypeStrategy.MainIfSecondaryEmpty);
            //添加主库配置
            context.UseConnectionString("MainDbConnectionString");
            //添加多个从库配置
            context.UseSecondaryConnectionString(new[] { "SecondaryDbConnectionStrings" });
        });
        //此处可添加多种数据库配置, 使用DbContext的对象名称作区分
        //options.AddMySqlContext<MySqlSqlContext>(t =>
        //{
        //    context.UseCache<RedisDbCache>();
        //    //数据库主从策略 从库优先,没有会报错/从库优先,没有会使用主库/只使用主库
        //    context.UseStrategy(DataBaseTypeStrategy.MainIfSecondaryEmpty);
        //    //添加主库配置
        //    context.UseConnectionString("MainDbConnectionString");
        //    //添加多个从库配置
        //    context.UseSecondaryConnectionString(new[] { "SecondaryDbConnectionStrings" });
        //});
    });
}
```
> ``context.UseCache<DbCache>()``参阅[数据库缓存](docs/DbCache.md)

> ``PostgreSqlContext``参阅[DbContext说明](docs/CreeperContext.md)
### Controller或其他注入类
``` C#
public class SomeController : Controller
{
    //如果此处使用多个DbContext, 直接使用DbContext名称或使用以下调用方式也行
    //private readonly Creeper.Driver.ICreeperContext _context;
    //public SomeController(IEnumerable<Creeper.Driver.ICreeperContext> contexts)
    //{
    //    _context = contexts.FirstOrDefault(c => c is PostgreSqlContext);
    //}
    private readonly Creeper.Driver.ICreeperContext _context;
    public SomeController(Creeper.Driver.ICreeperContext context)
    {
        _context = context;
    }

    [HttpGet]
    public DbModel SomeAction()
    {
        return _context.Select<DbModel>().Where(a => a.DbField == SomeValue).ToOne();
    }
}
```

