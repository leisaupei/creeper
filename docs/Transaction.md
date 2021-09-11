# 事务
## 使用方法

- 自主判断事务操作

此处仅展示用法
``` C#
ICreeperExecute execute = _context.BeginTransaction();

int affrows1 = execute.Update<StudentModel>()
    .Set(a => a.Name, "小明").Where(a => a.Id == 1).ToAffrows();

int affrows2 = execute.Update<StudentModel>()
    .Set(a => a.Name, "小云").Where(a => a.Id == 2).ToAffrows();

if(affrows1 > 0)
{
    execute.CommitTransaction();
}
else 
{
    execute.RollbackTransation();
}
```
- ``Transaction``语法糖

``Action``抛出异常会调用``RollbackTransaction``回滚事务，否则调用
``CommitTransation``提交事务。

``` C#
_context.Transaction(execute =>
{
    long total = execute.Select<StudentModel>().Count();
    int affrows = execute.Update<StudentModel>()
        .Set(a => a.Name, "小明")
        .Where(a => a.Id == 1)
        .ToAffrows(;
});
```
等价于
``` C#
ICreeperDbExecute execute = _context.BeginTransaction();
try
{
    long total = execute.Select<StudentModel>().Count();
    int affrows = execute.Update<StudentModel>()
        .Set(a => a.Name, "小明")
        .Where(a => a.Id == 1)
        .ToAffrows();
    execute.CommitTransaction();
}
catch(Exception e)
{
    execute.RollbackTransaction();
    throw e;
}

```