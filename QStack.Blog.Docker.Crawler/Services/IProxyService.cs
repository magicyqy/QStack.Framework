using QStack.Framework.Basic.IServices;
using QStack.Framework.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Services
{
    public interface IProxyService:IBaseService
    {
        [Caching(CacheMethod.Get)]
        Task<List<string>> GetAllAddresss(); 
    }
}
