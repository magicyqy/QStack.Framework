using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using QStack.Framework.Core.Persistent;
using QStack.Framework.Util;

namespace QStack.Blog
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

            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //SettingsHelpers.AddOrUpdateAppSetting("HadMigration", false, Path.Combine("app_data", "config", $"appsettings.Development.json"));
            var hostBuilder = QStack.Web.Program.CreateHostBuilder(args);
            hostBuilder.ConfigureServices((context, services) => {
                services.AddMvc(options => options.Filters.Add<Web.Filters.TitleFilter>())
                .AddRazorRuntimeCompilation()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.ApplicationParts.Add(new AssemblyPart(typeof(Program).Assembly));

                })
                .AddControllersAsServices()
                .AddJsonOptions(options => {
                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());

                });
            });

            return hostBuilder;

      
        }
    }
 
}
