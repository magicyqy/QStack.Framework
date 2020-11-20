using QStack.Web.Comon;
using QStack.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QStack.Framework.Core.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace QStack.Web.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        ILogger<ExceptionHandlerMiddleware> _logger;
        ICompositeViewEngine viewEngine;
        IWebHostEnvironment environment;
        ITempDataProvider tempDataProvider;
        //IJsonHelper jsonHelper;
        JsonOptions jsonOptions;
        private readonly string[] apiPrefixs = new string[] { "/api"};

        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment,
            ICompositeViewEngine engine,
            ITempDataProvider tempDataProvider,
            IJsonHelper jsonHelper,IOptions<JsonOptions> options)
        {
            _next = next;
            _logger = logger;
            viewEngine = engine;
            this.environment = environment;
            this.tempDataProvider = tempDataProvider;
            //this.jsonHelper = jsonHelper;
          
            jsonOptions = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            
            var exceptionDetails = context.Features.Get<IExceptionHandlerPathFeature>();
            var ex = exceptionDetails?.Error;
            var isApiRequest = false;
            if (ex == null)
            {
                try
                {
                    await _next(context);
                }
                catch(Exception e)
                {
                    _logger.LogException(e);
                    ex = e;
                    var endpoint = context.GetEndpoint();
                    var apiAcitonDescriptor = endpoint?.Metadata.GetOrderedMetadata<ApiControllerAttribute>();
                    if (apiAcitonDescriptor != null)
                        isApiRequest = true;
                }
            }
            var exception = new HttpException((int)HttpStatusCode.InternalServerError);
            if(ex is UnauthorizedAccessException)
            {
                ex = new HttpException((int)HttpStatusCode.Unauthorized);

            }
            if (ex is HttpException)
            {
                exception = (HttpException)ex;
            }



            var originPath = exceptionDetails?.Path ?? context.Request.Path;
            if(isApiRequest||apiPrefixs?.Count(p=>originPath.StartsWith(p))>0)
                await HandleApiException(context, exception);
            else
                await HandleMvcException(context, exception);


        }

        private async Task HandleApiException(HttpContext context, HttpException ex)
        {
            if(!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorViewModel
                {
                    StatusCode = ex.StatusCode,
                    ErrorMessage = ex.Message 
                },jsonOptions.JsonSerializerOptions).ToString());
            }
        }

        private async  Task HandleMvcException(HttpContext context, HttpException ex)
        {
            var viewModel = new ErrorViewModel
            {
                StatusCode = ex.StatusCode,
                ErrorMessage = ex.Message
            };
            var viewResult = viewEngine.GetView("~/", "~/Views/Shared/Error.cshtml", true);
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
                            Model = viewModel
                        },
                        TempData = new TempDataDictionary(context, tempDataProvider), //ViewData
                        View = viewResult.View,
                        FormContext = new FormContext(),
                        ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    };
                    //渲染
                    try
                    {
                      await viewResult.View.RenderAsync(viewContext);
                    }
                    catch(Exception e)
                    {
                        var msg = e.Message;
                    }
                  
                    var html = output.ToString();
                    context.Response.ContentType = "text/html";
                    //输出到响应体
                    await context.Response.WriteAsync(html);
                }
            }
            else
                await context.Response.WriteAsync(JsonSerializer.Serialize(viewModel,jsonOptions.JsonSerializerOptions).ToString());
        }
    }
}
