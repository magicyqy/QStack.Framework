using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.Model.Articles
{
    public class ArticleTag:IEntityRoot
    {
        /// <summary>
        /// 文章Id
        /// </summary>
        [Key, Column(Order = 1)]
        public virtual int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
        [Key, Column(Order = 2)]
        public virtual int TagId { get; set; }

        public virtual Tag Tag { get; set; }

    }
}
