using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QStack.Framework.Core;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Loader;

namespace QStack.Framework.Persistent.EFCore
{
    public static class ServiceExtentions
    {
        /// <summary>
        /// 注入持久层
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuraion"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddEFCore(this IServiceCollection services,IConfiguration configuraion,Action<DaoFactoryOption> optionConfigAction=null)
        {
            var sections = configuraion.GetSection("DaoFactories").GetChildren();
            if (sections == null||sections.Count()==0)
                throw new ServiceFrameworkException("config section \"DaoFactories\" not found or is empty");
            foreach (var section in sections)
            {
                var daoFactoryOption=  section.Get<DaoFactoryOption>();
                optionConfigAction?.Invoke(daoFactoryOption);
                services.AddSingleton<IDaoFactory>(serviceProvider => {
                    var builder = new DbContextOptionsBuilder<EFCoreDao>();
                    var loggerFatory = serviceProvider.GetService<ILoggerFactory>();
                    if (loggerFatory != null)
                        builder.UseLoggerFactory(loggerFatory);
                    switch (daoFactoryOption.DbType)
                    {
                        case DbTypes.MsSqlServer:
                            //todo
                            break;
                        case DbTypes.MySql:
                            //todo
                            break;
                        case DbTypes.NpgSql:
                            {
                                var assembly=AssemblyLoadContext.All.SelectMany(a => a.Assemblies).FirstOrDefault(a => a.GetName().Name.Equals("Npgsql.EntityFrameworkCore.PostgreSQL"));
                                if (assembly == null)
                                    assembly= AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, "Npgsql.EntityFrameworkCore.PostgreSQL.dll"));
                                var type=Type.GetType("Microsoft.EntityFrameworkCore.NpgsqlDbContextOptionsExtensions,Npgsql.EntityFrameworkCore.PostgreSQL");
                                var method = type.GetMethods().Where(m=>m.Name.Equals("UseNpgsql")).First();
                                //var method= type.GetMethod("UseNpgsql",new Type[] {typeof(DbContextOptionsBuilder),typeof(string),typeof(Action<>) });
                                //var gm = method.MakeGenericMethod(new Type[] { typeof(EFCoreDao) });
                                method.Invoke(null, new object[] {builder, daoFactoryOption.ConnectionString,null });
                                break;
                            }
                        case DbTypes.Sqlite:
                            //todo
                            break;
                        default:
                            throw new ServiceFrameworkException($"not support database type '{daoFactoryOption.DbType}'");
                    }
                 
                    //builder.ReplaceService<IModelCacheKeyFactory, MasterModelCacheKeyFactory>();
                    return new EFCoreDaoFactory(daoFactoryOption, builder);

                });
            }
            services.AddTransient<SessionScope>();
            services.AddScoped<SessionContext>();
            services.AddTransient<AbstractTransactionScope, EFCoreTransactionScope>();
            services.AddScoped<EFCoreTransactionContext>();
            services.AddScoped<IDaoCollection, DaoCollection>();
            //services.AddScoped<AutoMigration>();
            return services;
        }

     
    }
    

}
