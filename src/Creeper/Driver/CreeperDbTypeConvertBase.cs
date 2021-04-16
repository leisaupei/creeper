using Creeper.DbHelper;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Creeper.Driver
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CreeperDbTypeConvertBase : ICreeperDbTypeConverter
	{
		/// <summary>
		/// 
		/// </summary>
		public abstract DataBaseKind DataBaseKind { get; }

		public virtual string CastStringDbType => "VARCHAR";

		public virtual string StringConnectWord => "||";

		public virtual bool QuotationMarks => true;

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public abstract T ConvertDbData<T>(object value);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		public abstract object ConvertDbData(object value, Type convertType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		public object ConvertDataReader(IDataReader reader, Type convertType)
		{
			bool isValueOrString = convertType == typeof(string) || convertType.IsValueType;
			object model;
			if (convertType.IsTuple())
			{
				int columnIndex = -1;
				model = GetValueTuple(convertType, reader, ref columnIndex);
			}
			else if (isValueOrString || convertType.IsEnum)
			{
				model = CheckType(reader[0], convertType);
			}
			else
			{
				model = Activator.CreateInstance(convertType);
				bool isDictionary = typeof(IDictionary).IsAssignableFrom(convertType); //判断是否字典类型

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

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objReader"></param>
		/// <returns></returns>
		public T ConvertDataReader<T>(IDataReader objReader)
			=> (T)ConvertDataReader(objReader, typeof(T));

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
				if (!objType.GetInterfaces().Any(f => f == typeof(ICreeperDbModel)))
					throw new NotSupportedException("only the generate models.");

				var model = Activator.CreateInstance(objType);
				var isSet = false; // 这个实体类是否有赋值 没赋值直接返回 default

				var fs = EntityHelper.GetFields(objType);
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

		/// <summary>
		/// 反射设置实体类字段值
		/// </summary>
		/// <param name="objType"></param>
		/// <param name="value"></param>
		/// <param name="model"></param>
		/// <param name="fs"></param>
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

			return ConvertDbData(value, valueType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		public abstract string ConvertSqlToString(ISqlBuilder sqlBuilder);
		public abstract DbParameter GetDbParameter(string name, object value);
	}
}
