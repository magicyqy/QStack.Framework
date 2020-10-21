using QStack.Blog.Docker.Crawler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Portal.Controllers
{
	[Area(CrawlerPluginContext.Area)]
	[Route("{area:exists}/agents")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[Authorize(AuthenticationSchemes = "Cookies")]
	public class AgentController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
		
		[HttpGet("{id}/heartbeats")]
		public IActionResult Heartbeat()
		{
			return View();
		}
	}
}
