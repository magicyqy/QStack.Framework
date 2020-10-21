using Docker.DotNet;
using Docker.DotNet.Models;
using QStack.Blog.Docker.Crawler.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Services
{
    public class JobService
    {
		readonly ILogger<JobService> _logger;
		readonly IDockerCrawlerService _dockerCrawlerService;
		readonly CrawlerOptions _crawlerOptions;

		public JobService(ILogger<JobService> _logger, 
			IDockerCrawlerService _dockerCrawlerService, 
			CrawlerOptions _crawlerOptions)
        {
			this._logger = _logger;
			this._dockerCrawlerService = _dockerCrawlerService;
			this._crawlerOptions = _crawlerOptions;
        }

		public async Task ExecuteJob(int spiderId)
		{

			try
			{
				_logger.LogInformation($"触发任务 {spiderId}");

				var spider = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == spiderId);
				if (spider == null)
				{
					_logger.LogError($"任务 {spiderId} 不存在");
					return;
				}

				if (!spider.Enabled)
				{
					_logger.LogError($"任务 {spiderId} 被禁用");
					return;
				}

				var client = new DockerClientConfiguration(
						new Uri(_crawlerOptions.Docker))
					.CreateClient();
				var batch = Guid.NewGuid().ToString("N");
				var env = new List<string>((spider.Environment ?? "").Split(new[] { " ", "\n" },
					StringSplitOptions.RemoveEmptyEntries))
					{
						$"DOTNET_SPIDER_ID={batch}",						
						$"DOTNET_SPIDER_NAME={spider.Name}"
					};
				
				var name = $"dotnetspider-{spider.Id}-{batch}";
				var parameters = new CreateContainerParameters
				{
					Image = spider.Image,
					Name = name,
					Labels = new Dictionary<string, string>
						{
							{"dotnetspider.spider.id", spider.Id.ToString()},
							{"dotnetspider.spider.batch", batch},						
							{"dotnetspider.spider.name", spider.Name}
						},
					Env = env,
					HostConfig = new HostConfig()
				};
				var volumes = new HashSet<string>();
				foreach (var volume in _crawlerOptions.DockerVolumes)
				{
					volumes.Add(volume);
				}

				var configVolumes = new List<string>((spider.Volume ?? "").Split(new[] { " ", "\n" },
					StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
				foreach (var volume in configVolumes)
				{
					volumes.Add(volume);
				}

				parameters.HostConfig.Binds = volumes.ToList(); ;
				var result = await client.Containers.CreateContainerAsync(parameters);

				if (result.ID == null)
				{
					_logger.LogError($"创建任务 {spiderId} 实例失败: {string.Join(", ", result.Warnings)}");
				}

				var spiderContainer = new SpiderHistoryDto
				{
					ContainerId = result.ID,
					Batch = batch,
					SpiderId = spider.Id,
					SpiderName = spider.Name,
					Status = "Created",
					CreationTime = DateTimeOffset.Now
				};

				spiderContainer = await _dockerCrawlerService.AddSpiderContainer(spiderContainer);


				var startResult =
					await client.Containers.StartContainerAsync(result.ID, new ContainerStartParameters());
				await _dockerCrawlerService.UpdateSpiderContainerStatus(spiderContainer.Id, startResult ? "Success" : "Failed");


				_logger.LogInformation($"触发任务 {spiderId} 完成");
			}
			catch (Exception ex)
			{
				_logger.LogError($"触发任务 {spiderId} 失败: {ex}");
			}

		}
	}
}
