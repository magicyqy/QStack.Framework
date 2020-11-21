using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model.Articles
{
    public class Tag: EntityRoot
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public virtual string Name { get; set; }

    }
}
