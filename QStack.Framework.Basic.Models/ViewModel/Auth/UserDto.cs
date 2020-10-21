using QStack.Framework.Basic.Enum;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class UserDto:BaseDto
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
        [JsonIgnore]
        public virtual string PassWord { get; set; }
        [DisplayName("邮件")]
        public virtual string Email { get; set; }

        [DisplayName("角色")]
        public virtual IList<RoleDto> Roles { get; set; } = new List<RoleDto>();

        public virtual IList<FunctionDto> Functions { get; set; } = new List<FunctionDto>();
        public virtual int? GroupId { get; set; }
        [DisplayName("组织")]
        public virtual string GroupName { get; set; }

        [DisplayName("描述")]
        public virtual string Describe { get; set; }

        [DisplayName("序号")]
        public virtual int Sequence { get; set; }
    }
}
