# 查询
``` C#
[CreeperDbTable(@"""public"".""student""", typeof(DbMain))]
public partial class StudentModel : ICreeperDbModel
{
    //唯一键
    [CreeperDbColumn(Primary = true)] 
    public Guid Id { get; set; }
    //学号, 唯一键
    public string Stu_no { get; set; }
    //年龄
    public int Age { get; set; }
    //姓名
    public string Name { get; set; }
    //创建时间
    public DateTime Create_time { get; set; }
    //外键, 班级id
    public Guid Grade_id { get; set; }
    //外键, 老师id
    public Guid Teacher_id { get; set; }
}
```
## 主从数据库切换By
根据场景选择查询的主/从数据库，优先使用全局数据库主从策略，详见[配置DbContext-Startup](./../README.md#Startup)
``` C#
public enum DataBaseType
{
	/// <summary>
	/// 主库
	/// </summary>
	Main = 1,
	/// <summary>
	/// 从库
	/// </summary>
	Secondary
}
```
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id == 1).By(DataBaseType.Main).FirstOrDefault();
``` 
或者使用数据库名称的Secondary库
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id == 1).By<DbSecondary>().FirstOrDefault();
```
> 数据库名称详见生成Entity层``Options/DbOptions.cs``，详见[DbOptions说明](./DbOptions.md)
## 查询一条数据FirstOrDefault
查询people表返回Id为1的第一条记录
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().Where(a => a.Id == 1).FirstOrDefault();
```
> ``Where``更多用法详见[查询表达式](./SelectExpression.md)

## 查询多条数据ToList
查询people表返回Name为小明的多条记录
``` C#
List<StudentModel> stus = _dbContext.Select<StudentModel>().Where(a =>a.Name == "小明").ToList();
```

## 返回指定列FristOrDefault/ToList
### 返回单列
返回id为1的学号
``` C#
string stuNo = _dbContext.Select<StudentModel>().Where(a => a.Id == 1).FirstOrDefault(a => a.Stu_no);
```

### 返回多列
``` C#
List<(string stuNo, string name)> stu = _dbContext.Select<StudentModel>().ToList<(string, string)>(@"a.""stu_no"", a.""name""");
//如果是主表的字段, 也可以这么写
List<(string stuNo, string name)> stu = _dbContext.Select<StudentModel>().ToList<(string, string)>(a => new { a.Stu_no, a.Name });
```

## 查询返回类型
> 使用的泛型FristOrDefault也同样支持
- #### 元组接收
``` C#
List<(string stuNo, string name)> stu = _dbContext.Select<StudentModel>().ToList<(string, string)>(@"a.""stu_no"", a.""name""");
```
> ToList参数是SQL语句中的返回字段
- #### class接收

``` C#
public class StuInfoModel
{
    public string Stu_no { get; set; }
    public string Name { get; set; }
}
List<StuInfoModel> stus = _dbContext.Select<StudentModel>().ToList<StuInfoModel>(@"a.""stu_no"", a.""name""");
```
> 定义的变量自动匹配，名称需要与返回的列名一致，大小写忽略
- #### IDictionary接收
``` C#
List<Hashtable> stus = _dbContext.Select<StudentModel>().ToList<Hashtable>(@"a.""stu_no"", a.""name""");
```
或者
``` C#
List<Dictonary<string, object>> stus = _dbContext.Select<StudentModel>()
    .ToList<Dictonary<string, object>>(@"a.""stu_no"", a.""name""");
```

## 关联查询UnionToList/UnionFirstOrDefault
> 用法见[关联查询](./JoinQuery.md)
## 自定义查询PipeToList/PipeFirstOrDefault
> 用法见[自定义查询](./CustomQuery.md)
## 汇总Avg/Min/Max/Sum/Count
- ``Avg``/``Min``/``Max``/``Sum``还有``defaultValue``参数，数据为``null``时取该值，也可使用可空类型接收``<decimal?>``
- 可使用COALESCE语法规则

学生总数
``` C#
long count = _dbContext.Select<StudentModel>().Count();
```
学生平均年龄
``` C#
decimal avgAge = _dbContext.Select<StudentModel>().Avg<decimal>(a => a.Age);

//使用Coalesce语法, 假设age是int?类型, 数据库是可空字段
//SELECT AVG(COALESCE(a."age", 0)) FROM "public"."student"
decimal avgAge = _dbContext.Select<StudentModel>().Avg<decimal>(a => a.Age ?? 0);

