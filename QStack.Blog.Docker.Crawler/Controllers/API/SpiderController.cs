using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DotnetSpider.Extensions;
using DotnetSpider.MessageQueue;
using Hangfire;
using QStack.Blog.Docker.Crawler;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Services;
using QStack.Blog.Docker.Crawler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCrontab;
using QStack.Framework.Core.Entity;

namespace DotnetSpider.Portal.Controllers.API
{

    [ApiController]
    [Area(CrawlerPluginContext.Area)]
    [Route("{area:exists}/api/v1.0/spiders")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class SpiderController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDockerCrawlerService _dockerCrawlerService;
        private readonly IBackgroundJobClient _sched;
        private readonly IRecurringJobManager _recurringJobManager;
        readonly CrawlerOptions _crawlerOptions;
        private readonly IMessageQueue _mq;

        public SpiderController(IDockerCrawlerService dockerCrawlerService,
            IBackgroundJobClient sched, IRecurringJobManager recurringJobManager,
        ILogger<SpiderController> logger, CrawlerOptions crawlerOptions, IMessageQueue mq)
        {
            _logger = logger;
            _dockerCrawlerService = dockerCrawlerService;
            _sched = sched;
            _recurringJobManager = recurringJobManager;
            _crawlerOptions = crawlerOptions;
            _mq = mq;
        }


        [HttpPost]
        public async Task<bool> CreateAsync(SpiderDto viewModel)
        {

            if (ModelState.IsValid)
            {
                var exists = await _dockerCrawlerService.CountAsync<SpiderDto>(x =>
                    x.Name == viewModel.Name);
                if (exists > 0)
                {
                    ModelState.AddModelError("Name", "名称已经存在");
                }

                try
                {
                    CrontabSchedule.Parse(viewModel.Cron);
                }
                catch
                {
                    ModelState.AddModelError("Cron", "Cron 表达式不正确");
                }

                viewModel.CreationTime = DateTimeOffset.Now;
                viewModel.LastModificationTime = DateTimeOffset.Now;
                viewModel = await _dockerCrawlerService.AddAsync(viewModel);


                 var id = viewModel.Id;
             
               return true;
               

            }
            else
            {
                throw new ApplicationException("ModelState is invalid");
            }

            
        }

        [HttpPut("{id}")]
        public async Task<bool> UpdateAsync(int id, SpiderDto viewModel)
        {

            if (ModelState.IsValid)
            {
                var spider = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == id);
                if (spider == null)
                {
                    throw new ApplicationException($"Spider {id} exists");
                }

                var exists = await _dockerCrawlerService.CountAsync<SpiderDto>(x =>
                    x.Name == viewModel.Name && x.Id != id);
                if (exists > 0)
                {
                    ModelState.AddModelError("Name", "名称已经存在");
                }

                try
                {
                    CrontabSchedule.Parse(viewModel.Cron);
                }
                catch
                {
                    ModelState.AddModelError("Cron", "Cron 表达式不正确");
                }

          
                spider.Name = viewModel.Name;
                spider.Cron = viewModel.Cron;

                spider.Environment = viewModel.Environment;

                spider.LastModificationTime = DateTimeOffset.Now;

                await _dockerCrawlerService.AddOrUpdate<SpiderDto,int>(spider);
                _recurringJobManager.RemoveIfExists(spider.Name);
                await ScheduleJobAsync(spider);
              



                return true;
            }
            else
                throw new ApplicationException("ModelState is invalid");
        }


        private async Task ScheduleJobAsync(SpiderDto spider)
        {
            _recurringJobManager.AddOrUpdate<JobService>(spider.Name, x => x.ExecuteJob(spider.Id), spider.Cron, TimeZoneInfo.Local);

            await Task.CompletedTask;
        }

        //if (!string.IsNullOrWhiteSpace(viewModel.Registry))
        //{
        //	viewModel.Tags = await GetRepositoryTagsAsync(dockerRepository.Schema, dockerRepository.Registry,
        //		dockerRepository.Repository,
        //		dockerRepository.UserName, dockerRepository.Password);
        //}




