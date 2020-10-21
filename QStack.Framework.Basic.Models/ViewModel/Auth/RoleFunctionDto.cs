using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class RoleFunctionDto
    {
        public virtual int RoleId { get; set; }
        public virtual RoleDto Role { get; set; }
     
        public virtual int FunctionId { get; set; }
        public virtual FunctionDto Function { get; set; }
    }
}
