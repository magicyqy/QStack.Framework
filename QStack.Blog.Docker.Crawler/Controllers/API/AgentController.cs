using System.Threading.Tasks;
using DotNetCore.CAP;
using DotnetSpider.Extensions;
using Hangfire;
using QStack.Blog.Docker.Crawler;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QStack.Framework.Core.Entity;
namespace DotnetSpider.Portal.Controllers.API
{
    [Area(CrawlerPluginContext.Area)]
	[ApiController]
	[Route(CrawlerPluginContext.Area+"/api/v1.0/agents")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[Authorize(AuthenticationSchemes = "Cookies")]
	public class AgentController : Controller
	{
		private readonly ILogger _logger;
		private readonly ISpiderAgentService _spiderAgentService;
		private readonly IBackgroundJobClient _sched;
		readonly CrawlerOptions _crawlerOptions;
		private readonly ICapPublisher _mq;
	

		public AgentController(ISpiderAgentService spiderAgentService,
			IBackgroundJobClient sched,
			ILogger<AgentController> logger, CrawlerOptions crawlerOptions,ICapPublisher cap)
		{
			_logger = logger;
			_spiderAgentService = spiderAgentService;
			_sched = sched;
			_crawlerOptions = crawlerOptions;
			_mq = cap;
		}


		[HttpGet]
		public async Task<PageModel<AgentInfoDto>> PagedQueryAsync(string keyword, int page, int limit)
		{
			PageModel<AgentInfoDto> @out;
			if (!string.IsNullOrWhiteSpace(keyword))
			{
				@out = await _spiderAgentService.GetSpiderAgentInfos(page, limit, x => x.Name.Contains(keyword) || x.Id.Contains(keyword),
						$"{nameof(AgentInfoDto.CreationTime)} desc");
			}
			else
			{
				@out = await _spiderAgentService.GetSpiderAgentInfos(page, limit, null,
						$"{nameof(AgentInfoDto.CreationTime)} desc"); ;
			}

			return @out;
		}

		[HttpGet("{id}/heartbeats")]
		public async Task<PageModel<AgentHeartbeatDto>> PagedQueryHeartbeatAsync(string id, int page,
			int limit)
		{
			page = page <= 1 ? 1 : page;
			limit = limit <= 5 ? 5 : limit;

			var @out = await _spiderAgentService.GetSpiderAgentHeartBeats(page, limit, x => x.AgentId == id,
				$"{nameof(AgentHeartbeatDto.Id)} desc");
			return @out;
		}

		[HttpDelete("{id}")]
		public async Task<IApiResult> DeleteAsync(string id)
		{
			if (await _spiderAgentService.CountAsync<AgentInfoDto>(x => x.Id == id)<=0)
			{
				return new FailedResult("Agent is not exists");
			}

            await _mq.PublishAsync(string.Format(TopicNames.Agent, id.ToUpper()), new Message.Agent.Exit { AgentId = id }.Serialize());

            await _spiderAgentService.DeleteAgentInfo(id);
			
			return new ApiResult("OK");
		}

		[HttpPut("{id}/exit")]
		public async Task<IApiResult> ExitAsync(string id)
		{
			if ((await _spiderAgentService.CountAsync<AgentInfoDto>(x => x.Id == id))<=0)
			{
				return new FailedResult("Agent is not exists");
			}
			var ss = new AgentInfoDto();

            await _mq.PublishAsync(string.Format(TopicNames.Agent, id.ToUpper()), (new Message.Agent.Exit { AgentId = id }).Serialize());
            return new ApiResult("OK");
		}
	}
}
