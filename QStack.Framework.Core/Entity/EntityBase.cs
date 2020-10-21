using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QStack.Framework.Core.Entity
{
    public class EntityBase: EntityRoot
    {
        [DisplayName("创建时间")]
        public virtual DateTime? CreateDate { get; set; } = DateTime.Now;
        [DisplayName("最后修改时间")]
        public virtual DateTime? LastModifyDate { get; set; }
        [DisplayName("创建者Id")]
        public virtual int? CreateUserId { get; set; }
        [DisplayName("最后修改人Id")]
        public virtual int? LastModifyUserId { get; set; }
    }
}
