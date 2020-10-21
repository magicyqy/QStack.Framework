using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;

namespace QStack.Framework.HangFire
{
    public static class HangFireServiceExtensions
    { 
        public static  IServiceCollection AddHangFire(this IServiceCollection services,IConfiguration Configuration)
        {
            services.Configure<HangFireOptions>(options => Configuration.GetSection("HangFireOptions").Bind(options));
            services.AddHangfire((servieProvider,configuration) => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UsePostgreSqlStorage(servieProvider.GetRequiredService<IOptions<HangFireOptions>>().Value.ConnectionString, new PostgreSqlStorageOptions
               {
                  
                   InvisibilityTimeout = TimeSpan.FromMinutes(5),
                   QueuePollInterval = TimeSpan.FromSeconds(30),
                   SchemaName="hangfire"
               })
               .UseActivator(new ContainerJobActivator(servieProvider))
            );

            // Add the processing server as IHostedService
           
            services.AddHangfireServer();

            return services;
        }
    

        public static void UserHangFire(this IApplicationBuilder app)
        {
       
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            //var backgroundJobs = app.ApplicationServices.GetRequiredService<IBackgroundJobClient>();
            //    backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
        }
    }
}
