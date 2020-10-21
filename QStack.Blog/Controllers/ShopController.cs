using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Shop;

namespace QStack.Blog.Controllers
{
    public class ShopController : Controller
    {
        readonly IProductService _productService;
        public ShopController(IProductService productService)
        {
            this._productService = productService;
        }
        //[Route("/[controller]/{id:int?}")]
        public async Task<IActionResult> Index(int page=1,int pageSize=9)
        {
            var pageModel =await _productService.QueryPage<ProductDto>(p=>p.State==1, page, pageSize, nameof(ProductDto.PublishDate)+" desc ");
            
            
            return View(pageModel);
        }
        [Route("/[controller]/pc/{id:int?}")]
        public async Task<IActionResult> QueryByCatagory(int id=0,int page = 1, int pageSize = 9)
        {
            var pageModel = await _productService.QueryPage<ProductDto>(p => p.State == 1&&p.ProductCategoryId==id, page, pageSize, nameof(ProductDto.PublishDate) + " desc ");

            return View("Index",pageModel);
        }
        [Route("/[controller]/{action}/{id:int}.html",Name ="Shop_Detail_Tmp")]
        public async Task<IActionResult> Detail(int id)
        {
            var dataItem = await _productService.GetProduct(id);
            if (dataItem==null||dataItem.State == 0)
                return NotFound();
            await _productService.UpdateViewCount(id,dataItem.ViewCount+1);
            Response.Cookies.Append("Qy.produceId", id.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { Expires =DateTimeOffset.Now.AddMinutes(30) });
            return View(dataItem);
        }
    }
}