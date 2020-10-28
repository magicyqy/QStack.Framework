using System.Threading.Tasks;
using QStack.Blog.Docker.Crawler.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QStack.Blog.Docker.Crawler.Controllers
{
    [Area(CrawlerPluginContext.Area)]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[Authorize(AuthenticationSchemes = "Cookies")]
	public class HomeController : Controller
	{
		private readonly IDockerCrawlerService _dockerCrawlerService;

		public HomeController(IDockerCrawlerService dockerCrawlerService)
		{
			_dockerCrawlerService = dockerCrawlerService;
		}

		public async Task<IActionResult> Index()
		{
			//ViewData["Agent"] = await _dbContext.Set<DownloaderAgent>().CountAsync();
			//ViewData["OnlineAgent"] = await _dbContext.Set<DownloaderAgent>().CountAsync();
			
			ViewData["Spider"] = await _dockerCrawlerService.CountAsync<SpiderDto>();
			ViewData["RunningSpider"] = await _dockerCrawlerService.CountRunningContainers();
				
			return View();
		}

		
	}
}
