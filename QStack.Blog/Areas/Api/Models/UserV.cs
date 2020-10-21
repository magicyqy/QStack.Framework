using QStack.Framework.Basic.ViewModel.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Areas.Api.Models
{
    public class UserV
    {

    
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Avatar { get; set; } = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif";

        public string UserName { get; set; }

        //"introduction":"I am a super administrator","email":"admin@test.com","phone":"1234567890","roles":["admin"]
        public string Introduction { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public List<string> Roles { get; set; }

        public List<FunctionDto> Functions { get; set; }
    }
}
