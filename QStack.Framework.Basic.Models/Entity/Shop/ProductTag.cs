using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;
namespace QStack.Framework.Basic.Model.Shop
{

    public class ProductTag:EntityRoot
    {
     
        public virtual int ProductId { get; set; }
        public virtual int TagId { get; set; }
        public virtual string Title { get; set; }
    }
}
