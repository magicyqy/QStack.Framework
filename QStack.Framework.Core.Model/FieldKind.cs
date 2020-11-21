using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Core.Model
{
    public enum FieldKind
    {
        Reference = 1, //实体导航属性是一对一
        ICollection = 2,//实体导航属性是一对多
        ValueType = 3, //非导航属性
        EnvType = 4
    }

}
