using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    public class TagDto
    {
        [DisplayName("Id")]
        public virtual int Id { get; set; }
        [DisplayName("标签名称")]
        /// <summary>
        /// 标签名称
        /// </summary>
        public virtual string Name { get; set; }

    
    }
}
