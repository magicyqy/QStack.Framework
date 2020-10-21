using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class PluginsAssemblyLoadContexts : IPluginsAssemblyLoadContexts
    {
        private readonly Dictionary<string, CollectibleAssemblyLoadContext> _pluginAssemblyLoadContext = null;
        private readonly IPluginContextContainer _pluginContextContainer;
        public PluginsAssemblyLoadContexts()
        {
          
            _pluginAssemblyLoadContext = new Dictionary<string, CollectibleAssemblyLoadContext>();
        }
       
        public List<CollectibleAssemblyLoadContext> All()
        {
            return _pluginAssemblyLoadContext.Select(p => p.Value).ToList();
        }

        public bool Any(string pluginName)
        {
            return _pluginAssemblyLoadContext.ContainsKey(pluginName);
        }

        public void Remove(string pluginName)
        {
           
            if (_pluginAssemblyLoadContext.ContainsKey(pluginName))
            {
                
                _pluginAssemblyLoadContext[pluginName].Unload();
               
                _pluginAssemblyLoadContext.Remove(pluginName);
               
            }
           
        }

        public CollectibleAssemblyLoadContext Get(string pluginName)
        {
            return _pluginAssemblyLoadContext[pluginName];
        }

        public void Add(string pluginName, CollectibleAssemblyLoadContext context)
        {
            _pluginAssemblyLoadContext.Add(pluginName, context);
            
        }
    }
}