//使用defaultVlaue
//SELECT COALESCE(AVG(a."age"), 0) FROM "public"."student", 与上面COALESCE语法不一致, 也可同时使用
decimal avgAge = _dbContext.Select<StudentModel>().Avg<decimal>(a => a.Age, 0);
```

> 除了``Count``以外其他字段直接用lambda表达式selector即可
> 
> 注意区分``Coalesce``语法与``defaultValue``语法
## 排序OrderBy(升序)/OrderByDescending(降序)
多个``OrderBy``/``OrderByDescending``方法以','分隔输出
### 单级排序
查询people表根据Create_time降序后返回第1条记录
``` C#
StudentModel stu = _dbContext.Select<StudentModel>().OrderByDescending(a => a.Create_time).FirstOrDefault();
```
### 多级排序
查询people表根据Create_time降序后再根据Stu_no升序返回第1条记录
``` C#
//order by a."create_time" desc, order by a."stu_no" asc
//OrderBy方法也可使用字符串传入
//OrderByDescendingNullsLast/OrderByNullsLast是否将空数据放在末尾, 开情况选用
StudentModel stu = _dbContext.Select<StudentModel>().OrderByDescending(a => a.Create_time)
    .OrderBy(a => a.Stu_no).FirstOrDefault();
```

## 分组查询GroupBy
多个``GroupBy``方法以英文','分隔输出
### 单条件分组
查出各班级人数
``` C#
List<(Guid gradeId, long count)> stat = _dbContext.Select<StudentModel>().GroupBy(a => a.Grade_id)
    .ToList<(Guid, long)>(@"a.""grade_id"", COUNT(1)");
```
### 多条件分组
返回各班级年龄人数
``` C#
List<(Guid gradeId, int age, long count)> stat = _dbContext.Select<StudentModel>().GroupBy(a => a.Age)
    .GroupBy(a => a.Grade_id).ToList<(Guid, int, long)>(@"a.""grade_id"", a.""age"", COUNT(1)");
```
> 也可使用实体类接收返回列
## Having分组条件过滤
返回人数大于10人的各班级年龄人数
``` C#
List<(Guid gradeId, int age, long count)> stat = _dbContext.Select<StudentModel>().GroupBy(a => a.Age)
    .GroupBy(a => a.Grade_id).Having("COUNT(1) > 10")
    .ToList<(Guid, int, long)>(@"a.""grade_id"", a.""age"", COUNT(1)");
```
> 因为``Having``条件一般是自定义的聚合条件，所以目前仅支持字符串

## 分页查询Take/Skip/Page
使用的是Linq的语法，结合场景增加Page方法
- ``Skip(int)``等同于``Offset``
- ``Take(int)``等同于``Limit``
- ``Page(int,int)``就是``Offet``与``Limit``结合，实现分页效果

返回页码是1, 分页大小是10的场景：
``` C#
List<StudentModel> stus = _dbContext.Select<StudentModel>().OrderBy(a => a.Stu_no).Page(1, 10).ToList();
``` 
等同于
``` C#
List<StudentModel> stus = _dbContext.Select<StudentModel>().OrderBy(a => a.Stu_no).Skip(10).Take(10).ToList();
```

## 联合查询Union/UnionAll/Expect/Intersect
- ``Union``/``UnionAll``两个结果的并集，``UnionAll``不会忽略重复结果
- ``Expect``两个结果的差
- ``Intersect``两个结果的交集

找出学号前5人和后5人
``` C#
List<StudentModel> stus = _dbContext.Select<StudentModel>().OrderBy(a => a.Stu_no).Take(5)
    .Union(_dbContext.Select<StudentModel>().OrderByDescending(a => a.Stu_no).Take(5)).ToList();
```
> 其他方法的写法都与这个一致，可按照实际场景选择合适的方法 

## 随机抽样TableSampleSystem
- 大量数据中随机抽取样本数据，性能比``ORDER BY RANDOM()``高。

随机查出5名学生，如:
``` C#
//TableSampleSystem传入参数是要采样的分数，表示为一个0到100之间的百分数
List<StudentModel> stus = _dbContext.Select<StudentModel>().TableSampleSystem(1.1).Take(5).ToList();
``` 
> 此方法目前只支持PostgreSql数据库

## 组内去重DistinctOn
``` C#
List<StudentModel> stus = _dbContext.Select<StudentModel>().DistinctOn(a => a.Name).OrderBy(a => a.Stu_no).ToList();
```
> 此方法目前只支持PostgreSql数据库