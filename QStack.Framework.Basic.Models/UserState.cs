using System.ComponentModel.DataAnnotations;

namespace QStack.Framework.Basic.Enum
{
    public enum UserState
    {
        [Display(Name="冻结")]
        Freezed=-1,
        [Display(Name = "停用")]
        UnActive =0,
        [Display(Name = "正常")]
        Active =1
    }
}