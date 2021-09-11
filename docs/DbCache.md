# 自定义数据库缓存
框架可自定义数据库缓存，目前支``FirstOrDefault``/``Min``/``Count``等返回单行/单行单列的方法(暂不支持``ToList``查询)。具体可查看[查询](./Select.md)
- 快速使用，无需大规模封装存取
- 可使不同业务使用不同的过期策略

## 使用方法
实现``Creeper.Driver.ICreeperCache``
- 此处需要用户判断``object``类型参数，可能需要单独做序列化操作。
- 在配置``Context``时注入``.UseCache<T>();``。详见[配置CreeperContext-Startup](./../README.md#Startup)
- 此处框架使用``AddSingleton``注入，建议使用[``IDisposable``](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-5.0)作资源管理。
- 快速实现，可参照[示例DbCache](/test/Creeper.PostgreSql.XUnitTest/Extensions/CustomDbCache.cs)。

查询并设置1分钟缓存
``` C#
StudentModel stu = _context.Select<StudentModel>().Where(a => a.Id == 1).ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
```