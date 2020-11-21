using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.Model.Auth
{
    public class RoleFunction: IEntityRoot
    {
        [Key, Column(Order = 1)]
        public virtual int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [Key, Column(Order = 2)]
        public virtual int FunctionId { get; set; }
        public virtual Function Function { get; set; }
    }
}
