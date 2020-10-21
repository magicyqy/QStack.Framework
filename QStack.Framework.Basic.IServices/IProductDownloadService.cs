using QStack.Framework.Basic.ViewModel.Shop;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IProductDownloadService : IBaseService
    {
        [TransactionInterceptor]
        Task<ProductDownloadDto> GetProductDownload(string gid);
    }
}
