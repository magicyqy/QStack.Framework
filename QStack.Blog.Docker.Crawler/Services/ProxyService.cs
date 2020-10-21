using AutoMapper;
using QStack.Blog.Docker.Crawler.Models;
using QStack.Framework.Basic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Services
{
    public class ProxyService: AbstractService<ProxyIP>,IProxyService
    {
        public ProxyService(IMapper mapper)
        {
            Mapper = mapper;
        }

        public async Task<List<string>> GetAllAddresss()
        {
            var list = Daos.CurrentDao.DbSet<ProxyIP>().Select(p => p.Address).Distinct().ToList();
            list.Sort();

            return await Task.FromResult(list);
        }
    }
}
