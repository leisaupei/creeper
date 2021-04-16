# DbOption说明
- 生成规则：每种数据库一个``.cs``文件，存放于项目``/Options``目录中。
- 继承``ICreeperDbName``的构造体是数据库名称类型。

### 数据库名称
此处选用类型作为数据库名称是为了减少误输可能性
```C#
//主库名称
public struct DbMain : ICreeperDbName { }
//从库名称
public struct DbSecondary : ICreeperDbName { }
```
### 数据库配置
``` C#
public class MainPostgreSqlDbOption : BasePostgreSqlDbOption<DbMain, DbSecondary>
{
    public MainPostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) 
        : base(mainConnectionString, secondaryConnectionStrings) { }
    public override DbConnectionOptions Options => new DbConnectionOptions()
    {
        //Mappings配置
        MapAction = conn =>
        {
            //使用NewtonSoft.Json接收数据库类型为json/jsonb的字段
            conn.TypeMapper.UseJsonNetForJtype();
            //使用XmlDocument接收数据库类型为xml的字段
            conn.TypeMapper.UseCustomXml();
            //枚举类型CLR
            conn.TypeMapper.MapEnum<Model.EDataState>("public.e_data_state", PostgreSqlTranslator.Instance);
            //符合类型CLR
            conn.TypeMapper.MapComposite<Model.Info>("public.info");
        }
    };
}
```