using QStack.Framework.Basic.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    public  class ArticleDto : BaseDto
    {
        [DisplayName("作者")]
        public virtual string Author { get; set; }
        [DisplayName("标题")]
        public virtual string Title { get; set; }

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
        public virtual string CatagoryName { get; set; }
        [DisplayName("类别")]        
        public virtual ArticleType ArticleType { get; set; }

        [DisplayName("浏览数")]
        public virtual int PageViews { get; set; }
        [DisplayName("发布时间")]
        public virtual DateTime? PublishTime { get; set; } 

        [DisplayName("发布状态")]
        public virtual ArticleState State { get; set; } = ArticleState.Draft;
        [DisplayName("禁用评论")]
        public virtual bool DisableComment { get; set; }
        [DisplayName("置顶")]
        /// <summary>
        /// 置顶序号，<=0则不置顶
        /// </summary>
        public virtual int HotTop { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public virtual int ZanNum { get; set; }
        [DisplayName("正文")]

        public virtual string ArticleContentHtml { get; set; }
        [DisplayName("标签")]
        public virtual IList<TagDto> Tags { get; set; }


        public virtual string SeoKeyWord { get; set; }
        public virtual string SeoDescription { get; set; }

    }
}
