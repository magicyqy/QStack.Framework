using QStack.Framework.Basic.Enum;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace QStack.Framework.Basic.Model.Articles
{
    public class Article : EntityBase
    {
       
        [DisplayName("作者")]
        public virtual string Author { get; set; }
        [DisplayName("标题")]
        public virtual  string Title { get; set; }

        [DisplayName("封面")]
        public virtual string CoverUrl { get; set; }

        [DisplayName("摘要")]
        public virtual string Summary { get; set; }

        [DisplayName("来源")]
        public virtual string Source { get; set; }

        [DisplayName("来源地址")]
        public virtual string SourceUrl { get; set; }

        public virtual int? CatagoryId { get; set; }

        [DisplayName("分类")]
        public virtual Catagory Catagory { get; set; }

        [DisplayName("类别")]
        public virtual ArticleType ArticleType { get; set; }

        [DisplayName("浏览数")]
        public virtual int PageViews { get; set; }
        [DisplayName("发布时间")]
        public virtual DateTime? PublishTime { get; set; } 

        [DisplayName("发布状态")]
        public virtual ArticleState State { get; set; } = ArticleState.Draft;

        public virtual bool DisableComment { get; set; }

        /// <summary>
        /// 置顶序号，<=0则不置顶
        /// </summary>
        public virtual int HotTop { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public virtual int ZanNum { get; set; }

        public virtual ArticleContent ArticleContent { get; set; }

        public virtual IList<ArticleTag> ArticleTags { get; set; }

        [NotMapped]
        public virtual IList<Tag> Tags
        {
            get
            {
                return ArticleTags?.Select(u => u.Tag).ToList();
            }
            set
            {
                if (ArticleTags == null)
                    ArticleTags = new List<ArticleTag>();
                foreach (var item in value)
                {
                    if (item == null) continue;
                    ArticleTags.Add(new ArticleTag {
                        TagId = item.Id,
                        ArticleId = Id,
                        Tag =item
                    });
                }
            }
        }

        public virtual string SeoKeyWord { get; set; }
        public virtual string SeoDescription { get; set; }
    }
}
