using AspectCore.Extensions.Reflection;
using QStack.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using static QStack.Framework.Core.Entity.Query;

namespace QStack.Framework.Core.Entity
{
    //public enum Operators
    //{
    //    None = 0,
    //    Equal = 1,
    //    GreaterThan = 2,
    //    GreaterThanOrEqual = 3,
    //    LessThan = 4,
    //    LessThanOrEqual = 5,
    //    Contains = 6,
    //    StartWith = 7,
    //    EndWidth = 8,
    //    Range = 9,
    //    In=10
    //}
    //public enum Condition
    //{
    //    OrElse = 1,
    //    AndAlso = 2
    //}

    /// <summary>
    /// 组合过滤的条件属性种类，包含实体属性种类(Reference, ICollection, ValueType)，及上下文环境种类EnvType（userid,roleid）
    /// </summary>
    public enum FieldKind
    {
        Reference=1, //实体导航属性是一对一
        ICollection=2,//实体导航属性是一对多
        ValueType=3, //非导航属性
        EnvType=4
    }


    public class DataPrivilegeRule 
    {
        public string EntityType { get; set; }
        public string PropertyPath { get; private set; }
        public string[] PropertyNames { get; set; }
        public virtual FieldKind FieldKind { get; set; }

        public string PropertyType { get; set; }

       
       public string FilterValue { get; set; }
        public  object[] FilterValues { get; private set; }
        public Operators Operator { get; set; }
        public RuleGroup RuleGroup { get; set; }

        public  LambdaExpression GetExpression(Dictionary<string, object> enviromens = null)
        {
            FilterValues = FilterValue?.Split(',');
            if (enviromens != null&&enviromens.ContainsKey(FilterValue))
            {
                var realValue = enviromens[FilterValue];
                var realValueType = realValue.GetType();
                if (realValueType.GetInterface(nameof(IEnumerable)) != null && typeof(string) != realValueType)
                {
                    realValueType = realValueType.GetGenericArguments()[0];
                    MethodInfo toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
                            .MakeGenericMethod(new System.Type[] { realValueType });
                    FilterValues = toArrayMethod.Invoke(null, new object[] { realValue }) as object[];
                }
                   
                else
                    FilterValues = new object[] { realValue };

                if (Type.GetType(this.PropertyType) != realValueType)
                    throw new NotSupportedException("the propertyType can not compare to the value type ");

            }
           
            PropertyPath = string.Join(".", PropertyNames);
            var enityType = Type.GetType(EntityType);
            LambdaExpression lambdaExpression = null;
            var parameter = Expression.Parameter(enityType, QueryCollection.ParameterSymbol);
            switch (FieldKind)
            {
                case FieldKind.ValueType: //如：m.Id==2 或者m.Name.Contains("a") 或者m.Group.Name="a"
                case FieldKind.Reference: //reference下的属性处理与ValueType是一致的，其实可以忽略掉
                    {
                        var queryCollection = new QueryCollection();
                        var query = new Query { Name = this.PropertyPath, Operator = this.Operator };
                        switch (query.Operator)
                        {
                            case Operators.Range:
                                {
                                    query.ValueMin = this.FilterValues[0];
                                    query.ValueMax = this.FilterValues[1];
                                    break;
                                }
                            case Operators.In:
                                {
                                    query.Value = this.FilterValues;
                                    break;
                                }
                            default:
                                query.Value = this.FilterValues[0];
                                break;

                        }
                        queryCollection.Add(query);
                        var method = typeof(QueryCollection).GetMethod("AsExpression");
                        method = method.MakeGenericMethod(enityType);

                        lambdaExpression = (LambdaExpression)method.GetReflector().Invoke(queryCollection, Query.Condition.AndAlso);
                        break;
                    }
                case FieldKind.ICollection:
                    {
                        if (this.RuleGroup == null)
                            break;
                        var  childLambda = this.RuleGroup.GetExpression();
                       
                        var member= QueryCollection.GetPropertyExpression(parameter, PropertyPath);
                        if(member.Type.GetInterface(nameof(IEnumerable)) != null)
                        {
                            lambdaExpression=Expression.Lambda( 
                                Expression.Call(typeof(Enumerable), "Any", new Type[] { Type.GetType(this.RuleGroup.EntityType)},member, childLambda), 
                                parameter);
                        }
                        break;
                    }
                case FieldKind.EnvType://用户的上下文环境变量，在json字符串阶段需提前用变量值替换掉，替换后，propertyPath其实就是一个值                                  
                    {
                        var propertyType = Type.GetType(this.PropertyType);
                        if (enviromens == null)
                            throw new ArgumentNullException("enviroment values is null");
                        if(!enviromens.ContainsKey(this.PropertyPath))
                            throw new ArgumentNullException("not such enviroment key and value");
                        var environmentValue = enviromens[this.PropertyPath];
                        var left = Expression.Constant(environmentValue);
                        
                        Expression expression = null;
                        switch (Operator)
                        {
                            case Query.Operators.Equal:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                    expression =  Expression.Equal(left,right);
                                    break;
                                }
                            case Query.Operators.GreaterThan:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                    expression = Expression.GreaterThan(left, right);
                                   
                                    break;
                                }
                            case Query.Operators.GreaterThanOrEqual:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                    expression = Expression.GreaterThanOrEqual(left, right);
                                    
                                    break;
                                }
                            case Query.Operators.LessThan:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                    expression = Expression.LessThan(left, right);

                                    break;
                                }
                            case Query.Operators.LessThanOrEqual:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                    expression = Expression.LessThanOrEqual(left, right);

