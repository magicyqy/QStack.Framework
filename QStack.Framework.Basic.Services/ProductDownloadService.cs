using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Shop;
using QStack.Framework.Basic.ViewModel.Shop;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    [SessionInterceptor]
    public class ProductDownloadService:AbstractService<ProductDownload>,IProductDownloadService
    {
        public ProductDownloadService(IMapper mapper)
        {

            Mapper = mapper;

        }

        [TransactionInterceptor]
        public async Task<ProductDownloadDto> GetProductDownload(string gid)
        {

            var entity =await Daos.CurrentDao.SingleAsync<ProductDownload>(d=>d.Gid.Equals(gid));

            entity.DownloadNum++;
           await  Daos.CurrentDao.Flush();

            return Mapper.Map<ProductDownloadDto>(entity);

        }
    }
}
