using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Text;

namespace QStack.Framework.Basic.Model.Articles
{
    public class Comment:EntityRoot
    {
        public virtual string NickName { get; set; }

        public virtual string Email { get; set; }

        public virtual string RemoteIp { get; set; }

        public virtual string CommentText { get; set; }

        public virtual int? ArticleId { get; set; }

        public virtual int? ProductId { get; set; }

        public virtual DateTime CreateTime { get; set; } = DateTime.Now;

        public virtual string ReplyTo { get; set; }
        public virtual int? ParentId { get; set; }
        
        [ForeignKey("ParentId")]
        public virtual List<Comment> Children { get; set; }
        [NotMapped]
        public virtual int ChildrenCount { get; set; }
    }
}
