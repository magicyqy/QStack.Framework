#if Guid
using System; 
#endif
using QStack.Framework.SearchEngine.Imps;
using Lucene.Net.Documents;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.SearchEngine.Interfaces
{
    /// <summary>
    /// 需要被索引的实体基类
    /// </summary>
    public interface ILuceneIndexable
    {
        /// <summary>
        /// 实际需索引的对象
        /// </summary>
        object TargetObject { get; }
        string IndexId { get; set; }

        /// <summary>
        /// 转换成Lucene文档
        /// </summary>
        /// <returns></returns>
        Document ToDocument();
    }
}