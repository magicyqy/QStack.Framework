using QStack.Framework.Basic.ViewModel.Shop;
using System.Collections.Generic;
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
