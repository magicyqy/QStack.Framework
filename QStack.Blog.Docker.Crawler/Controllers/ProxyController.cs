using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Services;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Util;

namespace QStack.Blog.Docker.Crawler.Controllers
{
    [Area(CrawlerPluginContext.Area)]
    public class ProxyController : Controller
    {
        readonly IProxyService _proxyService;
        public ProxyController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }
        public async Task<IActionResult> Index(ProxyIPDto proxyIPDto,int page=1,int pageSize=50)
        {
            Expression<Func<ProxyIPDto, bool>> filterExp = PredicateBuilder.True<ProxyIPDto>();
            if (proxyIPDto.Type.IsNotNullAndWhiteSpace())
                filterExp = PredicateBuilder.And(filterExp, p => p.Type == proxyIPDto.Type);
            if(proxyIPDto.Secret.IsNotNullAndWhiteSpace())
                filterExp = PredicateBuilder.And(filterExp, p => p.Secret == proxyIPDto.Secret);
            if(proxyIPDto.Address.IsNotNullAndWhiteSpace())
                filterExp = PredicateBuilder.And(filterExp, p => p.Address== proxyIPDto.Address);
            var model=await _proxyService.QueryPage<ProxyIPDto>(filterExp, page, pageSize, $"{nameof(proxyIPDto.LastValidTime)} desc");
            var addressList = await _proxyService.GetAllAddresss();
            ViewData["AddressList"] = addressList;
            return View(model);
        }
    }
}
