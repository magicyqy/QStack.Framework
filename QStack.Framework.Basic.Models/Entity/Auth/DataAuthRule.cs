using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model.Auth
{
    public class DataAuthRule:EntityBase
    {
        public virtual string Repository { get; set; }
        public virtual string EntityType { get; set; }

        public virtual string RuleGroup { get; set; }

        public virtual string LambdaExpression { get; set; }

        public virtual DataAuthRuleState RuleState { get; set; }
        public virtual string Title { get; set; }
    }

   
}
