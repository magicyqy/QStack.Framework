using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace QStack.Framework.Core.Entity
{
    public class Query
    {
        public enum Operators
        {
            None = 0,
            Equal = 1,
            GreaterThan = 2,
            GreaterThanOrEqual = 3,
            LessThan = 4,
            LessThanOrEqual = 5,
            Contains = 6,
            StartWith = 7,
            EndWidth = 8,
            Range = 9,
            In=10
        }
        public enum Condition
        {
            OrElse = 1,
            AndAlso = 2
        }
        public string Name { get; set; }
        public Operators Operator { get; set; }
        public object Value { get; set; }
        public object ValueMin { get; set; }
        public object ValueMax { get; set; }
    }
    public class QueryCollection : Collection<Query>
    {
        public const  string ParameterSymbol="m";
        public Expression<Func<T, bool>> AsExpression<T>(Query.Condition? condition = Query.Condition.OrElse) where T : class
        {
            Type targetType = typeof(T);
            TypeInfo typeInfo = targetType.GetTypeInfo();
            var parameter = Expression.Parameter(targetType, ParameterSymbol);
            Expression expression = null;
            Func<Expression, Expression, Expression> Append = (exp1, exp2) =>
            {
                if (exp1 == null)
                {
                    return exp2;
                }
                return (condition ?? Query.Condition.OrElse) == Query.Condition.OrElse ? Expression.OrElse(exp1, exp2) : Expression.AndAlso(exp1, exp2);
            };
            foreach (var item in this)
            {
                Expression property = GetPropertyExpression(parameter,item.Name);
                if (property == null ||                   
                    (item.Operator != Query.Operators.Range && item.Value == null) ||
                    (item.Operator == Query.Operators.Range && item.ValueMin == null && item.ValueMax == null))
                {
                    continue;
                }
                Type realType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
                if (item.Value != null)
                {
                    if (realType.GetTypeInfo().IsEnum)
                    {
                        Type underlyingType = Enum.GetUnderlyingType(realType); 
                        item.Value= Convert.ChangeType(item.Value, underlyingType);
                        property = Expression.Convert(property, underlyingType);
                    } 
                    else if (item.Value.GetType().IsArray)
                    {
                        var objArray = item.Value as object[]; 
                      
                        item.Value = ReflectorHelper.ValueConvert(realType,objArray);
                    }
                    else
                        item.Value = Convert.ChangeType(item.Value, realType);
                }
                Expression<Func<object>> valueLamba = () => item.Value;
                switch (item.Operator)
                {
                    case Query.Operators.Equal:
                        {
                            expression = Append(expression, Expression.Equal(property,
                                Expression.Convert(valueLamba.Body, property.Type)));
                            break;
                        }
                    case Query.Operators.GreaterThan:
                        {
                            expression = Append(expression, Expression.GreaterThan(property,
                                Expression.Convert(valueLamba.Body, property.Type)));
                            break;
                        }
                    case Query.Operators.GreaterThanOrEqual:
                        {
                            expression = Append(expression, Expression.GreaterThanOrEqual(property,
                                Expression.Convert(valueLamba.Body, property.Type)));
                            break;
                        }
                    case Query.Operators.LessThan:
                        {
                            expression = Append(expression, Expression.LessThan(property,
                                Expression.Convert(valueLamba.Body, property.Type)));
                            break;
                        }
                    case Query.Operators.LessThanOrEqual:
                        {
                            expression = Append(expression, Expression.LessThanOrEqual(property,
                                Expression.Convert(valueLamba.Body, property.Type)));
                            break;
                        }
                    case Query.Operators.Contains:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, property));
                            var contains = Expression.Call(property, "Contains", null,
                                Expression.Convert(valueLamba.Body, property.Type));
                            expression = Append(expression, Expression.AndAlso(nullCheck, contains));
                            break;
                        }
                    case Query.Operators.In://value必须是数组类型
                        {
                            var constantExp = Expression.Constant(item.Value, item.Value.GetType());
                            Type typeIfNullable = Nullable.GetUnderlyingType(property.Type);
                            if (typeIfNullable != null)
                            {
                                property = Expression.Call(property, "GetValueOrDefault", Type.EmptyTypes);
                            }
                            var contains = Expression.Call(typeof(Enumerable), "Contains", new Type[] { realType }, constantExp, property); 
                            //var contains = Expression.Call(constantExp, "Contains", null, property);
                            expression = Append(expression, contains);
                            break;
                        }
                    case Query.Operators.StartWith:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, property));
                            var startsWith = Expression.Call(property, "StartsWith", null,
                                Expression.Convert(valueLamba.Body, property.Type));
                            expression = Append(expression, Expression.AndAlso(nullCheck, startsWith));
                            break;
                        }
                    case Query.Operators.EndWidth:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, property));
                            var endsWith = Expression.Call(property, "EndsWith", null,
                                Expression.Convert(valueLamba.Body, property.Type));
                            expression = Append(expression, Expression.AndAlso(nullCheck, endsWith));
                            break;
                        }
                    case Query.Operators.Range:
                        {
                            Expression minExp = null, maxExp = null;
                            if (item.ValueMin != null)
                            {
                                var minValue = Convert.ChangeType(item.ValueMin, realType);
                                Expression<Func<object>> minValueLamda = () => minValue;
                                minExp = Expression.GreaterThanOrEqual(property, Expression.Convert(minValueLamda.Body, property.Type));
                            }
                            if (item.ValueMax != null)
                            {
                                var maxValue = Convert.ChangeType(item.ValueMax, realType);
                                Expression<Func<object>> maxValueLamda = () => maxValue;
                                maxExp = Expression.LessThanOrEqual(property, Expression.Convert(maxValueLamda.Body, property.Type));
                            }

                            if (minExp != null && maxExp != null)
                            {
                                expression = Append(expression, Expression.AndAlso(minExp, maxExp));
                            }
                            else if (minExp != null)
                            {
                                expression = Append(expression, minExp);
                            }
                            else if (maxExp != null)
                            {
                                expression = Append(expression, maxExp);
                            }

                            break;
                        }
                }
            }
            if (expression == null)
            {
                return null;
            }
            return ((Expression<Func<T, bool>>)Expression.Lambda(expression, parameter));
        }

        public static MemberExpression GetPropertyExpression(ParameterExpression parameter,  string propertyPath)
        {
            try
            {
                var properties = propertyPath.Split(".");
                var property = Expression.Property(parameter, properties[0]);

                if (properties.Length > 1)
                    property = properties.Skip(1).Aggregate(property, (current, propertyName) => Expression.Property(current, propertyName));

                return property;
            }
            catch
            {
                return null;
            }
        }
    }
    public class DataTableOption
    {
        public int Draw { get; set; }
        public ColumnOption[] Columns { get; set; }
        public OrderOption[] Order { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public SearchOption Search { get; set; }
        public string[] IncludePaths { get; set; }
        public Expression<Func<T, bool>> AsExpression<T>() where T : class
        {
            QueryCollection queryCollection = new QueryCollection();
            var properties = typeof(T).GetTypeInfo().GetProperties();
            if (Columns == null)
                return t => true;
            foreach (var item in Columns)
            {
                var p = properties.FirstOrDefault(m => m.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                if (p == null) continue;
                var realType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                string value = null;
                if (item.Search != null && item.Search.Value.IsNotNullAndWhiteSpace())
                {
                    value = item.Search.Value;
                }
                else if (Search != null && Search.Value.IsNotNullAndWhiteSpace() && realType == typeof(string))
                {
                    value = Search.Value;
                }
                if ((item.Search.Opeartor == Query.Operators.Range && item.Search.ValueMin.IsNullOrWhiteSpace() && item.Search.ValueMax.IsNullOrWhiteSpace()) ||
                    (item.Search.Opeartor != Query.Operators.Range && value.IsNullOrWhiteSpace())) continue;

                Query query = new Query();
                query.Name = p.Name;
                try
                {
                    query.Value = ReflectorHelper.ValueConvert(p, value);
                    query.ValueMin = ReflectorHelper.ValueConvert(p, item.Search.ValueMin);
                    query.ValueMax = ReflectorHelper.ValueConvert(p, item.Search.ValueMax);

                    //if (query.ValueMax != null && query.ValueMax is DateTime)
                    //{
                    //    query.ValueMax = ((DateTime)query.ValueMax).AddDays(1).AddMilliseconds(-1);
                    //}
                }
                catch(Exception e)
                {
                    continue;
                }

                query.Operator = item.Search.Opeartor;
                queryCollection.Add(query);
            }
            return queryCollection.AsExpression<T>(Query.Condition.AndAlso);
        }
        public string GetOrderBy<T>()
        {
            if (Order == null || Order.Length == 0)
            {
                return null;
            }
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            List<string> orderBuilder = new List<string>();
            foreach(var order in Order)
            {
                var property = typeInfo.GetProperties().FirstOrDefault(p => p.Name.Equals(order.Column, StringComparison.OrdinalIgnoreCase));
                if (property != null&&order.IaValidDir())
                {
                  
                    orderBuilder.Add(property.Name+" "+order.Dir.Trim());
                }
            }
            //var property = typeInfo.GetProperties().FirstOrDefault(p => p.Name.Equals(Columns[Order[0].Column].Data, StringComparison.OrdinalIgnoreCase));
            //if (property != null)
            //{
            //    return property.Name;
            //}
            return orderBuilder.Count()>0?string.Join(",",orderBuilder.ToArray()):null;
        }
        public bool IsOrderDescending()
        {
            if (Order == null || Order.Length == 0)
            {
                return false;
            }
            return Order[0].Dir.Equals("desc", StringComparison.OrdinalIgnoreCase);
        }
        public void AppendCondition(string property, string value, Query.Operators operators = Query.Operators.Equal)
        {
            property = property.FirstCharToLowerCase();
            foreach (var item in Columns)
            {
                if (item.Name == property)
                {
                    item.SearchAble = true;
                    item.Search = new SearchOption { Opeartor = operators, Value = value };
                    return;
                }
            }
            Columns = Columns.Concat(new ColumnOption[]
            {
                new ColumnOption { Name = property, SearchAble = true, Search = new SearchOption { Opeartor = operators, Value = value } }
            }).ToArray();

        }

       
    }
    public class ColumnOption
    {
      
        public string Name { get; set; }
        public bool SearchAble { get; set; }
        public bool OrderAble { get; set; }
        public SearchOption Search { get; set; } = new SearchOption();
    }
    public class SearchOption
    {
        public string Value { get; set; }
        public string ValueMin { get; set; }
        public string ValueMax { get; set; }
        public bool Regex { get; set; }
        public Query.Operators Opeartor { get; set; }
    }
    public class OrderOption
    {
        public string Column { get; set; }
        //asc|desc
        public string Dir { get; set; }

        public bool IaValidDir()
        {
            if (Dir.IsNullOrWhiteSpace())
                return false;

            return Regex.IsMatch(Dir.Trim(), @"^(asc|desc)$", RegexOptions.IgnoreCase);
        }
    }
}
