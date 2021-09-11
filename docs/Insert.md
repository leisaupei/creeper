# 插入
示例实体类
``` C#
[CreeperDbTable(@"""public"".""people""", typeof(DbMain))]
public partial class PeopleModel : ICreeperDbModel
{
    //唯一键
    [CreeperDbColumn(Primary = true)] 
    public Guid Id { get; set; }
    //年龄
    public int Age { get; set; }
    //姓名
    public string Name { get; set; }
}
```

> Guid主键：忽略默认使用``Guid.NewGuid()`` <br>
> 自增主键: 如果是default，则忽略，否则会取该值存入数据库<br>
> 表示创建时间字段：名称为Create_time的非空字段，如果是default会主动赋值为DateTime.Now
## Insert插入后返回实体类
``` C#
PeopleModel info = _context.Insert(new PeopleModel
{
    Age = 25,
    Name = "小明",
});
```
## 插入后返回修改行数
### 插入单条数据
``` C#
int affectedRows = _context.InsertOnly(new PeopleModel
{
    Age = 25,
    Name = "小明",
});
```
### 插入多条数据
此处插入多条只能返回修改行数
``` C#
List<PeopleModel> peoples = new List<PeopleModel>();
peoples.Add(new PeopleModel { Age = 25, Name = "小明" });
peoples.Add(new PeopleModel { Age = 26, Name = "小红" });
int affectedRows = _context.Insert(peoples);
```

## 根据条件插入
当Name为小明的数据不存在时, 插入people
``` C#
PeopleModel people = new PeopleModel { Age = 25, Name = "小明" });
int affectedRows = _context.Insert<PeopleModel>().Set(people)
    .WhereNotExists(_context.Select<PeopleModel>().Where(a => a.Name == "小明"))
    .ToAffrows(out people); //此处out可将原变量修改为update后的值, 也可忽略
```
> Where更多用法详见[查询表达式](./SelectExpression.md)