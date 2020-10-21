using QStack.Framework.Core.Entity;
using System.Collections.Generic;

namespace QStack.Framework.Basic.Model.Auth
{
    public class Group:EntityBase
    {
        public virtual int? ParentId { get; set; }
        public virtual Group Parent { get; set; }
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }


        public virtual string Describe { get; set; }

        public virtual IList<GroupRole> Roles { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}