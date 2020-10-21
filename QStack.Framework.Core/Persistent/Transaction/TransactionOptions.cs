

using System;

namespace QStack.Framework.Core.Transaction
{
    public class TransactionOptions
    {
        public ScopeOption transactionScopeOption {get;set;}
        public TimeSpan TimeOut { get; set; }
    }

    public enum ScopeOption
    {
        Required,
        RequiredNew
    }

}