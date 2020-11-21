using QStack.Framework.Core.Model;
using System.ComponentModel;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class OperationDto:BaseDto
    {
        [DisplayName("编码")]
        public virtual string Code { get; set; }
        [DisplayName("名称")]
        public virtual string Name { get; set; }
        [DisplayName("控制属性")]
        public virtual string ControlAttribute { get; set; }
        [DisplayName("属性值")]
        public virtual string AttributeValue { get; set; }
        [DisplayName("描述")]
        public virtual string Describe { get; set; }
    }
}