using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Extensions
{
    public static class PluginRouteExtension
    {
        public static IApplicationBuilder UsePluginRoute(this IApplicationBuilder app)
        {
           
            app.UseEndpoints(routes =>
            {
               
                routes.MapControllerRoute(
                    name: "Customer",
                    pattern: "Plugins/{area}/{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}
