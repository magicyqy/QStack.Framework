using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.ViewComponents
{
    public class RelatedProductViewComponent:ViewComponent
    {
        readonly IProductService _productService;
        public RelatedProductViewComponent(IProductService productService)
        {
            this._productService = productService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int activeId = 0)
        {
            
            var hotProducts = await _productService.QueryPage<ProductDto>(p => p.State == 1&&p.ProductCategoryId==activeId, 1, 3, nameof(ProductDto.ViewCount) + " desc");


            return View(hotProducts.Data);
        }
    }
}
