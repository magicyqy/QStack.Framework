using QStack.Framework.SearchEngine.Imps;
using Lucene.Net.Documents;
using QStack.Framework.SearchEngine.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace QStack.Framework.SearchEngine.Imps
{
    /// <summary>
    /// 需要被索引的实体基类
    /// </summary>
    public  class LuceneIndexable<T,Tkey> : ILuceneIndexable
    {
        private readonly T target;
        private readonly Tkey id;
        private readonly LuceneIndexBehavior<T> indexBehavior;
        public LuceneIndexable(T target,
            Tkey tkey, LuceneIndexBehavior<T> indexBehavior)
        {
            this.target = target;
            id = tkey;
            this.indexBehavior= indexBehavior;
        }
        public string IndexId
        {
            get => typeof(T).Name + ":" + id;
            set
            {
            }
        }

        public object TargetObject => target;

        /// <summary>
        /// 转换成Lucene文档
        /// </summary>
        /// <returns></returns>
        public virtual Document ToDocument()
        {
            var doc = new Document();
            var type = typeof(T);
            if (type.Assembly.IsDynamic && type.FullName.Contains("Prox"))
            {
                type = type.BaseType;
            }

            var classProperties = type.GetProperties();
            doc.Add(new StringField("Type", type.AssemblyQualifiedName, Field.Store.YES));
            doc.Add(new StringField("IndexId", IndexId, Field.Store.YES));

            foreach (var propertyInfo in classProperties)
            {
                if (indexBehavior?.IsExcluded(propertyInfo.Name)==true)
                    continue;
                if (indexBehavior?.IsIncluded(propertyInfo.Name) == false)
                    continue;
                var propertyValue = propertyInfo.GetValue(target);
                if (propertyValue == null)
                {
                    continue;
                }

                var value = indexBehavior?.GetIndexValue(propertyInfo.Name, propertyValue) ?? propertyValue.ToString();
                var store =Field.Store.YES;
                if (indexBehavior?.IsStore(propertyInfo.Name, propertyValue) == false)
                    store = Field.Store.NO;
                
                doc.Add(new TextField(propertyInfo.Name, value, store));
                //foreach (var attr in attrs)
                //{
                //    string name = !string.IsNullOrEmpty(attr.Name) ? attr.Name : propertyInfo.Name;
                //    string value = attr.IsHtml ? propertyValue.ToString().RemoveHtmlTag() : propertyValue.ToString();
                //    doc.Add(new TextField(name, value, attr.Store));
                //}
            }

            return doc;
        }

    }
}