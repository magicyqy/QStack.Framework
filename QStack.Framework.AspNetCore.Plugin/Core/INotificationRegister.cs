using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public interface INotificationRegister
    {
        void Subscribe(string eventName, INotificationHandler handler);

        void Publish(string eventName, string data);
        void UnRegisterFrom(CollectibleAssemblyLoadContext assemblyLoadContext);
        void RegisterFrom(CollectibleAssemblyLoadContext assemblyLoadContext);
    }
}