                                    break;
                                }
                            case Query.Operators.Contains:
                                {
                                    
                                    var itemType =propertyType!=typeof(string)&& propertyType.GetInterface(nameof(IEnumerable))!=null ? propertyType.GenericTypeArguments[0] : propertyType;
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(itemType, FilterValues[0]));                                   
                                    expression = Expression.Call(left, "Contains", null,right);                                   
                                    break;
                                }
                            case Query.Operators.In://value必须是集合类型
                                { 
                                    var constantVaule= ReflectorHelper.ValueConvert(propertyType, FilterValues);
                                    var right = Expression.Constant(constantVaule);

                                    expression = Expression.Call(typeof(Enumerable), "Contains", new Type[] { propertyType }, right, left);
                                   
                                    break;
                                }
                            case Query.Operators.StartWith:
                                {

                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));

                                    expression = Expression.Call(left, "StartsWith", null,right);
                                    
                                    break;
                                }
                            case Query.Operators.EndWidth:
                                {
                                    var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));

                                    expression = Expression.Call(left, "EndsWith", null, right);
                                    break;
                                }
                            case Query.Operators.Range:
                                {
                                    Expression minExp = null, maxExp = null;
                                    if (FilterValues.Length>0&&FilterValues[0]!=null)
                                    {
                                        var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[0]));
                                        minExp = Expression.GreaterThanOrEqual(left, right);
                                    }
                                    if (FilterValues.Length > 1 && FilterValues[1] != null)
                                    {
                                        var right = Expression.Constant(ReflectorHelper.ValueConvert(propertyType, FilterValues[1]));
                                        maxExp = Expression.LessThanOrEqual(left, right);
                                    }

                                    if (minExp != null && maxExp != null)
                                    {
                                        expression = Expression.AndAlso(minExp, maxExp);
                                    }
                                    else if (minExp != null)
                                    {
                                        expression = minExp;
                                    }
                                    else if (maxExp != null)
                                    {
                                        expression = maxExp;
                                    }

                                    break;
                                }
                               
                        } 
                        lambdaExpression = Expression.Lambda(expression,parameter);
                        break;
                    }
                default:
                    break;

            }
                
          
            return lambdaExpression;
        }
    }


    public class RuleGroup
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Repository { get; set; }
        public string EntityType { get; set; }
        public Condition  ConditionType{get;set;}

        public List<RuleGroup> NestedRules { get; set; } = new List<RuleGroup>();

        public List<DataPrivilegeRule> Rules { get; set; } = new List<DataPrivilegeRule>();

        public  LambdaExpression GetExpression(Dictionary<string,object> enviromens=null)
        {
            var enityType = Type.GetType(EntityType);
            List<LambdaExpression> lambdaExpressions = new List<LambdaExpression>();

            foreach(var item in NestedRules)
            {
                lambdaExpressions.Add(item.GetExpression(enviromens));
            }

            foreach(var item in Rules)
            {
                lambdaExpressions.Add(item.GetExpression(enviromens));
            }

            Expression expression = null;
            foreach(var item in lambdaExpressions)
            {
                if (expression == null)
                {
                    expression = item.Body;
                    continue;
                }
                  
                expression = ConditionType == Query.Condition.OrElse ? Expression.OrElse(expression, item.Body) : Expression.AndAlso(expression, item.Body);
            }
            var parameter = Expression.Parameter(enityType, QueryCollection.ParameterSymbol);
            expression = new ParameterReplacer(parameter).Replace(expression);
            return Expression.Lambda(expression, parameter);
        }
    }

}
