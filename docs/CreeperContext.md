# CreeperContext说明
- 生成规则：每种数据库一个``.cs``文件，存放于项目``/Options``目录中。

### 数据库配置
``` C#
public class PostgreSqlContext : CreeperContextBase
{
    public PostgreSqlContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

    //此对象一般只用于postgresql, 其余可忽略
    protected override Action<DbConnection> DbConnectionOptions => connection =>
    {
        var c = (NpgsqlConnection)connection;
        c.TypeMapper.UseNewtonsoftJson();
        c.TypeMapper.UseSystemXmlDocument();
        c.TypeMapper.UseLegacyPostgis();
        c.TypeMapper.MapEnum<Model.EtDataState>("public.et_data_state", PostgreSqlTranslator.Instance);
        c.TypeMapper.MapComposite<Model.Info>("public.info");
    };
}
```