using AutoMapper;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Models;

namespace QStack.Blog.Docker.Crawler
{
    public class CrawlerModelProfiles : Profile
    {
        public CrawlerModelProfiles()
        {
            //base.CreateMap<DockerRepository, DockerRepositoryDto>().ReverseMap();
            base.CreateMap<Spider, SpiderDto>().ReverseMap();
            base.CreateMap<SpiderHistory, SpiderHistoryDto>().ReverseMap();
            base.CreateMap<SpiderStatistics, SpiderStatisticsDto>().ReverseMap();
            base.CreateMap<AgentInfo, AgentInfoDto>().ReverseMap();
            base.CreateMap<AgentHeartbeat, AgentHeartbeatDto>().ReverseMap();
            base.CreateMap<ProxyIP, ProxyIPDto>().ReverseMap();
        }
    }
}
