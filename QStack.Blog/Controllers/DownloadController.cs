using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using QStack.Framework.Basic.IServices;

namespace QStack.Blog.Controllers
{
    public class DownloadController : Controller
    {
        readonly IProductDownloadService _productDownloadService;
        LinkParser _linkParser;
        public DownloadController(IProductDownloadService service, LinkParser linkParser)
        {
            this._productDownloadService = service;
            _linkParser = linkParser;
        }
        [Route("/[controller]/p/{id:int}")]
        public async Task<IActionResult> DownProductSource(int id,string gid ="")
        {

            var dataItem = await _productDownloadService.GetProductDownload(gid);

            if(dataItem == null)
                return NotFound();
            if(!Request.Headers.TryGetValue("Referer",out Microsoft.Extensions.Primitives.StringValues value))
            {
                return NotFound();
            }
            if(!value.ToString().ToLower().StartsWith(Request.GetHostUri().ToLower()))
                return NotFound();
            Regex r = new Regex("\\d+");
            MatchCollection mc = r.Matches(new Uri(value).LocalPath);
            if (mc.Count <=0)
                return NotFound();
            if (mc[0].Value!=dataItem.ProductId.ToString())
                return NotFound();
           
            //var referRoute = _linkParser.ParsePathByEndpointName("Shop_Detail_Tmp", pathString);
            //var cookie =Convert.ToInt32(Request.Cookies["Qy.produceId"]);
            //if(!cookie.Equals(id))
            //    return NotFound();
            return Redirect(dataItem.DownloadUrl);
        }

    }
}