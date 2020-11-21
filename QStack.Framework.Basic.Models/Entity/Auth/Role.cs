using QStack.Framework.Core.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace QStack.Framework.Basic.Model.Auth
{
    public class Role : EntityBase
    {
        [DisplayName("代码")]
        public virtual string Code { get; set; }

        [DisplayName("名称")]
        public virtual string Name { get; set; }
        [DisplayName("所属组织")]
        public virtual IList<GroupRole> GroupRoles { get; set; }
      
        [DisplayName("角色用户")]
        public virtual IList<UserRole> UserRoles { get; set; }
        [DisplayName("功能")]
        public virtual IList<RoleFunction> RoleFunctions { get; set; }
        [DisplayName("功能操作")]
        public virtual IList<FunctionOperation> FunctionOperations { get; set; }
        [DisplayName("描述")]
        public virtual string Describe { get; set; }
        [DisplayName("序号")]
        public virtual int Sequence { get; set; }

    }
}