using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Core.Cache
{
    public static class CacheExtension
    {
        /// <summary>
        /// 注册EasyCaching缓存操作
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configAction">配置操作</param>
        public static void AddCache(this IServiceCollection services, Action<EasyCachingOptions> configAction)
        {
            services.AddSingleton<ICache, CacheManager>();
            services.AddEasyCaching(configAction);
        }
    }
}
