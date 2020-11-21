using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model
{
    public class NavigationMenu:EntityRoot
    {
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int FlowNo { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual NavigationMenu Parent { get; set; }

        public virtual List<NavigationMenu> Children { get; set; }
    }
}
