using QStack.Framework.Core.Model;
using System.Collections.Generic;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class GroupDto:BaseDto
    {
        public virtual int? ParentId { get; set; }
        public virtual string ParentName { get; set; }
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }


        public virtual string Describe { get; set; }

        public virtual IList<RoleDto> Roles { get; set; }

        public virtual IList<UserDto> Users { get; set; }

        public virtual List<GroupDto> Children { get; set; }
    }
}