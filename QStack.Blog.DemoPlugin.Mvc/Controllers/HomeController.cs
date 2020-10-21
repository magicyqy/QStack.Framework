using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QStack.Blog.DemoPlugin.Mvc.Controllers
{
    [Area(DemoPluginContext.Area)]
    //[Route("DemoPlugin/[controller]/[action]/{id?}")]
    public class HomeController : Controller
    {
        ITestService _testService;
        public HomeController(ITestService testService)
        {
            _testService = testService;
        }
        public IActionResult Index()
        {
            return Ok($"{this.GetType().AssemblyQualifiedName}  is running.");
        }
        public async Task<IActionResult> Save()
        {
            await _testService.SaveMessage(new Models.TestModelDto { Message = "SSS" });

            return Ok("ok");
        }
    }
}
