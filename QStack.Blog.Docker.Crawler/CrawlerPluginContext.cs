using AspectCore.DynamicProxy;
using AutoMapper;
using DotnetSpider.MessageQueue;
using DotnetSpider.RabbitMQ;
using QStack.Blog.Docker.Crawler.Models;
using QStack.Blog.Docker.Crawler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using QStack.Framework.AspNetCore.Plugin.Contracts;

namespace QStack.Blog.Docker.Crawler
{
    [NonAspect]
    public class CrawlerPluginContext : IPluginContext
    {
        public const string Area = "Crawler";
        public Dictionary<string, List<Assembly>> PluginEntityAssemblies 
            => new Dictionary<string, List<Assembly>>
                {
                    {"spiderdb",new List<Assembly>{typeof(Spider).Assembly} }
                };

        public string RouteArea => Area;

        public List<Profile> AutoMapperProfiles=>
            new List<Profile>
            {
                new CrawlerModelProfiles()
            };
       

        public IServiceCollection Services
        {
            get
            {
                var services = new ServiceCollection();
                //var configFile = Path.Combine(Directory.GetParent(this.GetType().Assembly.Location).FullName, "settings.json");
                //var crawlerOptions = new CrawlerOptions(new ConfigurationBuilder().AddJsonFile(configFile).Build());

                services.AddSingleton<CrawlerOptions>(serviceProvider=> {
                    var configFile = Path.Combine(serviceProvider.GetService<IOptions<PluginOptions>>().Value.InstallBasePath, this.GetType().Assembly.GetName().Name, "settings.json");
                    return new CrawlerOptions(new ConfigurationBuilder().AddJsonFile(configFile).Build());
                });
                services.AddScoped<IDockerCrawlerService, DockerCrawlerService>();
                services.AddScoped<ISpiderAgentService, SpiderAgentService>();
                services.AddScoped<JobService, JobService>();
              
                services.AddSingleton<IMessageQueue, RabbitMQMessageQueue>(serviceProvider => {
                    RabbitMQOptions options = new RabbitMQOptions();
                    var section = serviceProvider.GetService<IConfiguration>().GetSection("RabbitMQOptions");
                   serviceProvider.GetService<IConfiguration>().GetSection("RabbitMQOptions").Bind(options);
                    //options.Exchange = section.GetValue<string>("ExchangeName");


                    return new RabbitMQMessageQueue(Options.Create<RabbitMQOptions>(options), serviceProvider.GetService<ILoggerFactory>());
                 });
                services.AddScoped<IProxyService, ProxyService>();
                return services;
            }
        }

        public string TestUrl => "/crawler/home/index";
    }
}
