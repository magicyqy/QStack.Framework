using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.ViewComponents
{
    public class ProductCatagoryViewComponent: ViewComponent
    {
        readonly IProductService _productService;
        public ProductCatagoryViewComponent(IProductService productService)
        {
            this._productService = productService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int activeId=0)
        {
            var result = await _productService.GetCatagories();

            var activeItem = result.FirstOrDefault(pc => pc.Id == activeId);
            if(activeItem!=null)
               activeItem.IsActive = true;
            var hotProducts= await _productService.QueryPage<ProductDto>(p => p.State == 1, 1, 5, nameof(ProductDto.ViewCount)+" desc");


            return View(Tuple.Create(result, hotProducts.Data));
        }
    }
}
