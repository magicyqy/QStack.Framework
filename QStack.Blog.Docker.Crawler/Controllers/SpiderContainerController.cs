using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetCore.CAP;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QStack.Framework.Core.Entity;

namespace QStack.Blog.Docker.Crawler.Controllers
{
    [Area(CrawlerPluginContext.Area)]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[Authorize(AuthenticationSchemes = "Cookies")]
	public class SpiderContainerController : Controller
	{
		private readonly ILogger _logger;
		private readonly IDockerCrawlerService _dockerCrawlerService;
		private readonly ICapPublisher _mq;

		public SpiderContainerController(IDockerCrawlerService dockerCrawlerService,
			ICapPublisher eventBus,
			ILogger<SpiderController> logger)
		{
			_logger = logger;
			_dockerCrawlerService = dockerCrawlerService;
			_mq = eventBus;
		}

	
	}
}
