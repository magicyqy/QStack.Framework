using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Areas.Api.Models
{
    public class PasswordV
    {
        public int UserId { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string NewPassword1 { get; set; }

    }
}
