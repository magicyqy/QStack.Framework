using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QStack.Framework.Basic.IServices
{
    public static class ServiceExtension
    {
        /// <summary>
        /// 加载继承IBaseService的服务类进DI容器
        /// TODO:后续需要再细化
        /// </summary>
        /// <param name="services">容器</param>
        /// <param name="assemblies">服务类所在程序集</param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services,IEnumerable<Assembly> assemblies)
        {
           
            if (assemblies == null)
                throw new ArgumentNullException("services assemblies is null");
            foreach(var assembly in assemblies)
            {
                var types=assembly.GetTypes().Where(p => typeof(IBaseService).IsAssignableFrom(p)).ToList();

                foreach(var type in types)
                {
                    if (type.IsAbstract) continue;
                    
                    var interfaces = type.GetInterfaces().Where(i => i!= typeof(IBaseService));                    
                    
                    foreach (var _interface in interfaces)
                        services.AddTransient(_interface,type);
                }
            }
            return services;
        }
    }
}
