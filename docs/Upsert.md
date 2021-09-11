# 增补
示例实体类
``` C#
[CreeperDbTable(@"""public"".""people""", typeof(DbMain))]
public partial class PeopleModel : ICreeperDbModel
{
    //唯一键
    //自增整型主键此处为
    [CreeperDbColumn(Primary = true, Identity = true)] 
    public int Id { get; set; }
    //年龄
    public int Age { get; set; }
    //姓名
    public string Name { get; set; }
}
```
## 特性
- 主键值为default(不赋值或忽略)时, 必定是插入;
- 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
    - 整型自增主键: 根据数据库自增标识
    - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
## Upsert
此处逻辑为：如果Id=1的行存在, 则更新该行; 否则插入一行, Id(自增主键)取决于数据库自增标识。
``` C#
PeopleModel people = new PeopleModel { Id = 1, Age = 25, Name = "小明" };
//返回插入行
PeopleModel result = _context.Upsert(people);
//只返回受影响行数
int affrows = _context.UpsertOnly(people);
```
> 注意：若Table没有主键会抛出异常