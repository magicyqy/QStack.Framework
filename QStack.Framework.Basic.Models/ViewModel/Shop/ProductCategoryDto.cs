using QStack.Framework.Core.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QStack.Framework.Basic.ViewModel.Shop
{
   
    public class ProductCategoryDto : BaseDto
    {
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual string SeoTitle { get; set; }
        public virtual string SeoKeyWord { get; set; }
        public virtual string SeoDescription { get; set; }
        public virtual string Code { get; set; }
        public virtual List<ProductCategoryDto> Children { get; set; }

        public virtual bool IsActive { get; set; }
    }
  

}