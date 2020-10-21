using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Models;
using QStack.Framework.Basic.Services;
using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
    [QStack.Framework.Core.Persistent.SessionInterceptor]
    public class DockerCrawlerService:AbstractService<Spider>,IDockerCrawlerService
    {
        public DockerCrawlerService(IMapper mapper)
        {
            Mapper = mapper;
        }

        //public async Task<DockerRepositoryDto> AddRepository(DockerRepositoryDto repository)
        //{
        //    var dockerRepository = Mapper.Map<DockerRepository>(repository);
        //    dockerRepository = await Daos.CurrentDao.AddOrUpdate<DockerRepository, int>(dockerRepository);

        //    return  Mapper.Map<DockerRepositoryDto>(dockerRepository);

        //}

        public async Task<SpiderHistoryDto> AddSpiderContainer(SpiderHistoryDto spiderContainer)
        {
            var container = Mapper.Map<SpiderHistory>(spiderContainer);
            container = await Daos.CurrentDao.AddOrUpdate<SpiderHistory, int>(container);

            return Mapper.Map<SpiderHistoryDto>(container);
        }

        //public async Task<bool> AnyRepository(Expression<Func<DockerRepositoryDto, bool>> filterExpression)
        //{
        //    var expression = Mapper.Map<Expression<Func<DockerRepository, bool>>>(filterExpression);

        //    return await Task.FromResult(Daos.CurrentDao.DbSet<DockerRepository>().Any(expression));
        //}

        //public async Task<int> CountRepositories()
        //{
        //    var count = Daos.CurrentDao.DbSet<DockerRepository>().Count();
        //    return await Task.FromResult(count);
        //}

        public async Task<int> CountRunningContainers()
        {
            var count = Daos.CurrentDao.DbSet<SpiderHistory>().Count();
            return await Task.FromResult(count);
        }

        //public async Task DeleteRepository(int id)
        //{
        //    await Daos.CurrentDao.DeleteById<DockerRepository>(id);
        //}

        public async Task<SpiderHistoryDto> GetHistory(int id)
        {
            var spiderHistory = await Daos.CurrentDao.Get<SpiderHistory>(id);

            return Mapper.Map<SpiderHistoryDto>(spiderHistory);
        }

        //public async Task<List<DockerRepositoryDto>> GetRepositories()
        //{
        //    var list = await Daos.CurrentDao.Query<DockerRepository>();

        //    return Mapper.Map<List<DockerRepositoryDto>>(list);
        //}

        public async Task<PageModel<SpiderHistoryDto>> GetSpiderHistories(Expression<Func<SpiderHistoryDto, bool>> p, string orderby, int page, int size)
        {
            var expression = Mapper.MapExpression<Expression<Func<SpiderHistory, bool>>> (p);

            var pageModel = await Daos.CurrentDao.QueryPage<SpiderHistory>(page, size, expression, new List<string>(), orderby);
            return Mapper.Map<PageModel<SpiderHistoryDto>>(pageModel);
        }

        public async Task<List<SpiderStatisticsDto>> GetSpiderStatistics(Expression<Func<SpiderStatisticsDto, bool>> p)
        {
            var expression = Mapper.MapExpression<Expression<Func<SpiderStatistics, bool>>>(p);

            var list = await Daos.CurrentDao.Query<SpiderStatistics>(expression);

            return Mapper.Map<List<SpiderStatisticsDto>>(list);
        }

        public async Task UpdateSpiderContainerStatus(int id,string status)
        {
            await Daos.CurrentDao.Update<SpiderHistory>(x => x.Id == id, x => new SpiderHistory { Status= status });
        }
    }

  
}
