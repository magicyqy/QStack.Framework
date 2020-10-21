using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class PluginAssemblyPart : AssemblyPart, ICompilationReferencesProvider
    {
        public PluginAssemblyPart(Assembly assembly) : base(assembly) { }

        public IEnumerable<string> GetReferencePaths()
        {
            return Array.Empty<string>();
        }
    }
}
