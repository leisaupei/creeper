# 事务
## 使用方法

- 自主判断事务操作

此处仅展示用法
``` C#
ICreeperDbExecute transDbContext = _dbContext.BeginTransaction();

int affrows1 = transDbContext.Update<StudentModel>()
    .Set(a => a.Name, "小明").Where(a => a.Id == 1).ToAffectedRows();

int affrows2 = transDbContext.Update<StudentModel>()
    .Set(a => a.Name, "小云").Where(a => a.Id == 2).ToAffectedRows();

if(affrows1 > 0)
{
    transDbContext.CommitTransaction();
}
else 
{
    transDbContext.RollbackTransation();
}
```
- ``Transaction``语法糖

``Action``抛出异常会调用``RollbackTransaction``回滚事务，否则调用
``CommitTransation``提交事务。

``` C#
_dbContext.Transaction(transDbContext =>
{
    long total = transDbContext.Select<StudentModel>().Count();
    int affrows = transDbContext.Update<StudentModel>()
        .Set(a => a.Name, "小明")
        .Where(a => a.Id == 1)
        .ToAffectedRows(;
});
```
等价于
``` C#
ICreeperDbExecute transDbContext = _dbContext.BeginTransaction();
try
{
    long total = transDbContext.Select<StudentModel>().Count();
    int affrows = transDbContext.Update<StudentModel>()
        .Set(a => a.Name, "小明")
        .Where(a => a.Id == 1)
        .ToAffectedRows();
    transDbContext.CommitTransaction();
}
catch(Exception e)
{
    transDbContext.RollbackTransaction();
    throw e;
}

```