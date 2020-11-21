using QStack.Framework.Basic.ViewModel.Shop;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IProductDownloadService : IBaseService
    {
    
        Task<ProductDownloadDto> GetProductDownload(string gid);
    }
}
