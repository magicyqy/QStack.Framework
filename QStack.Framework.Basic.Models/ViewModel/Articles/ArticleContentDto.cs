using QStack.Framework.Core.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    /// <summary>
    /// 文章内容
    /// </summary>
    public class ArticleContentDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
       
        public virtual int ArticleId { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public virtual string Html { get; set; }
    }
}
