using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Areas.Api.Models
{
    public class UserRoleV
    {
        public int UserId { get; set; }

        public int[] RoleIds { get; set; }
    }
}
