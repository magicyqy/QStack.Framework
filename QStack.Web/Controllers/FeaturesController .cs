using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using QStack.Framework.Basic.IServices;
using QStack.Framework.AspNetCore.Plugin.Core;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace QStack.Web.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly ApplicationPartManager _partManager;
        DynamicChangeTokenProvider _dynamicChangeTokenProvider;

        public FeaturesController(ApplicationPartManager partManager, DynamicChangeTokenProvider dynamicChangeTokenProvider)
        {
            _partManager = partManager;
            _dynamicChangeTokenProvider = dynamicChangeTokenProvider;
        }

        public IActionResult Index()
        {
            var viewModel = new FeaturesViewModel();

            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);
            viewModel.Controllers = controllerFeature.Controllers.ToList();

            var tagHelperFeature = new TagHelperFeature();
            _partManager.PopulateFeature(tagHelperFeature);
            viewModel.TagHelpers = tagHelperFeature.TagHelpers.ToList();

            var viewComponentFeature = new ViewComponentFeature();
            _partManager.PopulateFeature(viewComponentFeature);
            viewModel.ViewComponents = viewComponentFeature.ViewComponents.ToList();

            var feature = new ViewsFeature();
            _partManager.PopulateFeature(feature);
            viewModel.Views = feature.ViewDescriptors.Select(v => v.Type.GetTypeInfo()).ToList();
            return View(viewModel);
        }

        public IActionResult Install()
        {
            _partManager.ApplicationParts.Add(new AssemblyPart(Assembly.LoadFrom("Plugins/QStack.Framework.AspNetCore.Plugin/QStack.Framework.AspNetCore.Plugin.dll")));
            _dynamicChangeTokenProvider.NotifyChanges();
            return Ok("OK");
        }

        public IActionResult Entities([FromServices]IDataAuthService dataAuthService)
        {
            var datas = dataAuthService.GetDaoFactoryEntityInfo();

            return Json(datas.Values.SelectMany(v=>v).Select(v=>v.EntityName));
        }

        public IActionResult DefaultAssemblies()
        {
            GC.Collect();
            var dynamicAssemblies= System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies.Where(a => a.FullName.Contains("AspectCore.DynamicProxy.Generator"));
            var names=System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies.Select(a => a.FullName).OrderBy(n=>n);

            return Ok(names.Count()+"\r\n"+string.Join("\r\n", names));
        }

        public IActionResult AllAppContext()
        {
            var contexts = System.Runtime.Loader.AssemblyLoadContext.All;
            StringBuilder sb = new StringBuilder();
            foreach(var context in contexts )
            {
                sb.AppendLine(context.Name);
                sb.Append(string.Join("\r\n", context.Assemblies.Select(a => a.FullName)));
                sb.AppendLine();
                sb.AppendLine();
            }

            return Ok(sb.ToString());
        }
    }


        public class FeaturesViewModel
        {
            public List<TypeInfo> Controllers { get; set; }

            public List<TypeInfo> TagHelpers { get; set; }

            public List<TypeInfo> ViewComponents { get; set; }
            
            public List<TypeInfo> Views { get; set; }
        }

}
