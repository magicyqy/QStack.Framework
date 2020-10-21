using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace QStack.Framework.Basic.Model.Shop
{
   
    public class ProductCategory : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual string SEOTitle { get; set; }
        public virtual string SEOKeyWord { get; set; }
        public virtual string SEODescription { get; set; }

        public virtual string Code { get; set; }
    }
  

}