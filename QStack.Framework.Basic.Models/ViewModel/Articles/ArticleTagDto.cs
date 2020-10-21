using QStack.Framework.Basic.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    public class ArticleTagDto:BaseDto
    {
        /// <summary>
        /// 文章Id
        /// </summary>
     
        public virtual int ArticleId { get; set; }
        public virtual ArticleDto Article { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
       
        public virtual int TagId { get; set; }

        public virtual TagDto Tag { get; set; }

    }
}
