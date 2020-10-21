using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace QStack.Framework.Basic.Model.Shop
{
   
    public class ProductImage : EntityRoot
    {
        
        public virtual int ProductId { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual int UploadFileId { get; set; }
    }
   
}
