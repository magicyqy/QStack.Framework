//#define DEBUG_SESSIONCONTEXT

using ServiceFramework.Common;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Threading;

namespace QStack.Framework.Core.Persistent
{
    public class SessionScope : IDisposable,IAsyncDisposable
    {
        bool registered = false;
        SessionContext sessionContext;
      
        readonly string SCOPE_SESSION_KEY = "SESSION_SCOPE";
        public SessionScope(IServiceProvider serviceProvider)
        {
            //sessionContext = SessionContext.CurrentContext;

            //            if (sessionContext == null)
            //            {
            //                sessionContext = new SessionContext(serviceProvider);

            //                SessionContext.SetContext(sessionContext);

            //#if DEBUG_SESSIONCONTEXT
            //                Console.WriteLine("注册SessionContext:{0}", sessionContext.GetHashCode());
            //#endif
            //                registered = true;
            //            }
            sessionContext = serviceProvider.GetRequiredService<SessionContext>();
            AddOne();
            Console.WriteLine("注册SessionContext:{0}", sessionContext.GetHashCode());

            
        }
        /// <summary>
        /// 嵌套服务方法调用时，需要计数
        /// </summary>
        /// <returns></returns>
        private int AddOne()
        {
            var nestedScopeCount =Convert.ToInt32( sessionContext.GetData(SCOPE_SESSION_KEY));

            sessionContext.SetData(SCOPE_SESSION_KEY, nestedScopeCount++);
            return nestedScopeCount;
        }
        /// <summary>
        /// 嵌套服务方法调用时，需要计数
        /// </summary>
        /// <returns></returns>
        private int MinusOne()
        {
            var nestedScopeCount = Convert.ToInt32(sessionContext.GetData(SCOPE_SESSION_KEY));
            nestedScopeCount--;
            nestedScopeCount = nestedScopeCount < 0 ? 0 : nestedScopeCount;
            sessionContext.SetData(SCOPE_SESSION_KEY, nestedScopeCount);
            return nestedScopeCount;
        } 
        bool terminated = false;
        public void Terminated()
        {
            sessionContext.SetData(SCOPE_SESSION_KEY, 0);
            terminated = true;
        }
        public  void Dispose()
        {
            DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();


        }

        public async ValueTask DisposeAsync()
        {
            var nestedScopeCount = MinusOne();
            if (!terminated && nestedScopeCount == 0)
            {

                foreach (KeyValuePair<string, IDao> kv in sessionContext.Daos)
                {
                    await kv.Value.Flush();

                }


#if DEBUG_SESSIONCONTEXT
                Console.WriteLine("注销SessionContext:{0}", sessionContext.GetHashCode());
#endif

            }
            if (nestedScopeCount == 0)
                sessionContext.Daos.Clear();
        }
    }
}
