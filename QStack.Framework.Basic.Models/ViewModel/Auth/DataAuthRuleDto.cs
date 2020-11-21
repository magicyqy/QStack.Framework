using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model.ViewModel.Auth
{
    public class DataAuthRuleDto:BaseDto
    {
         public virtual string Repository { get; set; }
        public virtual string EntityType { get; set; }

        public virtual string RuleGroup { get; set; }

        public virtual string LambdaExpression { get; set; }

        public virtual DataAuthRuleState RuleState { get; set; }

        public virtual string Title { get; set; }
    }
}
