using QStack.Framework.AspNetCore.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    [Obsolete]
    public class PluginContextContainer : IPluginContextContainer
    {
        private readonly Dictionary<string, IPluginContext> _pluginContexts;
        

        public event PluginsContextChange PluginsContextChangeEvent;

        public PluginContextContainer()
        {
            _pluginContexts = new Dictionary<string, IPluginContext>();
        }

        public void Add(string pluginName, IPluginContext context)
        {
            _pluginContexts.Add(pluginName, context);
            PluginsContextChangeEvent?.Invoke(pluginName,context);


        }

        public List<IPluginContext> All()
        {
            return _pluginContexts.Select(p => p.Value).ToList();
        }

        public bool Any(string pluginName)
        {
            return _pluginContexts.ContainsKey(pluginName);
        }

        public IPluginContext Get(string pluginName)
        {
            return _pluginContexts[pluginName];

        }

        public void Remove(string pluginName)
        {
            if (_pluginContexts.ContainsKey(pluginName))
            {
                var context = _pluginContexts[pluginName];
                _pluginContexts.Remove(pluginName);
                PluginsContextChangeEvent?.Invoke(pluginName, context);
            }
        }

        public void AddFromLoadContext(CollectibleAssemblyLoadContext assemblyLoadContext)
        {
            var assembly = assemblyLoadContext.GetEntryPointAssembly();
            var pluginContext = assembly.GetExportedTypes().SingleOrDefault(p => p.GetInterfaces().Any(x => x.Name == nameof(IPluginContext)));

            if (pluginContext!=null)
            {
                IPluginContext obj = (IPluginContext)assembly.CreateInstance(pluginContext.FullName);

                Add(assemblyLoadContext.PluginName, obj);

            }
        }
    }
}
