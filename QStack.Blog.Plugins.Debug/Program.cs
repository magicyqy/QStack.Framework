using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using AutoMapper;

using QStack.Blog.Docker.Crawler;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using QStack.Framework.Core.Persistent;
using QStack.Framework.Util;

namespace QStack.Blog.Plugins.Debug
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AssemblyLoadContext.Default.Resolving += (context, assembly) =>
            {
                assembly.Version = assembly.Version ?? new Version("0.0.0.0");
                Func<AssemblyLoadContext, bool> filter = p => p.Assemblies.Any(p => p.GetName().Name == assembly.Name
                                                        && p.GetName().Version == assembly.Version);


                if (AssemblyLoadContext.All.Any(filter))
                {
                    var assemblyLoadContext = AssemblyLoadContext.All.First(filter);
                    Assembly ass = assemblyLoadContext.Assemblies.First(p => p.GetName().Name == assembly.Name
                        && p.GetName().Version == assembly.Version);
                    return ass;
                }
                return null;
            };

        

            //SettingsHelpers.AddOrUpdateAppSetting("HadMigration", false, Path.Combine("app_data", "config", $"appsettings.Development.json"));
            var hostBuilder = QStack.Blog.Program.CreateHostBuilder(args);
            hostBuilder.ConfigureServices((context, services) => {
                services.AddMvc(options => options.Filters.Add<Web.Filters.TitleFilter>())
                .AddRazorRuntimeCompilation()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.ApplicationParts.Add(new AssemblyPart(typeof(CrawlerOptions).Assembly));
                    apm.ApplicationParts.Add(new CompiledRazorAssemblyPart(Assembly.LoadFrom($"{typeof(CrawlerOptions).Assembly.GetName().Name}.Views.dll")));
                })
                //.AddApplicationPart(typeof(CrawlerOptions).Assembly)
                .AddApplicationPart(Assembly.LoadFrom("QStack.Blog.Docker.Crawler.Views.dll"))
                .AddControllersAsServices()
                .AddJsonOptions(options => {
                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());

                });

                foreach (var item in new QStack.Blog.Docker.Crawler.CrawlerPluginContext().Services)
                    services.Add(item);

                services.AddAutoMapper(typeof(CrawlerOptions).Assembly);
            });

            var host = hostBuilder.Build();
            var factories = host.Services.GetServices<IDaoFactory>();
            factories.First(factory => factory.FactoryName == "spiderdb").AddExtraEntityAssemblies(new Assembly[] { typeof(CrawlerOptions).Assembly });
            await host.RunAsync();
        }
    }

}
