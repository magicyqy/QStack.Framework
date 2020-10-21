using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QStack.Framework.Basic.Enum
{
    /// <summary>
    /// 文章状态
    /// </summary>
    public  enum ArticleState : byte
    {
        [Display(Name ="草稿")]
        Draft = 0,
        [Display(Name = "已发布")]
        Published = 1

    }
}
