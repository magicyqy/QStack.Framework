using QStack.Framework.Basic.Enum;
using QStack.Framework.Core.Entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace QStack.Framework.Basic.Model.Auth
{
    [DisplayName("用户表")]
    public class User:EntityBase
    {
       
        [DisplayName("手机号")]
        public virtual string Mobile { get; set; }
        [DisplayName("状态")]
        public virtual UserState State { get; set; }

        [DisplayName("代号")]
        public virtual string Code { get; set; }

        [DisplayName("名称")]
        public virtual string Name { get; set; }
        [DisplayName("密码")]
        public virtual string PassWord { get; set; }
        [DisplayName("邮件")]
        public virtual string Email { get; set; }

    
        public virtual IList<UserRole> UserRoles { get; set; }
        [DisplayName("角色")]
        [NotMapped]
        public virtual IList<Role> Roles {
            get
            {
                return UserRoles?.Select(u => u.Role).ToList();
            }
            set {
                var items = value?.Where(i => i != null);
                
                if (UserRoles == null)
                    UserRoles = new List<UserRole>();
                foreach(var role in items)
                {
                    UserRoles.Add(new UserRole { RoleId = role.Id, UserId = Id });
                }
            }
        }

        public   virtual int? GroupId { get; set; }
        [DisplayName("组织")]
        public virtual Group Group { get; set; }

        [DisplayName("描述")]
        public virtual string Describe { get; set; }

        [DisplayName("序号")]
        public virtual int Sequence { get; set; }

    }
}
