using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Configuration;
using Lucene.Net.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using QStack.Framework.AspNetCore.Plugin.Core;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QStack.Blog.Comon
{
    public class CustomServiceBasedControllerActivator : IControllerActivator
    {
        ILogger<CustomServiceBasedControllerActivator> _logger;

        static ConcurrentDictionary<string, IServiceProvider> _pluginProviders = new ConcurrentDictionary<string, IServiceProvider>();
        public CustomServiceBasedControllerActivator(ILogger<CustomServiceBasedControllerActivator> logger)
        {
            _logger = logger;
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
        /// <inheritdoc />
        public object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
            try
            {
                var controller = actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
                return controller;
            }
            catch
            {
                ///*
                try
                { 
                    var oldSsevices = actionContext.HttpContext.RequestServices.GetService<IServiceCollection>();
                    var myServices = oldSsevices.Where(s => s.ServiceType.FullName.Contains("QStack"));
                    var oldProvider = actionContext.HttpContext.RequestServices;

                    _pluginProviders.TryAdd(Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier, oldProvider);

                    var area = actionContext.RouteData.Values["area"]?.ToString().ToLower();
                     var assemblyLoadContext = oldProvider.GetService<IPluginsAssemblyLoadContexts>().All().SingleOrDefault(p => p.PluginContext.RouteArea?.ToLower() == area);
                    var newServices = assemblyLoadContext.ConfigureServices(oldSsevices, oldProvider);
                    var myServices1 = newServices.Where(s => s.ServiceType.FullName.Contains("QStack"));
                    //actionContext.HttpContext.RequestServices = newServiceProvider;
                    //foreach (var item in newServices.Select(s => s.ServiceType.FullName).OrderBy(n => n))
                    //    Console.WriteLine(item);
                    var serviceProvider = newServices.BuildServiceProvider();
                    newProviders.TryAdd(Activity.Current?.Id, serviceProvider);
                    var obj=serviceProvider.GetRequiredService(controllerType);
                    return obj;
                }
                catch(Exception e)
                {
                    _logger.LogException(e);
                   
                  
                    return null;
                }
                //*/
               /*
                try
                {

                    //if (newProvider != null)
                    //    return newProvider.GetRequiredService(controllerType);
                    var oldSsevices = actionContext.HttpContext.RequestServices.GetService<IServiceCollection>();
                    var oldProvider = actionContext.HttpContext.RequestServices;
                    _pluginProviders.TryAdd(Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier, oldProvider);
                    var newServices = new ServiceCollection();
                    newServices.AddOptions();
                    newServices.AddScoped(controllerType);
                    foreach (var service in oldSsevices)
                    {
                        Console.WriteLine(service.ServiceType.FullName);
                        if (service.ServiceType.IsGenericType && service.ServiceType.GenericTypeArguments.Length == 0)
                        {
                            newServices.Add(service);
                            continue;
                        }


                        if (service.Lifetime != ServiceLifetime.Transient)
                        {
                            var instance = oldProvider.GetRequiredService(service.ServiceType);

                            var instanceType = instance.GetType();
                            var canAddDirect = service.ImplementationType != null && (instanceType == service.ImplementationType || instanceType.IsAssignableFrom(service.ImplementationType));
                            var sourceType = typeof(ServiceDescriptor);
                            //if (canAddDirect)
                            //{
                            //    var scopedMethod = getMethod(sourceType, service.Lifetime.ToString(), new Type[] { service.ServiceType, service.ImplementationType }, new Type[] { typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), service.ImplementationType) });
                            //    var func = GetLambdaFunc(service.ImplementationType, service.ServiceType).Compile();
                            //    var serviceDesc = (ServiceDescriptor)scopedMethod.Invoke(null, new object[] { func });
                            //    newServices.Add(serviceDesc);
                            //}
                            //else
                            //{
                                if (service.Lifetime == ServiceLifetime.Singleton)
                                    newServices.AddSingleton(service.ServiceType, p => instance);
                                else
                                    newServices.AddScoped(service.ServiceType, p => instance);

                            //}
                        }    

                        //if (service.Lifetime == ServiceLifetime.Singleton)
                        //{
                        //    var instance = oldProvider.GetRequiredService(service.ServiceType);

                        //    var instanceType = instance.GetType();
                        //    var canAddDirect = service.ImplementationType != null && (instanceType == service.ImplementationType || instanceType.IsAssignableFrom(service.ImplementationType));
                        //    var sourceType = typeof(ServiceDescriptor);
                        //    if (canAddDirect)
                        //    {
                        //        var singletonMethod = getMethod(sourceType, nameof(ServiceDescriptor.Singleton), new Type[] { service.ServiceType, service.ImplementationType }, new Type[] { typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), service.ImplementationType) });
                        //        var func = GetLambdaFunc(service.ImplementationType, instance).Compile();
                        //        var serviceDesc = (ServiceDescriptor)singletonMethod.Invoke(null, new object[] { func });
                        //        newServices.Add(serviceDesc);
                        //    }
                        //    else
                        //        newServices.AddSingleton(service.ServiceType, p => instance);
                        //}
                        if (service.Lifetime == ServiceLifetime.Transient)
                        {
                            //if (canAddDirect)
                            //{
                            //    var transientMethod = getMethod(sourceType, nameof(ServiceDescriptor.Transient), new Type[] { service.ServiceType, service.ImplementationType }, new Type[] { typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), service.ImplementationType) });
                            //    var func = GetLambdaFunc(service.ImplementationType, instance).Compile();
                            //    var serviceDesc = (ServiceDescriptor)transientMethod.Invoke(null, new object[] { func });
                            //    newServices.Add(serviceDesc);
                            //}
                            //else
                           newServices.Add(service);

                        }
                    }
                    //sevices.AddSingleton<IAspectConfiguration>(actionContext.HttpContext.RequestServices.GetRequiredService<IAspectConfiguration>());
                    //sevices.AddScoped(controllerType);
                    var area = actionContext.RouteData.Values["area"]?.ToString().ToLower();
                    var pluginContext = oldProvider.GetService<IPluginsAssemblyLoadContexts>().All().Select(p=>p.PluginContext).SingleOrDefault(p => p.RouteArea?.ToLower() == area);
                    if (pluginContext != null)
                    {
                        #region 重置automapper配置

                        var profiles = pluginContext.AutoMapperProfiles;
                        var configExpression = new MapperConfigurationExpression();
                        configExpression.AddProfiles(profiles);
                        configExpression.AddAutoMaperConfig(typeof(UserDto).Assembly, typeof(PluginInfoDto).Assembly);

                        var configration = new MapperConfiguration(configExpression);
                        newServices.AddSingleton<IConfigurationProvider>(configration);
                        newServices.AddTransient<IMapper>(p => configration.CreateMapper());
                        #endregion
                        newServices.AddRange(pluginContext.Services);
                    }
              
                    newServices.AddSingleton<IProxyTypeGenerator, ProxyTypeGenerator>();
                    var newProvider = newServices.BuildDynamicProxyProvider();
                    newProviders.TryAdd(Activity.Current.Id, newProvider);
                    var controller = newProvider.GetRequiredService(controllerType);
                    return controller;
                }
                catch(Exception e)
                {
                    _logger.LogException(e);
                    Console.WriteLine(this.GetType().FullName);
                    return null;
                }
               */
            }

        }
        static Dictionary<string,IServiceProvider> newProviders=new Dictionary<string, IServiceProvider>();
        static object GetServiceProvider(Type serviceType)
        {
            return _pluginProviders.GetValueOrDefault(Activity.Current.Id).GetRequiredService(serviceType);
        }
        private LambdaExpression GetLambdaFunc(Type type, Type serviceType)
        {
            var method = typeof(CustomServiceBasedControllerActivator).GetMethod("GetServiceProvider", BindingFlags.Static| BindingFlags.NonPublic| BindingFlags.Public);
            var p = Expression.Parameter(typeof(IServiceProvider), "p");
            var contans = Expression.Convert(Expression.Call(method,
                            Expression.Constant(serviceType)
                        ),type);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), type);
            return Expression.Lambda(delegateType, contans, p);
        }
        private LambdaExpression GetLambdaFunc(Type type, object value)
        {

            var p = Expression.Parameter(typeof(IServiceProvider), "p");
            var contans = Expression.Constant(value);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), type);
            return Expression.Lambda(delegateType, contans, p);
        }
        /// <inheritdoc />
        public virtual void Release(ControllerContext context, object controller)
        {

            var provider = newProviders.GetValueOrDefault(Activity.Current.Id) as ServiceProvider;
            provider?.Dispose();
            GC.Collect();

        }
    }
}
