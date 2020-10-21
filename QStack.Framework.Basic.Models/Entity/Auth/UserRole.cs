using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QStack.Framework.Basic.Model.Auth
{
    public class UserRole : IEntityRoot
    {
        [Key, Column(Order = 1)]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
        [Key, Column(Order = 2)]
        public virtual int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
