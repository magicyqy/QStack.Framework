using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QStack.Framework.Core;
using System.Collections.Generic;

namespace QStack.Framework.Persistent.EFCore
{
    public class EFCoreTransactionContext
    {
        public Dictionary<DbContext, IDbContextTransaction> trans =new Dictionary<DbContext, IDbContextTransaction>();

        //public static EFCoreTransactionContext GetContext()
        //{
        //    return CallContext.GetData("EFCoreTransactionContext") as EFCoreTransactionContext;            
        //}

        //public static void SetContext(EFCoreTransactionContext ctx)
        //{
        //    CallContext.SetData("EFCoreTransactionContext", ctx);
        //}

        /// <summary>
        /// 遍历是否已经有TransSite包含了context
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool ContainSession(DbContext dbContext)
        {
            return trans.ContainsKey(dbContext);
        }

        public  void Clear()
        {
            trans.Clear();
        }
        
    }
}
