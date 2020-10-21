using AspectCore.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Core.Cache;
using QStack.Framework.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QStack.Blog.Comon
{
    public static class AspectInterceptorExtensions
    {
        /// <summary>
        /// 添加全局aop拦截器
        /// </summary>
        /// <param name="aspectConfiguration"></param>
        /// <param name="configuration"></param>
        public static void AddInterceptor(this IAspectConfiguration aspectConfiguration,IConfiguration configuration)
        {
            var enableAopCache = Convert.ToBoolean(configuration["EnableAopCache"]);
            var enableAopLog= Convert.ToBoolean(configuration["EnableAopLog"]); 
            if(enableAopLog)
                aspectConfiguration.Interceptors.AddTyped<LogInterceptor>(Predicates.Implement(typeof(IBaseService)));
            if (enableAopCache)
                aspectConfiguration.Interceptors.AddTyped<CacheInterceptor>(method => method.IsDefined(typeof(CachingAttribute)));

            //aspectConfiguration.Interceptors.AddTyped<ControllerActionInterceptor>(Predicates.Implement(typeof(ControllerBase)));
        }
    }
}
