
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using QStack.Framework.AspNetCore.Plugin.Core;
using QStack.Framework.AspNetCore.Plugin.IServices;
using QStack.Framework.AspNetCore.Plugin.Services;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.Core.Persistent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QStack.Framework.AspNetCore.Plugin.Extensions
{

    public static class PluginExtension
    {

        public static void PluginSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.GetValue<bool>("PluginOptions:Enable", false))
                return;
            services.Configure<PluginOptions>(options => configuration.GetSection("PluginOptions").Bind(options));

            services.AddSingleton<IMvcModuleSetup, MvcModuleSetup>();
            services.AddScoped<IPluginManagerService, PluginManagerService>();


            services.AddSingleton<INotificationRegister, NotificationRegister>();
            services.AddSingleton<DynamicChangeTokenProvider>()
                               .AddSingleton<IActionDescriptorChangeProvider>(provider => provider.GetRequiredService<DynamicChangeTokenProvider>());
            services.AddSingleton<IReferenceContainer, DefaultReferenceContainer>();
            services.AddSingleton<IReferenceLoader, DefaultReferenceLoader>();
            services.AddSingleton<IPluginsAssemblyLoadContexts, PluginsAssemblyLoadContexts>();

        }

        public static IApplicationBuilder UsePlugin(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            PluginOptions _pluginOptions = serviceProvider.GetService<IOptions<PluginOptions>>().Value;
            if (!_pluginOptions.Enable)
                return applicationBuilder;
            MvcRazorRuntimeCompilationOptions option = serviceProvider.GetService<IOptions<MvcRazorRuntimeCompilationOptions>>()?.Value;

            var pluginsLoadContexts = serviceProvider.GetService<IPluginsAssemblyLoadContexts>();
            //AssemblyLoadContextResoving(pluginsLoadContexts);
            IReferenceLoader loader = serviceProvider.GetService<IReferenceLoader>();
            var moduleSetup = serviceProvider.GetService<IMvcModuleSetup>();
            IPluginManagerService pluginManager = serviceProvider.GetService<IPluginManagerService>();
            List<PluginInfoDto> allEnabledPlugins = pluginManager.GetAllEnabledPlugins().ConfigureAwait(false).GetAwaiter().GetResult();

            ModuleChangeDelegate moduleStarted = (moduleEvent, context) =>
            {

                if (moduleEvent != ModuleEvent.Started || context?.PluginContext == null || context?.PluginContext.PluginEntityAssemblies.Count() == 0)
                    return;

                //var pluginContexts = pluginsLoadContexts.All().Select(c => c.PluginContext).SkipWhile(c => c == null);

                #region 加载插件实体类
                var daoFactories = serviceProvider.GetServices<IDaoFactory>();
                foreach (var factory in daoFactories)
                {
                    if (context.PluginContext.PluginEntityAssemblies.ContainsKey(factory.FactoryName))
                    {

                        factory.AddExtraEntityAssemblies(context.PluginContext.PluginEntityAssemblies[factory.FactoryName].ToArray());

                    }

                }

                #endregion

            };
            moduleSetup.ModuleChangeEventHandler += moduleStarted;

            foreach (var plugin in allEnabledPlugins)
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pluginOptions.InstallBasePath, plugin.Name, $"{ plugin.Name}.dll");

                option.AdditionalReferencePaths.Add(filePath);
                moduleSetup.EnableModule(plugin.Name);
            }
            moduleSetup.ModuleChangeEventHandler -= moduleStarted;
            AdditionalReferencePathHolder.AdditionalReferencePaths = option?.AdditionalReferencePaths;
            var razorViewEngineOptions = serviceProvider.GetService<IOptions<RazorViewEngineOptions>>()?.Value;

            razorViewEngineOptions?.AreaViewLocationFormats.Add("/Plugins/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            razorViewEngineOptions?.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");


            return applicationBuilder;
        }

        //had remove to program.cs
        private static void AssemblyLoadContextResoving(IPluginsAssemblyLoadContexts pluginsLoadContexts)
        {
            //AssemblyLoadContext.Default.Resolving += (context, assembly) =>
            //{
            //    assembly.Version = assembly.Version ?? new Version("0.0.0.0");
            //    Func<AssemblyLoadContext, bool> filter = p => p.Assemblies.Any(p => p.GetName().Name == assembly.Name
            //                                            && p.GetName().Version == assembly.Version);

            //    if (pluginsLoadContexts.All().Any(filter))
            //    {
            //        var assemblyLoadContext = pluginsLoadContexts.All().First(filter);
            //        Assembly ass = assemblyLoadContext.Assemblies.First(p => p.GetName().Name == assembly.Name
            //            && p.GetName().Version == assembly.Version);
            //        return ass;
            //    }
            //    if(AssemblyLoadContext.All.Any(filter))
            //    {
            //        var assemblyLoadContext = AssemblyLoadContext.All.First(filter);
            //        Assembly ass = assemblyLoadContext.Assemblies.First(p => p.GetName().Name == assembly.Name
            //            && p.GetName().Version == assembly.Version);
            //        return ass;
            //    }
            //    return null;
            //};
        }


    }
}
