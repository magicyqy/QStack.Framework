using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Util;
using QStack.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Comon
{
    public static class IMvcBuilderExtentions
    {
        /// <summary>
        /// 将mvc相关放进这里，然后作为库被引用时可复用
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IMvcBuilder AddCustomMvc(this IServiceCollection services)
        {
            var mvcBuilder= services.AddMvc(options => options.Filters.Add<TitleFilter>())
                .AddRazorRuntimeCompilation()
#if DEBUG
                //.AddApplicationPart(typeof(Docker.Crawler.CrawlerOptions).Assembly)
#endif
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                });

            var defaultActivator = services.FirstOrDefault(c => c.ServiceType == typeof(IControllerActivator));
            if (defaultActivator != null)
            {
                services.Remove(defaultActivator);
                services.AddSingleton<IControllerActivator, CustomServiceBasedControllerActivator>();
            }

            return mvcBuilder;
        }
    }
}
