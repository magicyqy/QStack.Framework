using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class MvcModuleSetup : IMvcModuleSetup
    {
        private readonly ApplicationPartManager _partManager;
        private readonly IReferenceLoader _referenceLoader = null;
        private readonly IPluginsAssemblyLoadContexts _pluginsLoadContexts;
        private readonly DynamicChangeTokenProvider _dynamicChangeTokenProvider;
        private readonly INotificationRegister _notificationRegister;
        private readonly PluginOptions _pluginOptions;
        public event ModuleChangeDelegate ModuleChangeEventHandler;
        readonly IWebHostEnvironment _env;
        readonly string _baseDirectory;
        public MvcModuleSetup(ApplicationPartManager partManager, IReferenceLoader referenceLoader,
            IPluginsAssemblyLoadContexts pluginsLoadContexts, DynamicChangeTokenProvider dynamicChangeTokenProvider,
             INotificationRegister notificationRegister,IOptions<PluginOptions> options, IWebHostEnvironment webHostEnvironment)
        {
            _partManager = partManager;
            _referenceLoader = referenceLoader;
            _pluginsLoadContexts = pluginsLoadContexts;
            _dynamicChangeTokenProvider = dynamicChangeTokenProvider;
            _notificationRegister = notificationRegister;
            _pluginOptions = options.Value;
            _env = webHostEnvironment;
            _baseDirectory = AppContext.BaseDirectory;


        }

        public void LoadModule(string moduleName,bool isInstall=true)
        {
            if (!_pluginsLoadContexts.Any(moduleName))
            {
                CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(moduleName);

                string filePath = Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName, $"{moduleName}.dll");
                string referenceFolderPath = Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName);
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    Assembly assembly = context.LoadFromStream(fs);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                    context.SetEntryPoint(assembly);
                    context.PluginAssemblyParts.Add(new PluginAssemblyPart(assembly));
                    if(!AdditionalReferencePathHolder.AdditionalReferencePaths.Contains(filePath))
                         AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);

               
                   
                }
                var viewsFilePath = Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName, $"{moduleName}.Views.dll");
                if (File.Exists(viewsFilePath))
                {
                    using (FileStream fs = new FileStream(viewsFilePath, FileMode.Open))
                    { 
                        Assembly assembly = context.LoadFromStream(fs); 
                        context.PluginAssemblyParts.Add(new CompiledRazorAssemblyPart(assembly));
                        _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);                      
                      
                    }
                }
                var pluginWebRoot = Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName, $"wwwroot");
                DirectoryCopy(pluginWebRoot, _env.WebRootPath, true);
                _pluginsLoadContexts.Add(moduleName, context);
                if (isInstall)
                    ModuleChangeEventHandler?.Invoke(ModuleEvent.Installed, context);
            }

        }
        public void ReLoadModule(string moduleName,bool isReInstall = true)
        {
            if (_pluginsLoadContexts.Any(moduleName))
            {
                var context = _pluginsLoadContexts.Get(moduleName);
                foreach (var part in context.PluginAssemblyParts)
                    _partManager.ApplicationParts.Remove(part);
                _pluginsLoadContexts.Remove(moduleName);
              

            }
            LoadModule(moduleName, isReInstall);
        }

        public void EnableModule(string moduleName)
        {
            if (!_pluginsLoadContexts.Any(moduleName))
            {
                ReLoadModule(moduleName,false);
            }
            CollectibleAssemblyLoadContext context = _pluginsLoadContexts.Get(moduleName);
            foreach(var part in context.PluginAssemblyParts)
            {
                _partManager.ApplicationParts.Add(part);
            }           
             
            context.Enable();
            //_pluginContextContainer.AddFromLoadContext(context);
            _notificationRegister.RegisterFrom(_pluginsLoadContexts.Get(moduleName));        
            ResetControllActions();
            ModuleChangeEventHandler?.Invoke(ModuleEvent.Started, context);
        }

        public void DisableModule(string moduleName)
        { 
            var context = _pluginsLoadContexts.Get(moduleName);
            foreach(var part in context.PluginAssemblyParts)
                _partManager.ApplicationParts.Remove(part);
            context.Disable();
            //_pluginContextContainer.Remove(moduleName);
            ModuleChangeEventHandler?.Invoke(ModuleEvent.Stoped, context);
            _notificationRegister.UnRegisterFrom(context);
            ResetControllActions();
        }

        public void DeleteModule(string moduleName)
        {
            var context = _pluginsLoadContexts.Get(moduleName);
            if (context != null)
            {
                _pluginsLoadContexts.Remove(moduleName);

            }
            ModuleChangeEventHandler?.Invoke(ModuleEvent.UnInstalled, context);
            //删除静态文件
            DirectoryInfo staticDirectory = new DirectoryInfo(Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName, $"wwwroot"));
            if (staticDirectory.Exists)
            {
                foreach (var item in staticDirectory.GetDirectories())
                 {
                      DirectoryInfo tempdir = new DirectoryInfo(Path.Combine(_env.WebRootPath, item.Name));
                      if(tempdir.Exists)
                         tempdir.Delete(true);

                 }
            }
            
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, moduleName));
            directory.Delete(true);
           
        }

        private void ResetControllActions()
        {
            _dynamicChangeTokenProvider.NotifyChanges();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return;
                //throw new DirectoryNotFoundException(
                //    "Source directory does not exist or could not be found: "
                //    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
