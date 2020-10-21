using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class NotificationRegister : INotificationRegister
    {
        private static readonly Dictionary<string, List<INotificationHandler>>
            _containers = new Dictionary<string, List<INotificationHandler>>();

        public void Publish(string eventName, string data)
        {
            if (_containers.ContainsKey(eventName))
            {
                foreach (INotificationHandler item in _containers[eventName])
                {
                    item.Handle(data);
                }
            }
        }

        public void Subscribe(string eventName, INotificationHandler handler)
        {
            if (_containers.ContainsKey(eventName))
            {
                _containers[eventName].Add(handler);
            }
            else
            {
                _containers[eventName] = new List<INotificationHandler>() { handler };
            }
        }

        public void RegisterFrom(CollectibleAssemblyLoadContext assemblyLoadContext)
        {
            var assembly = assemblyLoadContext.GetEntryPointAssembly();
            IEnumerable<Type> providers = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x.Name == "INotificationProvider"));

            if (providers.Any())
            {


                foreach (Type p in providers)
                {
                    INotificationProvider obj = (INotificationProvider)assembly.CreateInstance(p.FullName);
                    Dictionary<string, List<INotificationHandler>> result = obj.GetNotifications();

                    foreach (KeyValuePair<string, List<INotificationHandler>> item in result)
                    {
                        foreach (INotificationHandler i in item.Value)
                        {
                            
                            Subscribe(item.Key, i);
                            
                        }
                    }
                }
            }
        }
  
        public void UnRegisterFrom(CollectibleAssemblyLoadContext assemblyLoadContext)
        {
            var assembly = assemblyLoadContext.GetEntryPointAssembly();
            foreach(var kp in _containers)
               kp.Value.RemoveAll(handler => handler.GetType().Assembly.GetName() == assembly.GetName());
        }
    }
}
