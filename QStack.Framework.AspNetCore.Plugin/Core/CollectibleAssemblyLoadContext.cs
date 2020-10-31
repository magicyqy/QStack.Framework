using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.Basic.Services;
using QStack.Framework.Basic.ViewModel.Auth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
        private IPluginContext _pluginContext = null;
        private IList<ApplicationPart> _pluginAssemblyParts = new List<ApplicationPart>();

        public CollectibleAssemblyLoadContext(string pluginName) : base(pluginName,isCollectible: true)
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

        public IEnumerable<Type> Controllers 
        {
            get
            {
               var types= _pluginAssemblyParts.Where(p => typeof(PluginAssemblyPart).IsAssignableFrom(p.GetType())).Select(a=>(a as AssemblyPart).Assembly).SelectMany(a=>a.GetTypes()).Where(t => typeof(ControllerBase).IsAssignableFrom(t));
               return types;
            }
        }
        private IServiceCollection _services;
        public IServiceCollection ConfigureServices(IServiceCollection oldServices, IServiceProvider oldServiceProvider)
        {
            if (_pluginContext.Services?.Count() > 0)
            {

                var newServices = oldServices;
                if (_services != null)
                {
                    newServices = _services;
                }
                var services = new ServiceCollection();
                services.AddOptions();

                //添加基础service
                foreach (var service in newServices)
                {
                    //Console.WriteLine(service.ServiceType.FullName);
                    //if (_pluginContext.Services.Any(s => s.ServiceType == service.ServiceType))
                    //    continue;
                    //if (Controllers.Any(s => s == service.ServiceType))
                    //    continue;
                    if (service.ServiceType.IsGenericType && service.ServiceType.GenericTypeArguments.Length == 0)
                    {
                        services.Add(service);
                        continue;
                    }


                    if (service.Lifetime != ServiceLifetime.Transient)
                    {
                        var instance = oldServiceProvider.GetService(service.ServiceType);
                        if(instance==null)
                        {
                            services.Add(service);
                            continue;
                        }
                        var instanceType = instance.GetType();
                        var canAddDirect = service.ImplementationType != null && (instanceType == service.ImplementationType || instanceType.IsAssignableFrom(service.ImplementationType));
                        var sourceType = typeof(ServiceDescriptor);
                        if (canAddDirect)
                        {
                            var scopedMethod = getMethod(sourceType, service.Lifetime.ToString(), new Type[] { service.ServiceType, service.ImplementationType }, new Type[] { typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), service.ImplementationType) });
                            var func = GetLambdaFunc(service.ImplementationType, instance).Compile();
                            var serviceDesc = (ServiceDescriptor)scopedMethod.Invoke(null, new object[] { func });
                            services.Add(serviceDesc);
                        }
                        else
                        {
                            if (service.Lifetime == ServiceLifetime.Singleton)
                                services.AddSingleton(service.ServiceType, p => instance);
                            else
                                services.AddScoped(service.ServiceType, p => instance);

                        }
                    }
                                       
                    if (service.Lifetime == ServiceLifetime.Transient)
                    {                       
                        services.Add(service);
                    }
                }

                if (_services == null)
                {
                    #region 重置automapper配置
                    var profiles = _pluginContext.AutoMapperProfiles;
                    var configExpression = new MapperConfigurationExpression();
                    configExpression.AddProfiles(profiles);
                    configExpression.AddAutoMaperConfig(typeof(UserDto).Assembly, typeof(PluginInfoDto).Assembly);

                    var configration = new MapperConfiguration(configExpression);
                    services.AddSingleton<IConfigurationProvider>(configration);
                    services.AddTransient<IMapper>(p => configration.CreateMapper());
                    #endregion
                    foreach (var item in _pluginContext.Services)
                        services.Add(item);
                    //添加plugin controllers                   
                    foreach (var type in Controllers)
                        services.AddScoped(type);
                  
                    services.AddSingleton<IProxyTypeGenerator, ProxyTypeGenerator>();
                    _services = services.WeaveDynamicProxyService();
                }
                else
                    _services = services;

                return _services;

            }

            return oldServices;
        }

        private LambdaExpression GetLambdaFunc(Type type,object value)
        {
           
            var p = Expression.Parameter(typeof(IServiceProvider), "p");
           
            var contans =Expression.Constant(value);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), type);
            return Expression.Lambda(delegateType, contans, p);
        }
        Func<Type, string, Type[], Type[], MethodInfo> getMethod = (t, n, genargs, args) =>
        {
            var methods = t.GetMethods();
            var result = methods.Where(m => m.Name == n && m.GetGenericArguments().Length == genargs.Length);
            result = result.Select(m => m.IsGenericMethodDefinition ? m.MakeGenericMethod(genargs) : m);
            foreach (var mg in result)
            {
                var parmeterType = mg.GetParameters().Select(p => p.ParameterType);
                var b = parmeterType.SequenceEqual(args);
            }
            methods = result.Where(mg => mg.GetParameters().Select(p => p.ParameterType).SequenceEqual(args)).ToArray();


            return methods.Single();
        };
    }
}
