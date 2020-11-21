using QStack.Framework.Basic.Model.ViewModel;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.ViewModel
{
    public class NavigationMenuDto:BaseRootDto
    {
        public virtual string Name { get; set; }

        public virtual string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int FlowNo { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual string ParentName { get; set; }

        public virtual List<NavigationMenuDto> Children { get; set; }
    }
}
