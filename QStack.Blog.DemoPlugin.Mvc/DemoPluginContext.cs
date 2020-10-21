using AspectCore.DynamicProxy;
using AutoMapper;
using QStack.Blog.DemoPlugin.Mvc.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using QStack.Framework.AspNetCore.Plugin.Contracts;

namespace QStack.Blog.DemoPlugin.Mvc
{
    [NonAspect]
    public class DemoPluginContext : IPluginContext
    {
        public const string Area = "DemoPlugin";
        public Dictionary<string, List<Assembly>> PluginEntityAssemblies 
            => new Dictionary<string, List<Assembly>>
                {
                    {"sfdb",new List<Assembly>{typeof(TestModel).Assembly} }
                };

        public string RouteArea => Area;

        public List<Profile> AutoMapperProfiles=>
            new List<Profile>
            {
                new TestModelProfiles()
            };
       

        public IServiceCollection Services
        {
            get
            {
                var services = new ServiceCollection();
                services.AddScoped<ITestService, TestService>();
                return services;
            }
        }

        public string TestUrl => "/demoplugin/home/index";
    }
}