        [HttpGet]
        public async Task<PageModel<SpiderDto>> PagedQueryAsync(string keyword, int page, int size)
        {
            page = page <= 1 ? 1 : page;
            size = size <= 20 ? 20 : size;
            PageModel<SpiderDto> viewModel;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                viewModel = await _dockerCrawlerService.QueryPage<SpiderDto>(s => true, page, size, $"{nameof(SpiderDto.Id)} desc");

            }
            else
            {
                viewModel = await _dockerCrawlerService.QueryPage<SpiderDto>(x => x.Name.Contains(keyword), page, size, $"{nameof(SpiderDto.Id)} desc");

            }

            return viewModel;
        }

        [HttpPut("{id}/run")]
        public async Task<bool> RunAsync(int id)
        {

            try
            {
                var item = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == id);
                if (item != null)
                {
                    _recurringJobManager.Trigger(item.Name);
                    
                    return await Task.FromResult(true);
                }
                 
            }
            catch (Exception e)
            {
                _logger.LogError($"启动失败: {e}");
             
            }
            return await Task.FromResult(false);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == id);
            if (item != null)
            {

                await _dockerCrawlerService.DeleteByIdAsync(id);
                _recurringJobManager.RemoveIfExists(item.Name);
           
                return Ok();

            }
            else 
                return NotFound();
        }

        [HttpPut("{id}/disable")]
        public async Task<IActionResult> DisableAsync(int id)
        {
            try
            {
                var item = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == id);
                if (item != null && item.Enabled)
                {
                    _recurringJobManager.RemoveIfExists(item.Name);
                    await _dockerCrawlerService.Update<SpiderDto>(x => x.Id == id, x => new SpiderDto { Enabled = false });

                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"禁用失败: {e}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { e.Message });
            }
        }


        [HttpPut("{id}/enable")]
        public async Task<IActionResult> EnableAsync(int id)
        {
            try
            {
                var item = await _dockerCrawlerService.Get<SpiderDto>(x => x.Id == id);
                if (item != null && !item.Enabled)
                {
                    await ScheduleJobAsync(item);
                    await _dockerCrawlerService.Update<SpiderDto>(x => x.Id == id, x => new SpiderDto { Enabled = true });

                }
              

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"启用失败: {e}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { e.Message });
            }
        }


        [HttpGet("{id}/histories")]
        public async Task<PageModel<SpiderHistoryViewObject>> PagedQueryHistoryAsync(int id, int page, int size)
        {
            page = page <= 1 ? 1 : page;
            size = size <= 20 ? 20 : size;

            var containers = await _dockerCrawlerService.GetSpiderHistories(x => x.SpiderId== id, $"{nameof(SpiderHistoryDto.Id)} desc", page, size);


            var batches = containers.Data.Select(x => x.Batch).ToList();
            var dict = (await _dockerCrawlerService.GetSpiderStatistics(x => batches.Contains(x.Id)))
                .ToDictionary(x => x.Id, x => x);

            var list = new List<SpiderHistoryViewObject>();
            foreach (var container in containers.Data)
            {
                var item = new SpiderHistoryViewObject
                {
                    Id=container.Id,
                    Batch = container.Batch,
                    ContainerId = container.ContainerId,
                    SpiderName = container.SpiderName,

                    Status = container.Status

                };
                if (dict.ContainsKey(item.Batch))
                {
                    item.Total = dict[item.Batch].Total;
                    item.Failure = dict[item.Batch].Failure;
                    item.Success = dict[item.Batch].Success;
                    item.Start = dict[item.Batch].Start?.ToString("yyyy-MM-dd HH:mm:ss");
                    item.Exit = dict[item.Batch].Exit?.ToString("yyyy-MM-dd HH:mm:ss");
                    item.Left = item.Total - item.Success;
                }

                list.Add(item);
            }

            return new PageModel<SpiderHistoryViewObject>
            {
                Data = list,
                Page = page,
                PageSize = size,
                TotalCount = containers.TotalCount

            };
        }

        [HttpPut("{id}/exit")]
        public async Task<bool> ExitAsync(int id)
        {
            try
            {
                var spiderHistory = await _dockerCrawlerService.GetHistory(id);
                if (spiderHistory == null)
                {
                    throw new ApplicationException("Spider history is not exits");
                }

                var spiderId = spiderHistory.Batch.ToUpper();
                var topic = string.Format(TopicNames.Spider, spiderHistory.Batch.ToUpper());
                _logger.LogInformation($"Try stop spider {topic}");
                await _mq.PublishAsBytesAsync(topic,
                    new Message.Spider.Exit(spiderId));

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"关闭失败: {e}");
                return false;
            }
        }


    }
}
