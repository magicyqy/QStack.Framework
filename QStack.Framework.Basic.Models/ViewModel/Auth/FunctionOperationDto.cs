using QStack.Framework.Core.Model;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Framework.Basic.ViewModel.Auth
{
    public class FunctionOperationDto:BaseDto
    {
       
        public virtual int FunctionId { get; set; }
        [DisplayName("功能")]
        public virtual FunctionDto Function { get; set; }
      
        public virtual int OperationId { get; set; }
        [DisplayName("操作")]
        public virtual OperationDto Operation { get; set; }
       
       
        [DisplayName("描述")]
        public virtual string Describe { get; set; }
        [DisplayName("序号")]
        public virtual int Sequence { get; set; }
    }
}