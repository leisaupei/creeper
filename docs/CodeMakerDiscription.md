# 代码生成器配置说明
- `` GenerateRules``中，已定义好的元素一般不需要删除，只需添加排除项即可。
- `` GenerateRules``支持使用'*'通配符。
- 命名规则可根据需求修改。
- 快速实现，可参照[示例appsettings.json](/generator/Creeper.Generator/appsettings.json)。
``` json
{
	"GenerateRules": { //全局配置
		"PostgresqlRules": { //PostgreSql数据库规则
			"Excepts": { //排除
				"Global": { //全局排除策略, 忽略大小写, 支持使用'*'通配符
					"Schemas": [ "pg_toast" ], //排除Schema名称, 此项针对Tables/Views
					"Tables": [ "public.us_rules", "*copy*" ], //排除表格名称
					"Views": [ "public.geography_columns" ], //排除视图名称
					"Composites": [ "public.reclassarg" ] //排除自定义类型名称
				},
				"Customs": { //自定义排除策略，针对对应数据库配置做排除策略
					"Main": { //对应生成字符串name参数，忽略大小写，实体与Global一致
						"Schemas": [],
						"Tables": [],
						"Views": [],
						"Composites": []
					}
				}
			},
			"FieldIgnore": { //字段忽略, 控制返回与插入, 在特性中声明忽略的成员
				"Insert": [ "class.grade.id" ], 
				"Returning": [ "class.grade.id" ]
			}
		}
	},
	"CreeperNugetVersion": "1.0.6", //引用creeper版本号，一般不需要操作，作者会迭代
	"ModelSuffix": "Model", //实体类后缀，生成实体类会加此后缀
	"ModelNamespace": "Model", //生成实体类命名空间后缀，一般是项目名称+DbStandardSuffix+此参数
	"DbStandardSuffix": "Entity", //项目名称区分后缀 项目名称+此后缀
	"AuthorHeader": true //页头可选项
}
```