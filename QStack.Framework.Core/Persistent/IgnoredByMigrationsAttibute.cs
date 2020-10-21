using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Core.Persistent
{
    [AttributeUsage(AttributeTargets.Class  | AttributeTargets.Interface, Inherited = true)]
    public class IgnoredByMigrationsAttribute:Attribute
    {
    }
}
