# 关联查询
> 此处示例查询语句只是为了描述写法，而并非根据真实业务场景还原。
> 
示例实体类
``` C#
//学生表
[CreeperDbTable(@"""public"".""student""", typeof(DbMain))]
public partial class StudentModel : ICreeperDbModel
{
    //唯一键
    [CreeperDbColumn(Primary = true)]
    public Guid Id { get; set; }
    //学号
    public string Name { get; set; }
    //学号
    public string Stu_no { get; set; }
    //创建时间
    public DateTime Create_time { get; set; }
    // 外键
    public Guid Grade_id { get; set; }
}
//班级表
[CreeperDbTable(@"""public"".""grade""", typeof(DbMain))]
public partial class GradeModel : ICreeperDbModel
{
    [CreeperDbColumn(Primary = true)]
    public Guid Id { get; set; }
    //班级名称
    public string Name { get; set; }
    public Guid Teacher_id { get; set; }
}
//教师表
[CreeperDbTable(@"""public"".""teacher""", typeof(DbMain))]
public partial class TeacherModel : ICreeperDbModel
{
    [CreeperDbColumn(Primary = true)]
    public Guid Id { get; set; }
    //班级名称
    public string Name { get; set; }
}
```

## 单级联表查询
返回班级名称是软件技术的所有学生
- 注意此处``(a,b)``代表了两个表的别名，详见[使用者必读注意事项](./Attention.md) <br>
- 在条件``Where<GradeModel>``需要检索b表用到字段时，需要添加b表泛型，且lambda表达式需要以b为[``ParameterExpression``](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.parameterexpression?view=net-5.0)
- ToList方法只会返回主表的所有列
``` C#
List<StudentModel> stus = _context.Select<StudentModel>()
    //此处可变化连表方式, 支持LeftJoin/RightJoin/LeftOutJoin/RightOutJoin
    .InnerJoin<GradeModel>((a, b) => a.Grade_id == b.Id))
    //WhereAny<T>/WhereExists<T>等都可使用其他表的类型
    .Where<GradeModel>(b => b.Name == "软件技术")
    .ToList();
```

## 多级联表查询
查出名字是小明所在班级教师的名称
- 此处``InnerJoin<T1>``和``InnerJoin<T1, T2>``，前者是关联主表，后者是需要关联次表的情况下使用
- 关联此表的[``ParameterExpression``](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.parameterexpression?view=net-5.0)需要符合别名规范，详见[使用者必读注意事项](./Attention.md)
``` C#
TeacherModel teacher = _context.Select<TeacherModel>()
    .InnerJoin<GradeModel>((a, b) => a.Id == b.Teacher_id))
    .InnerJoin<GradeModel, StudentModel>((b, c) => b.Id = c.Grade_id)
    .Where<StudentModel>(c => c.Name == "小明")
    .FirstOrdefault();
```

## 结果输出
此时如果需要把student表和grade表同时返回，可以使用Union方法，然后在输出的时候也使用Union方式输出
``` C#
List<(StudentModel, GradeModel)> stus = _context.Select<StudentModel>()
    //此处改为UnionInnerJoin方法
    .UnionInnerJoin<GradeModel>((a, b) => a.Grade_id == b.Id))
    .Where<GradeModel>(b => b.Name == "软件技术")
    //此处改为UnionToList输出
    //同时使用泛型接收，此处语法糖等同于使用ToList<(StudentModel, GradeModel)>();
    .UnionToList<GradeModel>();
```
或者
``` C#
(TeacherModel, GradeModel, StudentModel) result = _context.Select<TeacherModel>()
    .UnionInnerJoin<GradeModel>((a, b) => a.Id == b.Teacher_id))
    .UnionInnerJoin<GradeModel, StudentModel>((b, c) => b.Id = c.Grade_id)
    .Where<StudentModel>(c => c.Name == "小明")
    .UnionFirstOrDefault<GradeModel, StudentModel>();
```
> 此处输出结果泛型顺序需要按照Union的顺序 <br>
> 建议需要返回数据的Join才用Union，然后数据只写需要返回的实体即可
