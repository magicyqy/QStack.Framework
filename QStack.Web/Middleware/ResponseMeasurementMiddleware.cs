using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QStack.Web.Middleware
{
    /// <summary>
    /// 修改response body的内容<br/>
    /// 
    /// 建议放在中间件顺序的首位。
    /// </summary>
    public class ResponseMeasurementMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 指定匹配的的路由路径或路径正则，若包含"*"则会修改所有路径返回的内容
        /// </summary>
        private List<string> _injectRouteRegs = new List<string> { @"^/tools/[\s\S]+" };
        ICompositeViewEngine _viewEngine;
        ITempDataProvider _tempDataProvider;
        public ResponseMeasurementMiddleware(RequestDelegate next, ICompositeViewEngine engine, ITempDataProvider tempDataProvider)
        {
            _next = next;
            _viewEngine = engine; ;
            _tempDataProvider = tempDataProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            
            if(!MatchRoute(context.Request.Path))
            {
                await _next(context);
                return;
            }
        
            context.Response.OnStarting(async () =>
            {
                if (IsHtmlResponse(context))
                {
                    context.Response.Headers.ContentLength = null;
                }
                   
             
                
                await Task.CompletedTask;
            });
            var originalBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;

            var watch = new Stopwatch();
            long responseTime = 0;
            watch.Start();
            await _next(context);
            //// read the new body
            // read the new body

            if (IsHtmlResponse(context))
            {
                responseTime = watch.ElapsedMilliseconds;
                newBody.Position = 0;
                var newContent = await new StreamReader(newBody).ReadToEndAsync();
                // calculate the updated html
                var updatedHtml =await CreateDataNode(newContent, context);
                // set the body = updated html
                using (var updatedStream = GenerateStreamFromString(updatedHtml))
                {
                    await updatedStream.CopyToAsync(originalBody);
                }


                context.Response.Body = originalBody;
                //context.Response.Headers.ContentLength = null;
            }
            else
            {
                newBody.Position = 0;
                await newBody.CopyToAsync(originalBody);

                context.Response.Body = originalBody;
            }
        

        }

        private bool MatchRoute(string path)
        {
            if (!Path.GetExtension(path).IsNullOrWhiteSpace())
                return false;
            if (this._injectRouteRegs.Any(p => "*".Equals(p)))
                return true;
            
            return this._injectRouteRegs.Any(p => Regex.IsMatch(path,p));
            
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
       
        private async Task<string> CreateDataNode(string originalHtml, HttpContext context)
        {
            var html = string.Empty;
            var viewResult = _viewEngine.GetView("~/", "~/Views/Shared/_ToolLayout.cshtml", true);
            if (viewResult.Success)
            {
                //创建临时的StringWriter实例，用来配置到视图上下文中
                using (var output = new StringWriter())
                {
                    //视图上下文对于视图渲染来说很重要，视图中的前后台交互都需要它
                    var viewContext = new ViewContext()
                    {
                        HttpContext = context,
                        Writer = output,
                        RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                        {

                        },
                        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                        {
                            Model = null
                        },
                        TempData = new TempDataDictionary(context, _tempDataProvider), //ViewData
                        View = viewResult.View,
                        FormContext = new FormContext(),
                        ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    };
                    //渲染
                    await viewResult.View.RenderAsync(viewContext);
                    html = output.ToString();
                    
                }
            }
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(originalHtml);
            HtmlNode testNode = HtmlNode.CreateNode(html);
            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
            htmlBody.InsertBefore(testNode, htmlBody.FirstChild);

            string rawHtml = htmlDoc.DocumentNode.OuterHtml; //using this results in a page that displays my inserted HTML correctly, but duplicates the original page content.
                                                             //rawHtml = "some text"; uncommenting this results in a page with the correct format: this text, followed by the original contents of the page

            return rawHtml;
        }

        private bool IsHtmlResponse(HttpContext context)
        {


            return context.Response.StatusCode == 200 &&
                 context.Response.ContentType != null &&
                 context.Response.ContentType.Contains("text/html", StringComparison.OrdinalIgnoreCase);

            
        }
    }
}
