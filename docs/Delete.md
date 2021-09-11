# 删除
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

## 根据实体类删除数据
根据数据库模型以主键为条件更新
### 删除单条数据Delete
``` C#
PeopleModel people = new PeopleModel { Id = 1, Age = 25, Name = "小明" }
int affectedRows = _context.Delete(people);
```
### 删除多条数据DeleteMany
``` C#
List<PeopleModel> peoples = new List<PeopleModel>();
peoples.Add(new PeopleModel { Id = 1, Age = 25, Name = "小明" });
peoples.Add(new PeopleModel { Id = 2, Age = 26, Name = "小红" });
int affectedRows = _context.Delete(peoples);
```
## 根据条件删除
``` C#
int affectedRows = _context.Delete<PeopleModel>().Where(a => a.Id == 1).ToAffrows();
```
> Where更多用法详见[查询表达式](./SelectExpression.md)