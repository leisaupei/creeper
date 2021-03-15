# 更新
示例实体类
``` C#
[CreeperDbTable(@"""public"".""people""", typeof(DbMain))]
public partial class PeopleModel : ICreeperDbModel
{
    //唯一键
    //自增整型主键此处为
    //[CreeperDbColumn(PrimaryKey = true, IdentityKey = true, Ignore = IgnoreWhen.Input)]
    [CreeperDbColumn(PrimaryKey = true)] 
    public Guid Id { get; set; }
    //年龄
    public int Age { get; set; }
    //姓名
    public string Name { get; set; }
}
```
## 根据实体类更新
会根据数据库模型以主键为条件更新
``` C#
PeopleModel people = new PeopleModel { Id = 1, Age = 25, Name = "小明" }
//可以传入集合
int affectedRows = _dbContext.Update(people).Set(a => a.Name, "小云")
    .ToAffectedRows(out people);//out可将原变量修改为update后的值, 也可忽略
```
> 注意：以此方式修改，若Table没有主键会抛出异常

## 根据条件更新
``` C#
int affectedRows = _dbContext.Update<PeopleModel>().Set(a => a.Name, "小云").Where(a => a.Id == 1).ToAffectedRows();
```
> Where更多用法详见[查询表达式](./SelectExpression.md)