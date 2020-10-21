using QStack.Framework.Core.Persistent;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace QStack.Framework.Core.Transaction
{
    public abstract class AbstractTransactionScope: IDisposable,IAsyncDisposable
    {
        protected TransactionOptions transactionOptions;
        protected IServiceProvider serviceProvider;
        protected SessionContext SessionContext;
        protected int TransactionCount = 0;
       protected  readonly string SCOPE_TRANS_KEY = "TRANSACTION_SCOPE";
        public AbstractTransactionScope(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            SessionContext = serviceProvider.GetService<SessionContext>();
            AddOne();
        }
        public AbstractTransactionScope(TransactionOptions transactionOptions)
        {
            this.transactionOptions = transactionOptions;
        }

        public void SetTransactionOptions(TransactionOptions transactionOptions)
        {
            this.transactionOptions = transactionOptions;
        }

        #region IDisposable Members


        public abstract Task Complete();

        /// <summary>
        /// 事务计数器加1
        /// </summary>
        protected void AddOne()
        {
           
            var nestedScopeCount = Convert.ToInt32(SessionContext.GetData(SCOPE_TRANS_KEY));
            nestedScopeCount++;
            SessionContext.SetData(SCOPE_TRANS_KEY, nestedScopeCount);
           
        }

        /// <summary>
        /// 事务计数器减1
        /// </summary>
        protected void SubOne()
        {
            var nestedScopeCount = Convert.ToInt32(SessionContext.GetData(SCOPE_TRANS_KEY));
            nestedScopeCount--;
            nestedScopeCount = nestedScopeCount < 0 ? 0 : nestedScopeCount;
            SessionContext.SetData(SCOPE_TRANS_KEY, nestedScopeCount);
         

        }

        public abstract void Dispose();

        protected bool disposed = false;
        [Obsolete]
        public void Start()
        {
            disposed = false;
            AddOne();
        }
        public void Terminated()
        {  
            
            SessionContext.SetData(SCOPE_TRANS_KEY, 0);
          
        }

        public abstract ValueTask DisposeAsync();
        

        #endregion
    }
}
