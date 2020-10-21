using QStack.Framework.AspNetCore.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public delegate void PluginsContextChange(string pluginName,IPluginContext pluginContext);
   
   
    public interface IPluginContextContainer
    {
        event PluginsContextChange PluginsContextChangeEvent;
        void Add(string pluginName, IPluginContext context);
        void AddFromLoadContext(CollectibleAssemblyLoadContext assemblyLoadContext);
        List<IPluginContext> All();
        bool Any(string pluginName);
        IPluginContext Get(string pluginName);
        void Remove(string pluginName);
    }
}
