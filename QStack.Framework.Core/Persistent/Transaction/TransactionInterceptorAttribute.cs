using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Core.Persistent;

namespace QStack.Framework.Core.Transaction
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class TransactionInterceptorAttribute : AbstractInterceptorAttribute
    {
        private TransactionOptions options;
       


        public TransactionInterceptorAttribute()
        {
            options =new TransactionOptions { transactionScopeOption = ScopeOption.Required, TimeOut = new TimeSpan(0, 30, 0) };
            
        }

        public TransactionInterceptorAttribute(ScopeOption scopeOption)
        {
            this.options = new TransactionOptions { transactionScopeOption = scopeOption, TimeOut = new TimeSpan(0, 30, 0) }; 
            
        }
        public TransactionInterceptorAttribute(ScopeOption scopeOption,TimeSpan timeOut)
        {
            this.options = new TransactionOptions { transactionScopeOption = scopeOption, TimeOut = timeOut } ;

        }

        private static Type transactionScopeType;
        [Obsolete]
 
        AbstractTransactionScope CreateTransactionScope()
        {
           var constructor=  transactionScopeType.GetConstructor(new Type[] { typeof(TransactionOptions) });

            return (AbstractTransactionScope)constructor.GetReflector().Invoke(new TransactionOptions());

        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
          
            //var sessionContext = context.ServiceProvider.GetRequiredService<SessionContext>();

            //sessionContext.SetData("TransactionOptions", options);
           await  using (AbstractTransactionScope trans = context.ServiceProvider.GetRequiredService<AbstractTransactionScope>())
            {
                try
                {
                    //trans.Start();
                    await next(context);

                   await trans.Complete();
                }
                catch(Exception e)
                {
                    trans.Terminated();

                    throw new ServiceFrameworkException(e.Message,e);
                }
            }

          

        }
    }//end TransactionInterceptor

}//end namespace Transaction