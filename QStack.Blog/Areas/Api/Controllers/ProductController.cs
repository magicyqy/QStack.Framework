using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Blog.Areas.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Shop;
using QStack.Framework.Core.Entity;
using QStack.Framework.Util;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]

    public class ProductController : ApiBaseController
    {
        IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }

        public async Task<IActionResult> PostProduct(ProductDto productDto)
        {
           
            var result = new ResponseResult<int>();

            var id = await _productService.AddOrUpdate(productDto);

            result.Data = id;

            return Ok(result);
        }

        public async Task<IActionResult> UpdateProductState(int productId, int state)
        {
            await _productService.Update<ProductDto>(a => a.Id == productId, a => new ProductDto { State = state });

            return Ok(new ResponseResult());
        }

        public async Task<IActionResult> PostCatagory(ProductCategoryDto catagoryDto)
        {


            var id = await _productService.PostCatagory(catagoryDto);
            var result = new ResponseResult<int>(id);

            return Ok(result);
        }

        public async Task<IActionResult> Get(int id)
        {

            var result = new ResponseResult<ProductDto>();

            var productDto = await _productService.GetProduct(id);

            if (productDto == null)
            {
                result.Message = nameof(BusinessCode.Record_NotFound);
                result.Code = BusinessCode.Record_NotFound;
            }
            result.Data = productDto;


            return Ok(result);
        }
        public async Task<ResponseResult<PageModel<ProductDto>>> GetProducts(DataTableOption query)
        {
           

            return await base.SearchQuery<ProductDto,IProductService>(query);
        }
        public async Task<IActionResult> GetCatagories()
        {
            var result = new ResponseResult<List<ProductCategoryDto>>();

            var list = await _productService.GetCatagories();
            list = GetForTree(list, null);
            result.Data = list;

            return Ok(result);
        }


        public async Task<IActionResult> DeleteCatagory(int id)
        {
            var result = new ResponseResult();

            if (await _productService.CountAsync<ProductDto>(a => a.ProductCategoryId == id) > 0)
            {
                result.Message = nameof(BusinessCode.Record_Cascade_Exist);
                result.Code = BusinessCode.Record_Cascade_Exist;
            }
            else
                await _productService.DeleteCatagory(id);


            return Ok(result);
        }
    }
}