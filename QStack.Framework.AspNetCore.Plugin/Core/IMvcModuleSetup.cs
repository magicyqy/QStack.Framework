using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public enum ModuleEvent
    {
        Installed,
        Started,
        Stoped,
        UnInstalled
    }
    public delegate void ModuleChangeDelegate(ModuleEvent eventType,CollectibleAssemblyLoadContext assemblyLoadContext);
    public interface IMvcModuleSetup
    {
        event ModuleChangeDelegate ModuleChangeEventHandler;
        void DisableModule(string moduleName);
        /// <summary>
        /// reload a module but not enable it after loaded
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="isReInstall">determine whether upgrade or degrade</param>
        void ReLoadModule(string moduleName, bool isReInstall = true);
        /// <summary>
        /// load a module but not enable it after loaded
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="isInstall">determine whether install operation</param>
        void LoadModule(string moduleName, bool isInstall = true);

        /// <summary>
        /// enable a module,if the module not exist it will reload the module
        /// </summary>
        /// <param name="moduleName"></param>
        void EnableModule(string moduleName);


        void DeleteModule(string moduleName);
    }
}
