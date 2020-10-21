using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Shop;
using QStack.Framework.Basic.ViewModel.Shop;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    [SessionInterceptor]
    public class ProductService:AbstractService<Product>,IProductService
    {
        public ProductService(IMapper mapper)
        {
            Mapper = mapper;
        }

        public async Task<ProductDto> GetProduct(int id)
        {

            var list = await Daos.CurrentDao.QueryInclude<Product>(p => p.Id == id, p => p.ProductImages, p => p.ProductDownloads,p=>p.ProductCategory);

            if(list.Count>0)
            {
                return Mapper.Map<ProductDto>(list.First());
            }
            return null;
        }
        [TransactionInterceptor]
        public async Task<int> AddOrUpdate(ProductDto dto)
        {
            
            if (dto.Id < 0)
                return (await base.AddAsync(dto)).Id;
            else
            {
                var model = Mapper.Map<Product>(dto);
                
                var entity = await Daos.CurrentDao.AddOrUpdate<Product, int>(model);
                //非从属表的导航属性另外维护
               var productImages = model.ProductImages.ToList();
                var productDownloads = model.ProductDownloads.ToList();
                entity.ProductImages?.Clear();
                entity.ProductDownloads?.Clear();
                await Daos.CurrentDao.Flush();
                await Daos.CurrentDao.AddRangeAsync(productImages);
                await Daos.CurrentDao.AddRangeAsync(productDownloads);

                await Daos.CurrentDao.Flush();

                return entity.Id;
            }
           
                
        }
        [TransactionInterceptor]
        public async Task<int> PostCatagory(ProductCategoryDto dto)
        {
            var entity = Mapper.Map<ProductCategory>(dto);
            return (await Daos.CurrentDao.AddOrUpdate<ProductCategory, int>(entity)).Id;
        }
        [TransactionInterceptor]
        public async Task<bool> DeleteCatagory(int id)
        {
            return await Daos.CurrentDao.DeleteById<ProductCategory>(id);
        }

        public async Task<List<ProductCategoryDto>> GetCatagories()
        {
            var list = await Daos.CurrentDao.Query<ProductCategory>();

            return Mapper.Map<List<ProductCategoryDto>>(list);
        }

        public async Task UpdateViewCount(int id,int count)
        {

            await Daos.CurrentDao.Update<Product>(p => p.Id == id, p => new Product { ViewCount= count });
        }
    }
}
