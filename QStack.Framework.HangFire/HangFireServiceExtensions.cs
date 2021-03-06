﻿using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Linq;
using Hangfire.SqlServer;
using QStack.Framework.Persistent;
using QStack.Framework.Core;
namespace QStack.Framework.HangFire
{
    public static class HangFireServiceExtensions
    {
        public static IServiceCollection AddHangFire(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<HangFireOptions>(options => Configuration.GetSection("HangFireOptions").Bind(options));
            services.AddHangfire((servieProvider, configuration) =>
            {
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseTypeResolver(typeName =>
                {
                    typeName = typeName.Split(",")[0];
                    var type = Type.GetType(typeName);
                    if (type == null)
                        type = AssemblyLoadContext.All.SelectMany(c => c.Assemblies).SelectMany(a => a.GetTypes()).SingleOrDefault(t => t.FullName.Equals(typeName));

                    return type;
                })
                    .UseRecommendedSerializerSettings()
                    .UseActivator(new ContainerJobActivator(servieProvider));
                var hangFireOptions = servieProvider.GetRequiredService<IOptions<HangFireOptions>>().Value;
                switch (hangFireOptions.DbType)
                {
                    case DbTypes.MsSqlServer:
                        {
                            configuration
                               .UseSqlServerStorage(hangFireOptions.ConnectionString, new SqlServerStorageOptions
                               {

                                   InvisibilityTimeout = TimeSpan.FromMinutes(5),
                                   QueuePollInterval = TimeSpan.FromSeconds(30),
                                   SchemaName = "hangfire"
                               });
                            break;
                        }
                    case DbTypes.MySql:
                        //todo
                        break;
                    case DbTypes.NpgSql:
                        {
                            configuration
                               .UsePostgreSqlStorage(hangFireOptions.ConnectionString, new PostgreSqlStorageOptions
                               {

                                   InvisibilityTimeout = TimeSpan.FromMinutes(5),
                                   QueuePollInterval = TimeSpan.FromSeconds(30),
                                   SchemaName = "hangfire"
                               });
                            break;
                        }
                    case DbTypes.Sqlite:
                        //todo
                        break;
                    default:
                        throw new ServiceFrameworkException($"not support database type '{hangFireOptions.DbType}'");
                }
               
                   
            }
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

            //var backgroundJobs = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();
            //backgroundJobs.AddOrUpdate<TestJob>("testjob",
            //    x =>x.Run(),
            //    "0 1 0 * * ?");
        }

        public class TestJob
        {
            public void Run()
            {
                Console.WriteLine("Hello world from Hangfire!");
            }
        }
    }
}
