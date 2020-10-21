using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QStack.Framework.Util
{
    public static class ReflectorHelper
    {

        /// <summary>
        /// 取出类的属性值
        /// </summary>
        /// <typeparam name="T">类别</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>返回字典</returns>
        public static Dictionary<string, object> GetPropertieValues<T>(T obj)
        {
            Type objType = typeof(T);
            var properties = objType.GetProperties();
            return properties.Where(item => item.GetValue(obj, null) != null).ToDictionary(item => item.Name, item => item.GetValue(obj, null));
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="item">类型实例</param>
        /// <param name="property">属性名称</param>
        /// <returns>属性值</returns>
        public static object GetPropertyValue<T>(T item, string property)
        {
            Type entityType = typeof(T);
            PropertyInfo proper = entityType.GetProperty(property);
            if (proper != null && proper.CanRead)
            {
                return proper.GetValue(item, null);
            }
            else return null;
        }

        public static object GetObjPropertyValue(object item, string property)
        {
            Type entityType = item.GetType();
            PropertyInfo proper = entityType.GetProperty(property);
            if (proper != null && proper.CanRead)
            {
                return proper.GetValue(item, null);
            }
            else return null;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue<T>(T item, string property, object value)
        {
            Type entityType = typeof(T);
            PropertyInfo proper = entityType.GetProperty(property);
            if (proper != null && proper.CanWrite)
            {
                proper.SetValue(item, ValueConvert(proper, value), null);
            }
        }

        public static void SetObjPropertyValue(object obj, string property, object value)
        {
            Type entityType = obj.GetType();
            PropertyInfo proper = entityType.GetProperty(property);
            if (proper != null && proper.CanWrite)
            {
                proper.SetValue(obj, ValueConvert(proper, value), null);
            }
        }


        public static object ValueConvert(PropertyInfo property, object obj)
        {
            return ValueConvert(property.PropertyType, obj);
        }
        public static object ValueConvert(Type type, object obj)
        {
            if (obj == null) return null;
            var realType = Nullable.GetUnderlyingType(type) ?? type;
            if (realType == typeof(DateTimeOffset))
                return new DateTimeOffset((DateTime)Convert.ChangeType(obj, typeof(DateTime)));
            return Convert.ChangeType(obj, realType);
        }

        /// <summary>
        /// 将源数组对象转为目标类型的数组
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="sourceArray">原对象数组</param>
        /// <returns></returns>
        public static Array ValueConvert(Type targetType,object[] sourceArray)
        {
            var realTypeArray = Array.CreateInstance(targetType, sourceArray.Length);
            for (var i = 0; i < sourceArray.Length; i++)
            {
                realTypeArray.SetValue(Convert.ChangeType(sourceArray[i], targetType), i);
            }

            return realTypeArray;
        }

        public static bool IsSimple(this Type type)
        {

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));

        }


        public  static Expression<Func<T1, TResult>> ConvertFunction<T1, T2, TResult>(Expression<Func<T2, TResult>> function)
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new ArgumentException($"\"{typeof(T2).FullName}\" not inherit from \"{typeof(T1).FullName}\"");
            ParameterExpression p = Expression.Parameter(typeof(T1));

            return Expression.Lambda<Func<T1, TResult>>
            (
                Expression.Invoke(function, Expression.Convert(p, typeof(T2))), p
            );
        }
    }
}
