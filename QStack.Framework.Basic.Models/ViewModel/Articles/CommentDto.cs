using QStack.Framework.Basic.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    public class CommentDto:BaseRootDto
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

        public virtual List<CommentDto> Children { get; set; }
        public virtual int ChildrenCount { get; set; }

    }
}
