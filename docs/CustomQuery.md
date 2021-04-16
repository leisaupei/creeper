# 自定义查询 
框架提供可直接调用ADO进行查询。
## 切换数据库主从
使用``GetExecute()``方法可以使用其他(主从皆可)数据库配置
``` C#
_dbContext.GetExecute<DbSecondary>().ExecuteNonQuery();
```
或者
``` C#
_dbContext.GetExecute("DbSecondary").ExecuteNonQuery();
```
> 建议使用泛型选择配置，减少单词拼错可能性
## ExecuteNonQuery
``` C#
DbParameter[] ps = new[] {
    new NpgsqlParameter("name", "小明"), 
    new NpgsqlParameter("id", 1)
};
int affrows = _dbContext.ExecuteNonQuery("update student set name = @name where id == @id", CommandType.text, ps);
```
> ``DbParameter``派生类以PostgreSql为例<br>
> 如果不是使用默认数据库配置可使用``_dbContext.GetExecute<DbName>().ExecuteNonQuery()``方法，以下同理

## ExecuteScalar
``` C#
DbParameter[] ps = new[] { new NpgsqlParameter("id", 1) };
string name = _dbContext.ExecuteScalar<string>("select id from student where id = @id", CommandType.text, ps);
```

## ExecuteDataReader
查询包含几个语法糖
### 返回单行ExecuteDataReaderModel
``` C#
DbParameter[] ps = new[] { new NpgsqlParameter("id", 1) };
(int id, string name) stu = _dbContext.ExecuteDataReaderModel<(int, string))>("select id,name from student where id = @id", CommandType.text, ps);
```
> 接收泛型与``FirstOrDefault<T>``使用一致，详见[查询表达式-查询返回类型](./Select.md#查询返回类型)

### 返回多行ExecuteDataReaderList
``` C#
DbParameter[] ps = new[] { new NpgsqlParameter("id", 1) };
(int id, string name) stu = _dbContext.ExecuteDataReaderList<(int, string))>("select id,name from student", CommandType.text, ps);
```
> 接收泛型与``ToList<T>``使用一致，详见[查询表达式-查询返回类型](./Select.md#查询返回类型)

### 使用管道查询ExecuteDataReaderPipe
返回修改行数和返回结果集不能混合使用，如果混合使用会抛出[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception?view=net-5.0)异常
#### 查询
``` C#
ISqlBuilder[] sqls = new ISqlBuilder[] {
    _dbContext.Select<StudentModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).PipeToList(),
    _dbContext.Select<StudentModel>().Where(a => a.Id == StuPeopleId1).PipeFirstOrDefault<(Guid, string)>("id,name"),
};
object[] obj = _dbContext.ExecuteDataReaderPipe(sqls);

List<StudentModel> info1 = (obj[0] as object[]).OfType<StudentModel>().ToList();
(Guid, string) info2 = ((Guid, string))obj[1];
```
> 此处``Pipe``方法也支持泛型<br>
> 返回集合类型是``object[]``，使用强制转化为需要使用的类型即可

#### 更新/插入/删除
``` C#
ISqlBuilder[] sqls = new ISqlBuilder[] {
    _dbContext.Update<StudentModel>().Set(a => a.Name, "小明").Where(a => a.Id == 1).PipeToAffectRows(),
    _dbContext.Delete<StudentModel>().Where(a => a.Id == 1).PipeToAffectRows(),
    _dbContext.Insert<StudentModel>().Set(new StudentModel { Id = 3, Name = "小云" }).PipeToAffectRows(),
};
object[] obj = _dbContext.ExecuteDataReaderPipe(sqls);
var affrows = obj.OfType<int>().Sum();
```
### 自定义ExecuteDataReader
此处是最基础的``DataReader``操作
``` C#
public class Student
{
    public string Name { get; set; }
    public int Id { get; set; }
}
string sql = "select id, name from student";
List<Student> stus = new List<Student>();
_dbContext.ExecuteDataReader(reader => {
    Student stu = new Student();
    stu.Name = reader["name"].ToString();
    stu.Id = Convert.ToInt32(reader["id"]);
    stus.Add(stu);
}, sql);
```