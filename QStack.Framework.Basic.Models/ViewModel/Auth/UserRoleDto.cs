using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class UserRoleDto
    {
       
        public virtual int UserId { get; set; }
        public virtual UserDto User { get; set; }
      
        public virtual int RoleId { get; set; }
        public virtual RoleDto Role { get; set; }
    }
}
