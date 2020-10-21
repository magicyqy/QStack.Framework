using System.Collections.Generic;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public interface IPluginsAssemblyLoadContexts
    {
      

        void Add(string pluginName, CollectibleAssemblyLoadContext context);
        List<CollectibleAssemblyLoadContext> All();
        bool Any(string pluginName);
        CollectibleAssemblyLoadContext Get(string pluginName);
        void Remove(string pluginName);


    }
}