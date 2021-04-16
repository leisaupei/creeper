# 使用者必读注意事项
此框架有一些约定，需要使用者遵循，但并不困难
## 数据库代码生成
- 命名空间格式：按照"项目名称.后缀"。
- 实体类格式：``Schema``(如果有)+``TableName``(使用驼峰命名规范, 会将下划线命名规范转为驼峰)+后缀。目录``Model/Build``
- 实体类成员：按照数据库写法仅首字母大写。
- 数据库名称：``Db``+生成语句里面的``name``参数首字母大写，``Main``后缀是主库，``Secondary``后缀的是从库，默认或者忽略的情况是``DbMain``
> Postgresql中名称是``public``的``schema``会被忽略。<br>
> 数据库的生成文件会被覆盖，但可使用``partial class``关键字扩展
## Lamda表达式
查询语句中，如：
``` C#
string stuNo = _dbContext.Select<StudentModel>().Where(a => a.Id == 1).FirstOrDefault(a => a.Stu_no);
```
输出的sql语句为
``` sql
SELECT a."stu_no" FROM "public"."student" WHERE a."id" = 1
```
此处需要遵循一个约定，所有框架使用的lambda表达式中[``ParameterExpression``](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.parameterexpression?view=net-5.0)会跟随语句输出，用于数据库表的别名。
> 查询语句输出都附带别名，泛型主表默认别名是``a``。关联查询别名使用者可自定义

## Where条件
每个``Where``方法之间是用``AND``连接
- 使用``WhereOrStart/WhereOrEnd``情况除外
``` C#
StudentModel stuNo = _dbContext.Select<StudentModel>()
    .Where(a => a.Id == 1).Where(a => a.Name == "小明").FirstOrDefault(a => a.Stu_no);
```
输出的sql语句为
``` sql
SELECT a."stu_no" FROM "public"."student" WHERE a."id" = 1 AND a."name" = '小明'
```