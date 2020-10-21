using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QStack.Framework.SearchEngine.Imps
{
    /// <summary>
    /// 实体类型的指定索引行为，默认类型的所有属性值都将被索引
    /// </summary>
    /// <typeparam name="T">需索引的实体类型</typeparam>
    public class LuceneIndexBehavior<T>
    {

        private List<string> _indexFieldsIncluded;

       

        private List<string> _indexFieldsExcluded;

        private Dictionary<string, Expression<Func<object, string>>> _valueConfigurationExpressions;

        private Dictionary<string, Expression<Func<object, bool>>> _storeConfigurationExpressions;

        private bool isIncludeFirstInvoke = false;

       
        public LuceneIndexBehavior()
        {
            _indexFieldsIncluded = typeof(T).GetProperties().Select(p => p.Name).ToList();
            _indexFieldsExcluded = new List<string>();
            _valueConfigurationExpressions = new Dictionary<string, Expression<Func<object, string>>>();
            _storeConfigurationExpressions = new Dictionary<string, Expression<Func<object, bool>>>();
        }
        /// <summary>
        /// 添加需索引的属性<br/><br/>
        /// notice:此方法一旦调用，将清空默认添加的属性列表
        /// </summary>
        /// <typeparam name="TMember">属性类型</typeparam>
        /// <param name="memberExpression">属性lambda表达式</param>
        /// <returns></returns>
        public LuceneIndexBehavior<T> Include<TMember>(Expression<Func<T,TMember>> memberExpression)
        {
            return Include(new Expression[] { memberExpression });
            //if (memberExpression != null)
            //{
            //    var expression = memberExpression.Body as MemberExpression;
            //    if(!isIncludeFirstInvoke)
            //       _indexFieldsExcluded.Clear();
            //    _indexFieldsExcluded.Add(expression.Member.Name);
            //}
            //return this;
        }
        /// <summary>
        /// 添加需索引的属性<br/><br/>
        /// notice:此方法一旦调用，将清空默认添加的属性列表
        /// </summary>
        /// <param name="memberExpressions">属性lambda表达式</param>
        /// <returns></returns>
        public LuceneIndexBehavior<T> Include(params Expression[] memberExpressions)
        {
            if (!isIncludeFirstInvoke)
                _indexFieldsIncluded.Clear();
            var memberNames = GetMemberNames(memberExpressions);
            _indexFieldsIncluded.AddRange(memberNames);
            
            return this;
        }

        /// <summary>
        /// 添加不参与索引的属性值
        /// </summary>
        /// <param name="memberExpressions">属性lambda表达式</param>
        /// <returns></returns>
        public LuceneIndexBehavior<T> Exclude(params Expression[] memberExpressions)
        {
            var memberNames = GetMemberNames(memberExpressions);


            _indexFieldsExcluded.AddRange(memberNames);
          
            return this;
        }

        private List<string> GetMemberNames(params Expression[] memberExpressions)
        {
            var result = new List<string>();
            if (memberExpressions != null && memberExpressions.Count() > 0)
            {
                foreach (var lambdaExpression in memberExpressions)
                {
                    var lambda = lambdaExpression as LambdaExpression;
                    if (lambda == null)
                        continue;
                    MemberExpression memberExpression = null;
                    if (lambda.Body is UnaryExpression)
                    {
                        UnaryExpression unaryExpression = lambda.Body as UnaryExpression;
                        memberExpression = unaryExpression.Operand as MemberExpression;
                    }
                    if (lambda.Body is MemberExpression)
                        memberExpression = lambda.Body as MemberExpression;
                    if (memberExpression == null)
                        continue;
                    result.Add(memberExpression.Member.Name);
                }
            }

            return result;
        }

        /// <summary>
        /// 配置（索引前）加工处理属性值的表达式
        /// </summary>
        /// <typeparam name="TMember">需处理的属性值类型</typeparam>
        /// <param name="memberExpression">属性lambda表达式</param>
        /// <param name="valueConfigurationExpression">加工处理属性值的lambda表达式</param>
        /// <returns></returns>
        public LuceneIndexBehavior<T> ForMember<TMember>(
            Expression<Func<T, TMember>> memberExpression,
            Expression<Func<TMember,string>> valueConfigurationExpression,
             Expression<Func<TMember, bool>> storeConfigurationExpression)
        {
            var expression = memberExpression.Body as MemberExpression;
            var name = expression.Member.Name;
            if (valueConfigurationExpression != null)
            {
                Expression<Func<object, string>> formatExpression = ReflectorHelper.ConvertFunction<object, TMember, string>(valueConfigurationExpression);
                _valueConfigurationExpressions.Add(name, formatExpression);
            }
            if (storeConfigurationExpression != null)
            {
                Expression<Func<object, bool>> storeExpression = ReflectorHelper.ConvertFunction<object, TMember, bool>(storeConfigurationExpression);
                _storeConfigurationExpressions.Add(name, storeExpression);
            }
            return this;
        }
        /// <summary>
        /// 配置属性值是否被存储的表达式
        /// </summary>
        /// <typeparam name="TMember">需处理的属性值类型</typeparam>
        /// <param name="memberExpression">属性lambda表达式</param>
        /// <param name="storeConfigurationExpression">是否存储属性值的lambda表达式</param>
        /// <returns></returns>
        //public LuceneIndexBehavior<T> ForMember<TMember>(Expression<Func<T, TMember>> memberExpression, Expression<Func<TMember, bool>> storeConfigurationExpression)
        //{
        //    var expression = memberExpression.Body as MemberExpression;
        //    var name = expression.Member.Name;
        //    Expression<Func<object, bool>> innerExpression = ReflectorHelper.ConvertFunction<object, TMember, bool>(storeConfigurationExpression);
        //    _storeConfigurationExpressions.Add(name, innerExpression);

        //    return this;
        //}


        internal bool IsExcluded(string propertyName)
        {
            CheckPropertyExist(propertyName);
            return _indexFieldsExcluded.Contains(propertyName);
        }
        internal bool IsIncluded(string propertyName)
        {
            CheckPropertyExist(propertyName);
            return _indexFieldsIncluded.Contains(propertyName);
        }

        internal string GetIndexValue(string propertyName,object value)
        {
            CheckPropertyExist(propertyName);

            var expression = _valueConfigurationExpressions.GetValueOrDefault(propertyName);
            var result = expression?.Compile().Invoke(value);
            return result ?? value?.ToString();

        }
        internal bool IsStore(string propertyName,object value)
        {
            CheckPropertyExist(propertyName);

            var expression = _storeConfigurationExpressions.GetValueOrDefault(propertyName);
            var result = expression?.Compile().Invoke(value);
            return result ?? false;

        }
        private void CheckPropertyExist(string propertyName)
        {
            if (!typeof(T).GetProperties().Any(p => p.Name.Equals(propertyName)))
                throw new ArgumentException($"\"{typeof(T).FullName}\" can not find the property \"{propertyName}\"");
        }
    }
}
