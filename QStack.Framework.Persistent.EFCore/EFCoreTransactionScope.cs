using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;

namespace QStack.Framework.Persistent.EFCore
{
    public class EFCoreTransactionScope : AbstractTransactionScope
    {
        
        EFCoreTransactionContext eFCoreTransactionContext;
        SessionContext sessionCtx;
        public EFCoreTransactionScope(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            eFCoreTransactionContext = serviceProvider.GetRequiredService<EFCoreTransactionContext>();
            sessionCtx = serviceProvider.GetRequiredService<SessionContext>();
            if (sessionCtx != null)
            {
                foreach (KeyValuePair<string, IDao> kv in sessionCtx.Daos)
                {
                    DbContext dbContext = kv.Value as DbContext;

                    if (!eFCoreTransactionContext.ContainSession(dbContext))
                        eFCoreTransactionContext.trans.Add(dbContext, dbContext.Database.BeginTransaction());
                }
            }

            sessionCtx.CreateDaoEvent += new CreateDaoHandler(SessionContext_CreateDaoEvent);

          

           

        }


        void SessionContext_CreateDaoEvent(IDao dao)
        {
            var dbContext = dao as DbContext;
            if (!eFCoreTransactionContext.ContainSession(dbContext))
            {
                var transaction = dbContext.Database.BeginTransaction();
                eFCoreTransactionContext.trans.Add(dbContext, transaction);
              
            }

        }
       
      
        /// <summary>
        /// 不采用异步操作，异步操作下各种奇怪的错误。暂时解决不了TODO
        /// </summary>
        public override async Task Complete()
        {
            SubOne();
            if ( Convert.ToInt32(SessionContext.GetData(SCOPE_TRANS_KEY))== 0)
            {
               
                if (eFCoreTransactionContext.trans.Count > 0)
                {
                   
                    try
                    {
                     
                        foreach (KeyValuePair<DbContext, IDbContextTransaction> kv in eFCoreTransactionContext.trans)
                        {

                           await kv.Key.SaveChangesAsync();
                        }
                        //int[] totals = await Task.WhenAll(daoTasks);

                        foreach (KeyValuePair<DbContext, IDbContextTransaction> kv in eFCoreTransactionContext.trans)
                        {
                           
                            if (kv.Key.Database.CurrentTransaction != null)
                                await kv.Value.CommitAsync();
                        }

                    }
                    catch (Exception e)
                    {
                        foreach (KeyValuePair<DbContext, IDbContextTransaction> kv in eFCoreTransactionContext.trans)
                        {
                            if (kv.Key.Database.CurrentTransaction != null)
                               await kv.Value.RollbackAsync();
                        }
                        //await Task.WhenAll(RollbackTasks);
                       
                        throw e;
                    }
                }
            }

           
        }

       

        public override void Dispose()
        {
            DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();


        }

        public override async ValueTask DisposeAsync()
        {
            if (Convert.ToInt32(SessionContext.GetData(SCOPE_TRANS_KEY)) == 0)
            {


                if (eFCoreTransactionContext.trans.Count > 0)
                {
                    foreach (KeyValuePair<DbContext, IDbContextTransaction> kv in eFCoreTransactionContext.trans)
                    {
                        await kv.Value.DisposeAsync();
                        //if (kv.Key.Database.CurrentTransaction != null)
                        //    kv.Value.Rollback();

                    }
                }
                //sessionCtx.CreateDaoEvent -= new CreateDaoHandler(SessionContext_CreateDaoEvent);
                               
                eFCoreTransactionContext.trans.Clear();


            }

        }
    }
}
