using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Middleware
{
    /// <summary>
    /// 为serilog注入httpcontext的相关信息
    /// </summary>
    public class SerilogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            // Add user data to logging context       
            using (LogContext.PushProperty("UserId", context.User?.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value))
            using (LogContext.PushProperty("IP", context.Connection.RemoteIpAddress))
            {
                await _next.Invoke(context);
            }
        }
    }
}
