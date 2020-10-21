using QStack.Framework.Basic.ViewModel.Shop;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IProductService : IBaseService
    {
       
        Task<int> AddOrUpdate(ProductDto model);
        Task<bool> DeleteCatagory(int id);
        Task<ProductDto> GetProduct(int id);
        Task<List<ProductCategoryDto>> GetCatagories();
        Task<int> PostCatagory(ProductCategoryDto dto);
        Task UpdateViewCount(int id,int count);
    }
}
