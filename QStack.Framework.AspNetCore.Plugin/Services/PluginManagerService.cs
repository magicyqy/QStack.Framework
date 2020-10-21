using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.AspNetCore.Plugin.Core;
using QStack.Framework.AspNetCore.Plugin.IServices;
using QStack.Framework.AspNetCore.Plugin.Models;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.Basic.Services;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using QStack.Framework.Persistent.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Framework.AspNetCore.Plugin.Services
{
    [SessionInterceptor]
    public class PluginManagerService : AbstractService<PluginInfo>, IPluginManagerService
    {

        
        private readonly IMvcModuleSetup _mvcModuleSetup = null;
        private readonly IPluginsAssemblyLoadContexts _puginsAssemblyLoadContexts;
        readonly IServiceProvider _serviceProvider;

        public PluginManagerService(IMapper mapper,
            IMvcModuleSetup mvcModuleSetup,
            IPluginsAssemblyLoadContexts puginsAssemblyLoadContexts,IServiceProvider serviceProvider)
        {
            Mapper = mapper;
           
            _mvcModuleSetup = mvcModuleSetup;
            _puginsAssemblyLoadContexts = puginsAssemblyLoadContexts;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<PluginInfoDto>> GetAllPlugins()
        {
            return await this.GetAll<PluginInfoDto>();
        }

        public async Task<List<PluginInfoDto>> GetAllEnabledPlugins()
        {
            var list=await this.Query<PluginInfoDto>(p => p.IsEnable == true);
            return list.ToList();
        }

        public async Task<PluginInfoDto> GetPlugin(int pluginId)
        {
            return await this.Get<PluginInfoDto>(p=>p.Id == pluginId);
        }

        public async Task EnablePlugin(int pluginId)
        {
            PluginInfo pluginInfo =await Daos.CurrentDao.Get<PluginInfo>(pluginId);
            pluginInfo.IsEnable = true;
            await Daos.CurrentDao.Flush();

            _mvcModuleSetup.EnableModule(pluginInfo.Name);
        }

        public async Task DeletePlugin(int pluginId)
        {
            PluginInfo plugininfo = await Daos.CurrentDao.Get<PluginInfo>(pluginId);
            ModuleChangeDelegate action = (moduleEvent, context) =>
            {

                if (context.PluginName != plugininfo.Name || !plugininfo.IsMigration || context?.PluginContext == null || context?.PluginContext.PluginEntityAssemblies.Count() == 0)
                    return;
                if (moduleEvent == ModuleEvent.UnInstalled)
                {
                    //var pluginContexts = pluginsLoadContexts.All().Select(c => c.PluginContext).SkipWhile(c => c == null);

                    #region 重置实体类及数据库
                    var daoFactories = _serviceProvider.GetServices<IDaoFactory>();
                    foreach (var factory in daoFactories)
                    {
                        if (context.PluginContext.PluginEntityAssemblies.ContainsKey(factory.FactoryName))
                        {

                            factory.RemoveExtraEntityAssemblies(context.PluginContext.PluginEntityAssemblies[factory.FactoryName].ToArray());

                          
                        }

                    }
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var autoMigration = scope.ServiceProvider.GetService<AutoMigration>();
                        autoMigration.GenerateMigrations();

                        //if (context.PluginContext?.TestUrl.Any() == true)
                        //    this.Update<PluginInfoDto>(p => p.Name == context.PluginName, p => new PluginInfoDto { TestUrl = context.PluginContext.TestUrl, RouteArea = context.PluginContext.RouteArea }).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    #endregion
                }
            };
            _mvcModuleSetup.ModuleChangeEventHandler += action;
         

            if (plugininfo.IsEnable)
            {
               await DisablePlugin(pluginId);
            }
            await Daos.CurrentDao.Delete(plugininfo);
            await Daos.CurrentDao.Flush();
            _mvcModuleSetup.DeleteModule(plugininfo.Name);
            _mvcModuleSetup.ModuleChangeEventHandler -= action;
        }

        public async Task DisablePlugin(int pluginId)
        {
            PluginInfo pluginInfo = await Daos.CurrentDao.Get<PluginInfo>(pluginId);
            pluginInfo.IsEnable = false;
            await Daos.CurrentDao.Flush();
            _mvcModuleSetup.DisableModule(pluginInfo.Name);
        }

        public async Task AddPlugins(PluginInfoDto plugininfo)
        {
            //监听安装事件
            ModuleChangeDelegate action = (moduleEvent, context) =>
            {

                if (context.PluginName!=plugininfo.Name|| !plugininfo.IsMigration|| context?.PluginContext == null || context?.PluginContext.PluginEntityAssemblies.Count() == 0)
                    return;
                if (moduleEvent == ModuleEvent.Installed)
                {
                    //var pluginContexts = pluginsLoadContexts.All().Select(c => c.PluginContext).SkipWhile(c => c == null);

                    #region 重置实体类及数据库
                    var daoFactories = _serviceProvider.GetServices<IDaoFactory>();
                    foreach (var factory in daoFactories)
                    {
                        if (context.PluginContext.PluginEntityAssemblies.ContainsKey(factory.FactoryName))
                        {
                          
                            factory.RemoveExtraEntityAssemblies(context.PluginContext.PluginEntityAssemblies[factory.FactoryName].ToArray());
                         
                            factory.AddExtraEntityAssemblies(context.PluginContext.PluginEntityAssemblies[factory.FactoryName].ToArray());
                        }

                    }
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var autoMigration = scope.ServiceProvider.GetService<AutoMigration>();
                        autoMigration.GenerateMigrations();
                                           
                        //if (context.PluginContext?.TestUrl.Any() == true)
                        //    this.Update<PluginInfoDto>(p => p.Name == context.PluginName, p => new PluginInfoDto { TestUrl = context.PluginContext.TestUrl, RouteArea = context.PluginContext.RouteArea }).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    #endregion
                }
            };
            _mvcModuleSetup.ModuleChangeEventHandler += action;

            PluginInfoDto existedPlugin =await this.Get<PluginInfoDto>(p=>p.Name== plugininfo.Name);

            if (existedPlugin == null)
            {
                await InitializePlugin(plugininfo);
            }
            else if (new Version(plugininfo.Version) > new Version(existedPlugin.Version))
            {
                await UpgradePlugin(plugininfo, existedPlugin);
            }
            else if (new Version(plugininfo.Version) == new Version(existedPlugin.Version))
            {
                throw new ArgumentException("The package version is same as the current plugin version.");
            }
            else
            {
                await DegradePlugin(plugininfo, existedPlugin);
            }
            _mvcModuleSetup.ModuleChangeEventHandler -= action;
            var pluginContext = _puginsAssemblyLoadContexts.Get(plugininfo.Name).PluginContext;
            if (pluginContext.TestUrl?.Any() == true)
                await Daos.CurrentDao.Update<PluginInfo>(p => p.Name == plugininfo.Name, p => new PluginInfo { TestUrl = pluginContext.TestUrl, RouteArea = pluginContext.RouteArea });
        }
        [TransactionInterceptor]
        private async Task InitializePlugin(PluginInfoDto pluginInfo)
        {
            //PluginInfo plugin = new PluginInfo
            //{
            //    Name = pluginInfo.Name,
            //    DisplayName = pluginInfo.DisplayName,
            //    UniqueKey = pluginInfo.UniqueKey,
            //    Version = pluginInfo.Version
            //};

            PluginInfo plugin = Mapper.Map<PluginInfo>(pluginInfo);
            _mvcModuleSetup.LoadModule(pluginInfo.Name);
            await Daos.CurrentDao.AddAsync(plugin); ;
        }
        [TransactionInterceptor]
        public async Task UpgradePlugin(PluginInfoDto newPlugin, PluginInfoDto oldPlugin)
        {
            if (oldPlugin.IsEnable)
                throw new InvalidOperationException("the plugin is running.please stop it before upgradePlugin");

            _mvcModuleSetup.ReLoadModule(newPlugin.Name);
            await Daos.CurrentDao.Update<PluginInfo>(p => p.Id == oldPlugin.Id, p => new PluginInfo { Version = newPlugin.Version });

        }
        [TransactionInterceptor]
        public async Task DegradePlugin(PluginInfoDto newPlugin, PluginInfoDto oldPlugin)
        {
            if (oldPlugin.IsEnable)
                throw new InvalidOperationException("the plugin is running.please stop it before degradePlugin");
          
          
            _mvcModuleSetup.ReLoadModule(oldPlugin.Name);
            await Daos.CurrentDao.Update<PluginInfo>(p => p.Id == oldPlugin.Id, p => new PluginInfo { Version = newPlugin.Version });
        }

       
    }
}
