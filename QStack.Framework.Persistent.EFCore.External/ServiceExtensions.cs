using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Persistent.EFCore.External.MigrationsBackup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Persistent.EFCore.External
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAutoMigration(this IServiceCollection services,Action<MigrationOptions> options)
        {
            var migrationOptions = new MigrationOptions();
            options.Invoke(migrationOptions);
            services.AddSingleton(s => migrationOptions);
            services.AddScoped<AutoMigration>();
            return services;
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="migrationOptions"></param>
        /// <returns></returns>
        public static async Task Backup(this IDaoFactory daoFactory, MigrationOptions migrationOptions)
        {
            if (!migrationOptions?.IsBackup == true)
                await Task.CompletedTask;
            var daoFactoryOption = daoFactory.DaoFactoryOption;

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
                        var provider = new PostgreSqlBackupProvider(daoFactoryOption, migrationOptions);
                        await provider.Backup();
                        break;
                    }
                case DbTypes.Sqlite:
                    //todo
                    break;
                default:
                    break;
            }
        }
    }
}
