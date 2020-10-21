using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class GroupRoleDto
    {
      
        public virtual int GroupId { get; set; }

        public virtual GroupDto Group { get; set; }
      
        public virtual int RoleId { get; set; }
        public virtual RoleDto Role { get; set; }
    }
}
