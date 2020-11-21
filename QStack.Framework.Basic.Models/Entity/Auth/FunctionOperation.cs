using QStack.Framework.Core.Model;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.Model.Auth
{
    public class FunctionOperation:EntityBase
    {
        [Key, Column(Order = 1)]
        public virtual int FunctionId { get; set; }
        [DisplayName("功能")]
        public virtual Function Function { get; set; }
        [Key, Column(Order = 2)]
        public virtual int OperationId { get; set; }
        [DisplayName("操作")]
        public virtual Operation Operation { get; set; }
       
       
        [DisplayName("描述")]
        public virtual string Describe { get; set; }
        [DisplayName("序号")]
        public virtual int Sequence { get; set; }
    }
}