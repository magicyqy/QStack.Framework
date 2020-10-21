using QStack.Framework.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.ViewModel.Shop
{

    /// <summary>
    /// 产品标签是对产品类别的描述
    /// </summary>
    public class ProductCategoryTagDto : BaseDto
    {
        
        public virtual int ProductCategoryId { get; set; }
        public virtual int ParentId { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Selected { get; set; }
    }
}
