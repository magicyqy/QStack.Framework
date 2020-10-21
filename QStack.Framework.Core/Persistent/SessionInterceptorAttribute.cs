using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ServiceFramework.Common;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QStack.Framework.Core.Persistent
{
    /// <summary>
    /// 服务方法拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface,  Inherited = true)]
    public class SessionInterceptorAttribute : AbstractInterceptorAttribute
    {


        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine("start intercept method:" + context.ImplementationMethod.Name);
            //通过ServiceProvider获取的sessionscope在ServiceProviderScope期间都是唯一的，
            //即整个requet请求期间都是唯一，所以如果存在拦截器嵌套的情况，要通过内部计数器来确保flush,dispose方法
            //在最外层的拦截器才真正执行，
           await using (var sessionScope = context.ServiceProvider.GetRequiredService<SessionScope>())
            {
              
                try
                {
                    

                    var daoCollection = context.ServiceProvider.GetRequiredService<IDaoCollection>();
                    //var daoCollection = new DaoCollection(context.ServiceProvider.GetRequiredService<SessionContext>());
                    var types = context.Implementation.GetType().BaseType.GenericTypeArguments;
                    if (types != null && types.Length > 0)
                    {
                        var gerericTypeAssembly = types[0].Assembly;
                        var daoFactorys = context.ServiceProvider.GetServices<IDaoFactory>();
                        foreach (var factory in daoFactorys)
                        {
                            if (factory.DaoFactoryOption.EntityAssemblies.Any(a => a == gerericTypeAssembly))
                            {
                                daoCollection.SetDefaultFactoryName(factory.FactoryName);
                                break;
                            }
                        }
                    }
                    var property = context.Implementation.GetType().GetTypeInfo().GetProperty("Daos");
                    var reflector = property.GetReflector();
                    var value = reflector.GetValue(context.Implementation);
                    //if (value == null)
                        reflector.SetValue(context.Implementation, daoCollection);

                    await next(context);

                }
                catch(Exception e)
                {
                    sessionScope.Terminated();
                    throw new ServiceFrameworkException(e.Message, e);
                }

            }
           
        }
    }//end SessionInterceptor

}//end namespace Persistent