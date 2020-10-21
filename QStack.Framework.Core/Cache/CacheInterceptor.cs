using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QStack.Framework.Util;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace QStack.Framework.Core.Cache
{
    /// <summary>
    /// aop方式的全局缓存拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true)]
    public class CacheInterceptor: AbstractInterceptorAttribute
    {
        [FromServiceContext]
        public ICache CacheManager { get; set; }
        [FromServiceContext]
        public IConfiguration Configuration { get; set; }
        [FromServiceContext]
        public ILogger<CacheInterceptor> Logger { get; set; }
       
        private  string keyPrefix=> Configuration["CacheSettings:KeyPrefix"] ?? "SFCache";

        public CacheInterceptor()
        {
           
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cacheAttibute = context.ProxyMethod.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            if (cacheAttibute != null)
            {

                var cacheKey = $"{keyPrefix}_{context.Implementation.GetType().Name}";

                switch (cacheAttibute.Method)
                {
                    case CacheMethod.Get:
                        {
                            var parameterKey = GetParametersFingerprint(context.Parameters);
                            cacheKey = $"{cacheKey}_{context.ImplementationMethod.Name}_{parameterKey}";
                            //if (CacheManager.Exists(cacheKey))
                            //    context.ReturnValue = CacheManager.Get<object>(cacheKey);
                            //else
                            //{
                            //    await next(context);
                            //    CacheManager.TryAdd(cacheKey, context.ReturnValue);
                            //}

                            context.ReturnValue = await CacheManager.GetAsync(cacheKey,
                                            async () =>
                                            {
                                                await next(context);
                                                return context.ReturnValue;
                                            }
                            );


                            break;
                        }
                    case CacheMethod.Remove:
                        {
                            await next(context);
                            foreach (var methodName in cacheAttibute.CorrespondingMethodNames)
                            {
                                var key = $"{cacheKey}_{methodName}";
                                await CacheManager.RemoveByPrefixAsync(key);
                            }

                            break;
                        }
                    default:
                        await next(context);
                        break;

                }
                Logger?.LogInformation(cacheKey);
            }
            else
                await next(context);

                

            }

     

        private  string GetParametersFingerprint(params object[] parameters)
        {

            if (parameters == null)
                return "";

            return string.Join("_", parameters.Select(GetArgumentValue).ToArray());
        }

        private  string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();

            if (arg != null)
            {
                if (arg is Expression)
                {
                    var expression = arg as Expression;
                    var result = expression.ToExpressionString();
                    return Encrypt.Md5By16(result);
                }
                else if (arg.GetType().IsClass)
                {
                    return Encrypt.Md5By16(JsonSerializer.Serialize(arg));
                }
            }
            return string.Empty;
        }
    }
}

