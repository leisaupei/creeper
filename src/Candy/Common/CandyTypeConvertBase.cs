using Candy.DbHelper;
using Candy.Extensions;
using Candy.SqlBuilder;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Candy.Common
{
	public abstract class CandyTypeConvertBase : ICandyTypeConverter
	{
		public abstract T Convert<T>(object value);

		public abstract object Convert(object value, Type convertType);

		public object ConvertDataReader(IDataReader reader, Type convertType)
		{
			bool isValueOrString = convertType == typeof(string) || convertType.IsValueType;
			bool isEnum = convertType.IsEnum;
			object model;
			if (convertType.IsTuple())
			{
				int columnIndex = -1;
				model = GetValueTuple(convertType, reader, ref columnIndex);
			}
			else if (isValueOrString || isEnum)
			{
				model = CheckType(reader[0], convertType);
			}
			else
			{
				model = Activator.CreateInstance(convertType);

				bool isDictionary = convertType.Namespace == "System.Collections.Generic" && convertType.Name.StartsWith("Dictionary`2", StringComparison.Ordinal);//判断是否字典类型

				for (int i = 0; i < reader.FieldCount; i++)
				{
					if (isDictionary)
						model.GetType().GetMethod("Add").Invoke(model, new[] { reader.GetName(i), reader[i].IsNullOrDBNull() ? null : reader[i] });
					else
					{
						if (!reader[i].IsNullOrDBNull())
							SetPropertyValue(convertType, reader[i], model, reader.GetName(i));
					}
				}
			}
			return model;
		}

		public T ConvertDataReader<T>(IDataReader objReader)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// 遍历元组类型
		/// </summary>
		/// <param name="objType"></param>
		/// <param name="dr"></param>
		/// <param name="columnIndex"></param>
		/// <returns></returns>
		protected object GetValueTuple(Type objType, IDataReader dr, ref int columnIndex)
		{
			if (objType.IsTuple())
			{
				FieldInfo[] fs = objType.GetFields();
				Type[] types = new Type[fs.Length];
				object[] parameters = new object[fs.Length];
				for (int i = 0; i < fs.Length; i++)
				{
					types[i] = fs[i].FieldType;
					parameters[i] = GetValueTuple(types[i], dr, ref columnIndex);
				}
				ConstructorInfo info = objType.GetConstructor(types);
				return info.Invoke(parameters);
			}
			// 当元组里面含有实体类
			if (objType.IsClass && !objType.IsSealed && !objType.IsAbstract)
			{
				if (!objType.GetInterfaces().Any(f => f == typeof(ICandyDbModel)))
					throw new NotSupportedException("only the generate models.");

				var model = Activator.CreateInstance(objType);
				var isSet = false; // 这个实体类是否有赋值 没赋值直接返回 default

				var fs = EntityHelper.GetFieldsFromStaticTypeNoSymbol(objType);
				for (int i = 0; i < fs.Length; i++)
				{
					++columnIndex;
					if (!dr[columnIndex].IsNullOrDBNull())
					{
						isSet = true;
						SetPropertyValue(objType, dr[columnIndex], model, fs[i]);
					}
				}
				return isSet ? model : default;
			}
			else
			{
				++columnIndex;
				return CheckType(dr[columnIndex], objType);
			}
		}

		private void SetPropertyValue(Type objType, object value, object model, string fs)
		{
			var p = objType.GetProperty(fs, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null) p.SetValue(model, CheckType(value, p.PropertyType));
		}

		/// <summary>
		/// 对可空类型转化
		/// </summary>
		/// <param name="value"></param>
		/// <param name="valueType"></param>
		/// <returns></returns>
		private object CheckType(object value, Type valueType)
		{
			if (value.IsNullOrDBNull()) return null;
			valueType = valueType.GetOriginalType();

			return Convert(value, valueType);
		}

		public abstract string ConvertSqlToString(ISqlBuilder sqlBuilder);
	}
}
