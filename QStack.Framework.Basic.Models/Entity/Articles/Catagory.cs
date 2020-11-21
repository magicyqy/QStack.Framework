using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model.Articles
{
    public class Catagory: EntityRoot
    {  
        public virtual string Name { get; set; }

        public virtual int? ParentId { get; set; }
        public virtual Catagory Parent { get; set; }

        public virtual string Code { get; set; }

        public virtual List<Catagory> Children { get; set; }
    }
}
