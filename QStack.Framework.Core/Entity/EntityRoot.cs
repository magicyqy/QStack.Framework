using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QStack.Framework.Core.Entity
{
    public class EntityRoot: IEntityRoot
    {

        [DisplayName("Id")]
        public virtual int Id { get; set; }
    }
}
