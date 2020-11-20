using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using QStack.Framework.Core.Config;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System;
using System.Reflection;
using System.Linq;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace QStack.Web
{
    public class Program
    {
        public static void Main(string[] args)
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
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if DEBUG
              .UseEnvironment(Environments.Development)
#endif
                //.ConfigureHostConfiguration(builder =>
                //  builder.AddJsonFile("serilogConfig.json")
                //)
               .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(Path.Combine("app_data","config", "appsettings.json"));
                    config.AddJsonFile(Path.Combine("app_data", "config", $"appsettings.{ hostingContext.HostingEnvironment.EnvironmentName}.json"));
                    config.AddJsonFile(Path.Combine("app_data", "config", "serilogConfig.json"));
                    config.AddJsonFile(Path.Combine("app_data", "config", "IpRateLimiting.json"));
                    config.AddSwarmSecrets(new List<SwarmSecretsPath>{
                        new SwarmSecretsPath(Path.Combine("app_data","secrets"))
                    });
                    config.AddInterpolation(config);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //修改默认request body大小
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
                    });

                }).UseSerilog((context,loggerConfiguration)=> 
                {
                    
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                     //.Enrich.FromLogContext();

                 })
                .UseServiceProviderFactory(new CutomDynamicProxyServiceProviderFactory());
    }

    public class CutomDynamicProxyServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            services = services.WeaveDynamicProxyService();
            services.AddSingleton<IServiceCollection>(services);
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildServiceProvider();
        }
    }
}
