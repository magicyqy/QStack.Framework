using QStack.Framework.Core.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.Model.Articles
{
    /// <summary>
    /// 文章内容
    /// </summary>
    public class ArticleContent
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

      
        [DisplayName("正文")]
        /// <summary>
        /// 文章内容
        /// </summary>
        public virtual string Html { get; set; }
    }
}
