using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Core.Cache;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class CacheManagerController : ApiBaseController
    {
        ICache _cache;
        public CacheManagerController(ICache cache)
        {
            this._cache = cache;
        }

        //public async Task<ResponseResult> GetAll()
        //{
        //    var list = (await _cache.GetAllByPrefixAsync<object>()).Take(100);
        //    foreach(var item in list)
        //    {

        //    }
        //}
    }
}