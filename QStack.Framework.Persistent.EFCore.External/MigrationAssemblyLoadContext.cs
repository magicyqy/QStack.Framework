using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;

namespace QStack.Framework.Persistent.EFCore.External
{
    public class MigrationAssemblyLoadContext:AssemblyLoadContext
    {
        public MigrationAssemblyLoadContext():base(isCollectible: true)
        {

        }
    }
}
