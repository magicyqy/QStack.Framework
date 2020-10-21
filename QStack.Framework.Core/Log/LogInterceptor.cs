using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QStack.Framework.Util;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace QStack.Framework.Core.Log
{
    /// <summary>
    /// aop方式的全局服务方法日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true)]
    public class LogInterceptor : AbstractInterceptorAttribute
    {
        [FromServiceContext] 
        public ILogger<LogInterceptor> Logger { get; set; }
        [FromServiceContext]
        public IConfiguration Configuration { get; set; }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            Exception exception = null;
            Stopwatch timer = new Stopwatch();
            try
            {
                timer.Start();
                await next(context);
                
            }
            catch (Exception e)
            {
                exception = e.InnerException??e;
                throw new ServiceFrameworkException(e.Message, e);
            }
            finally
            {
               
                timer.Stop();
                //var task = context.ReturnValue as Task<object>;

                var aopLog = new
                {
                    ExecutionTime = timer.ElapsedMilliseconds,
                    Methiod = $"{context.Implementation.GetType().FullName}.{context.ImplementationMethod.Name}",
                    Arguments = GetArguments(context),
                    ReturnValue = context.ReturnValue?.GetType().GetProperty("Result")?.GetReflector().GetValue(context.ReturnValue)
                 
                };
                if (exception == null)
                    Logger.LogInfo(aopLog);
                else
                    Logger.LogException(exception, aopLog);
            }
        }
        private object GetArguments(AspectContext context)
        {
            StringBuilder builder = new StringBuilder(0x80);
            var parameters = context.ImplementationMethod.GetParameters();
            object[] arguments = context.Parameters;
            for (int i = 0; i < parameters.Length; i = (int)(i + 1))
            {

                try
                {
                    if (arguments[i] == null || arguments[i].GetType().IsValueType)
                        builder.Append(parameters[i].Name).Append("=").Append(arguments[i]);
                    else if (arguments[i] is Expression)
                        builder.Append(parameters[i].Name).Append("=").Append((arguments[i] as Expression).ToExpressionString());
                    else
                        builder.Append(parameters[i].Name).Append("=").Append(arguments[i]).Append(JsonSerializer.Serialize(arguments[i]));
                }
                catch
                {
                    builder.Append(parameters[i].Name).Append("=").Append(arguments[i]?.ToString());
                }
                if (i != parameters.Length)
                {
                    builder.Append("; ");
                }
            }
            return builder.ToString().TrimEnd(';');
        }
    
    }
}
