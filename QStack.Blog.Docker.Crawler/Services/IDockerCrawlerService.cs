
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Models;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
    public interface IDockerCrawlerService : IBaseService
    {
       
        Task<int> CountRunningContainers();
        //Task<int> CountRepositories();
        //Task<bool> AnyRepository(Expression<Func<DockerRepositoryDto, bool>> filterExpression);
       
    
        //Task DeleteRepository(int id);
        //Task<List<DockerRepositoryDto>> GetRepositories();
        Task<SpiderHistoryDto> AddSpiderContainer(SpiderHistoryDto spiderContainer);
        Task UpdateSpiderContainerStatus(int id,string status);
        //Task<DockerRepositoryDto> AddRepository(DockerRepositoryDto repository);
        Task<PageModel<SpiderHistoryDto>> GetSpiderHistories(Expression<Func<SpiderHistoryDto, bool>> p, string orderby, int page, int size);
        Task<List<SpiderStatisticsDto>> GetSpiderStatistics(Expression<Func<SpiderStatisticsDto, bool>> p);
        Task<SpiderHistoryDto> GetHistory(int id);
    }
}
