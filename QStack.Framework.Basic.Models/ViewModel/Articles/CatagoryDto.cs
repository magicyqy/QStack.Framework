using QStack.Framework.Basic.Model.ViewModel;
using System.Collections.Generic;

namespace QStack.Framework.Basic.ViewModel.Articles
{
    public class CatagoryDto:BaseRootDto
    {
        public virtual string Name { get; set; }

        public virtual int? ParentId { get; set; }

        public virtual string ParentName { get; set; }
        public virtual string Code { get; set; }

        public virtual List<CatagoryDto> Children { get; set; }
    }
}