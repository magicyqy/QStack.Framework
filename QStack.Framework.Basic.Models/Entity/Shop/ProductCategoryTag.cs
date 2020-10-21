using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.Model.Shop
{

    public class ProductCategoryTag : EntityRoot
    {
        
        public virtual int ProductCategoryId { get; set; }
        public virtual int ParentId { get; set; }
        [NotMapped]
        public virtual bool Selected { get; set; }
    }
}
