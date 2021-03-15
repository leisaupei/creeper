# 条件表达式
> 此处示例查询语句只是为了描述写法，而并非根据真实业务场景还原。

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
    //爱好
    public string[] Hobby { get; set; }
}
```

## 普通查询
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id == 1 && a.Name == "小明").FirstOrDefault();
```

## Where(string, params object)参数化
Where方法支持string.Format写法
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where("a.name = {0}", "小明").FirstOrDefault();
```
> 以参数化传入，即使数据库语句中需要带有单引号的格式，在这里也不需要添加单引号。<br>
> 错误写法：``Where("a.id = '{0}'", "小明")``

## In/NotIn
使用的是数据库in/not in的语法
``` C#
//WhereNotIn使用方式是一致的，唯一的区别就是输出sql语句的in->not in，这里不再赘述
//可直接使用字符串WhereIn("a.id", _dbContext.Select<StudentModel>().Field(b => b.Id).Where(b => b.Name == "小明"))
StudentModel stu = _dbContext.Select<StudentModel>()
    .WhereIn(a => a.Id, _dbContext.Select<StudentModel>().Field(b => b.Id).Where(b => b.Name == "小明"))
    .FirstOrDefault();
```
输出sql语句为
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" 
WHERE a."id" IN (SELECT b."id" FROM "public"."student" b WHERE b."name" = '小明')
```
> 使用的Field方法是用于返回一列使用，而且会取ParameterExpression中的b作为该sql语句表的别名，所以后面的Where方法需要用b.Name来表示

## Any/NotAny/AnyOrDefault
- ``WhereAny``返回数据表列在传入集合内的行
- ``WhereNotAny``则与``WhereAny``相反
- ``WhereAnyOrDefault``如果传入数据为空或长度为0，则返回空集合。此处是为了少做一层判断而做的语法糖
``` C#
int[] ids = new int[] { 1, 2 }; 
//可直接使用字符串WhereAny("a.id", ids)
List<StudentModel> stu = _dbContext.Select<StudentModel>().WhereAny(a => a.Id, ids).ToList();
```
输出的sql语句为(以Postgresql为例)
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" WHERE id = ANY(ARRAY[1,2])
```

## Exists/NotExists
``` C#
StudentModel stu = _dbContext.Select<StudentModel>()
    .WhereExists(_dbContext.Select<StudentModel>().Where(a => a.Name == "小明"))
    .FirstOrDefault();
```
输出的sql语句为
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" 
WHERE EXISTS (SELECT 1 FROM "public"."student" WHERE a."name" = '小明') LIMIT 1
```

## WhereOrStart/WhereOrEnd
此方法用于把``Where``与``Where``之间的连接变为Or
``` C#
StudentModel stu = _dbContext.Select<StudentModel>()
    .WhereOrStart()
    .Where(a => a.Stu_no == 1)
    .Where(a => a.Name == "小明")
    .WhereOrEnd()
    .FirstOrDefault();
```
输出的sql语句为
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" WHERE a."stu_no" = 1 OR a."name" = '小明'
```
## Where方法lambda表达式的使用支持
### IEnumerable.Contains
此处与[Any](#Any/NotAny/AnyOrDefault)等价，只是将其扩展在lambda表达式中
``` C#
int[] ids = new int[] { 1, 2 }; 
//也可非运算符'!'，!ids.Contains(a.Id)
List<StudentModel> stu = _dbContext.Select<StudentModel>().Where(a => ids.Contains(a.Id)).ToList();
//也可直接把数组放入lambda表达式中
List<StudentModel> stu = _dbContext.Select<StudentModel>().Where(a => new[] { 1, 2 }.Contains(a.Id)).ToList();
//也可反过来使用
List<StudentModel> stu = _dbContext.Select<StudentModel>().Where(a => a.Hobby.Contains("跑步")).ToList();
```

### 字符串模糊查询Contains/StartsWith/EndsWith
- 字符串不仅能使用包含，能使用起始于/结束于方法
- 支持该方法中``StringComparison``枚举参数，但仅限于分为名称包含``IgnoreCase``的成员和不含``IgnoreCase``成员的两种区分形式

以Contains为例
``` C#
List<StudentModel> stu = _dbContext.Select<StudentModel>().Where(a => a.Name.Contains("明")).ToList();
```
输出sql语句为
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" WHERE a."name" LIKE '%明%'
```
使用IgnoreCase忽略大小写，原理只是把LIKE替换为ILIKE其余一致
``` C#
//使用起始于时, 等价为'小%'
List<StudentModel> stu = _dbContext.Select<StudentModel>()
    .Where(a => a.Name.StartsWith("小", StringComparison.OrdinalIgnoreCase)).ToList();
```
输出sql语句为
``` sql
SELECT a."id", a."age", a."name" FROM "public"."student" WHERE a."name" ILIKE '小%'
```
其他方法Contains使用一致不过多赘述

### 委托/方法
``` C#
Func<string> func = () =>
{
    return "小明";
};
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Name == func()).FirstOrDefault();
```

### 三元运算符
``` C#
bool cond = false;
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Name == (cond ? "小明" : "")).FirstOrDefault();
```

### 数组长度
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Hobby.Length == 2).FirstOrDefault();
```

### 数组相等
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Hobby == new[] { "跑步", "游泳" }).FirstOrDefault();
```

### 数组索引值比较
数据库(以Postgresql为例)索引从1开始，但此处写C#语法规则从0开始，框架自动处理+1情况
``` C#
var hobbies = new [] { "跑步", "游泳" }
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Hobby[0] ==  hobbies[0]).FirstOrDefault();
```

### Coalesce语法
假设Name在数据库是可null字段
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => (a.Name ?? "无") ==  "小明").FirstOrDefault();
```

### 其他方法支持
``` C#
//a.Id.Equals(1) -> a.id = 1 
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id.Equals(1)).FirstOrDefault();

// a.Id.ToString() -> CAST(a.id as VARCHAR) = '1'
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id.ToString() == "1").FirstOrDefault();
```