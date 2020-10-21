using Microsoft.AspNetCore.Mvc.ApplicationParts;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        private Assembly _entryPoint = null;
        private bool _isEnabled = false;
        private readonly string _pluginName = string.Empty;
        private  IPluginContext _pluginContext = null;
        private IList<ApplicationPart> _pluginAssemblyParts=new List<ApplicationPart>();

        public CollectibleAssemblyLoadContext(string pluginName) : base(isCollectible: true)
        {
            _pluginName = pluginName;
        }

        public string PluginName => _pluginName;

        public bool IsEnabled => _isEnabled;

        public IPluginContext PluginContext => _pluginContext;

        public IList<ApplicationPart> PluginAssemblyParts => _pluginAssemblyParts;

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;

        }

        public void SetEntryPoint(Assembly entryPoint)
        {
            _entryPoint = entryPoint;
            LoadPluginContext(_entryPoint);
        }

        private void LoadPluginContext(Assembly entryPoint)
        {
            var exportedTypes = entryPoint.GetExportedTypes();
            var pluginContext = entryPoint.GetExportedTypes().SingleOrDefault(p => p.GetInterfaces().Any(x => x.Name == nameof(IPluginContext)));

            if (pluginContext != null)
            {
                _pluginContext = (IPluginContext)entryPoint.CreateInstance(pluginContext.FullName);

            }
        }

        public Assembly GetEntryPointAssembly()
        {
            return _entryPoint;
        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }
}
