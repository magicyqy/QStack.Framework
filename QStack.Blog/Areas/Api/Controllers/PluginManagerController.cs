using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using QStack.Blog.Areas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.AspNetCore.Plugin.IServices;
using QStack.Framework.AspNetCore.Plugin.Core;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using QStack.Framework.Basic;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class PluginManagerController : ApiBaseController
    {
        IPluginManagerService _pluginManager;
       
        PluginPackageManager _pluginPackageManager;
        public PluginManagerController(IPluginManagerService pluginManager,PluginPackageManager pluginPackageManager)
        {
            _pluginManager = pluginManager;
            _pluginPackageManager = pluginPackageManager;
           
         }
       
 
        public async Task<ResponseResult> Install([FromForm] string package,[FromForm]bool IsMigration)
        {
            var result = new ResponseResult();
            if (package == null)
            {
                result.Code = BusinessCode.Params_Error;
                result.Message = nameof(BusinessCode.Params_Error);
                return result;

            }
            var contentRootPath = HttpContext.RequestServices.GetService<IHostEnvironment>().ContentRootPath;
            var fullpath = Path.Combine(contentRootPath, package.Trim().TrimStart('/'));
           
            if(_pluginPackageManager.UnZipPackage(fullpath, out PluginInfoDto pluginInfo))
            {
                pluginInfo.IsMigration = IsMigration;
                await _pluginManager.AddPlugins(pluginInfo);
            }
            else
            {
                result.Code = 500;
                result.Message = "unzip error";
            }

            //using (FileStream fs = new FileStream(fullpath, FileMode.Open))
            //{
            //    var pluginPackage = new PluginPackage(fs, fullpath, pluginOptions);
            //    pluginPackage.SetupFolder();
            //    pluginPackage.PluginInfo.IsMigration = IsMigration;
            //    await _pluginManager.AddPlugins(pluginPackage.PluginInfo);
            //}
            return result;
        }

    
        public async Task<ResponseResult> Enable(int id)
        {
            var result = new ResponseResult();
            var pluginInfoDto = await _pluginManager.QueryById<PluginInfoDto>(id);
            if (pluginInfoDto == null)
            {
                result.Code = BusinessCode.Record_NotFound;
                result.Message = nameof(BusinessCode.Record_NotFound);
                return result;
            }
            await _pluginManager.EnablePlugin(id);

            return new ResponseResult();
        }
  
        public async Task<ResponseResult> Disable(int id)
        {
            var result = new ResponseResult();
            await _pluginManager.DisablePlugin(id);
            return result;
        }

        public async Task<ResponseResult> Delete(int id)
        {
            var result = new ResponseResult();
            await _pluginManager.DeletePlugin(id);

            return result;
        }

        public async Task<ResponseResult<List<PluginInfoDto>>> GetPlugins()
        {
            var plugins = await _pluginManager.GetAllPlugins();

            return new ResponseResult<List<PluginInfoDto>>(plugins);
        }




    }
}
