using QStack.Framework.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace QStack.Framework.Basic.ViewModel.Shop
{
   
    public class ProductImageDto : BaseDto
    {
        
        public virtual int ProductId { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual int UploadFileId { get; set; }
    }
   
}
