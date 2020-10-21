using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QStack.Framework.Basic.Enum
{
    public enum ArticleType
    {
        [Description("原创")]
        Original = 0,
        [Description( "转发")]
        Transmit

    }
}
