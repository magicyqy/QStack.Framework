//#define DEBUG_TRANSACTION

using ServiceFramework.Common;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace QStack.Framework.Core.Transaction
{

    /// <summary>
    /// System.TransactionScope在netcore中不支持分布式事务，而且efcore下，Complete前仍需显式调用各上下文的DbContext.SaveChange方法
    /// 缺点颇多，暂时不建议用
    /// </summary>
    [System.Obsolete]
    public class StandardTransactionScope:AbstractTransactionScope
    {
        private TransactionScope transactionScope;

        public StandardTransactionScope(TransactionOptions options) :base(options)
        {
            if (options.transactionScopeOption == ScopeOption.RequiredNew)
            {
                transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, options.TimeOut);
                SetTransactionScopeFlag();

                AddOne();
            }
            else if (!IsExistTransactionScope())
            {
                transactionScope = new TransactionScope(TransactionScopeOption.Required, options.TimeOut);
                SetTransactionScopeFlag();

                AddOne();
            }
            Console.WriteLine($"{this.GetType().Name} threadid {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public bool IsExistTransactionScope()
        {
            object flag = CallContext.GetData("StandardTransactionScopeFlag");
            
            if (flag != null)
                return true;
            else
                return false;
        }

        public void SetTransactionScopeFlag()
        {
            CallContext.SetData("StandardTransactionScopeFlag", 1);

#if DEBUG_TRANSACTION
            Console.WriteLine("启动事务:{0}", transactionScope.GetHashCode());
#endif
        }

        public override Task Complete()
        {
            if (transactionScope != null)
            {
                transactionScope.Complete();

#if DEBUG_TRANSACTION
                Console.WriteLine("提交事务:{0}", transactionScope.GetHashCode());
#endif
            }
            return Task.CompletedTask;
        }

      

        public override  void Dispose()
        {

            if (transactionScope != null)
            {
                transactionScope.Dispose();

                SubOne();

#if DEBUG_TRANSACTION
                Console.WriteLine("释放事务:{0}", transactionScope.GetHashCode());
#endif

            }
        }

        public override ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
