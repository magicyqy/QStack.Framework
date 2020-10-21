using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Blog.Docker.Crawler.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace QStack.Blog.Docker.Crawler.Controllers
{
    [Area(CrawlerPluginContext.Area)]
    [Route("{area:exists}/spider")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[Authorize(AuthenticationSchemes = "Cookies")]
	public class SpiderController : Controller
	{
		
		public SpiderController()
		{
		
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("{id}/histories")]
		public IActionResult History(int id)
		{
			return View();
		}



		private async Task<List<string>> GetRepositoryTagsAsync(string schema, string registry, string repository,
			string user,
			string password)
		{
			try
			{
				registry = $"{schema}://{registry}";
				var httpClient = HttpClientFactory.GetHttpClient(registry,
					user, password);
				var json = await httpClient.GetStringAsync(
					$"{registry}/v2/{repository}/tags/list");
				var repositoryTags = JsonConvert.DeserializeObject<RepositoryTags>(json);
				repositoryTags.Tags.Remove("latest");
				var list = new List<string> {"latest"};
				repositoryTags.Tags.Sort();
				list.AddRange(repositoryTags.Tags);
				return list.Take(20).ToList();
			}
			catch (Exception e)
			{
				
				return new List<string>();
			}
		}

	

	}
}
