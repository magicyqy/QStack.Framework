using QStack.Framework.AspNetCore.Plugin.Core;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.Basic.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.AspNetCore.Plugin.IServices
{
    public interface IPluginManagerService:IBaseService
    {
       
        Task<List<PluginInfoDto>> GetAllPlugins();
        Task<List<PluginInfoDto>> GetAllEnabledPlugins();
        Task<PluginInfoDto> GetPlugin(int pluginId);
        Task EnablePlugin(int pluginId);
        Task DeletePlugin(int pluginId);
        Task DisablePlugin(int pluginId);
        Task AddPlugins(PluginInfoDto pluginInfo);
    }
}
