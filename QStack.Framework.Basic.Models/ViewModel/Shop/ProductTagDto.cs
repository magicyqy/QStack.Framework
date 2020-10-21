using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;
namespace QStack.Framework.Basic.ViewModel.Shop
{

    public class ProductTagDto:BaseDto
    {
     
        public virtual int ProductId { get; set; }
        public virtual int TagId { get; set; }
        public virtual string Title { get; set; }
    }
}
