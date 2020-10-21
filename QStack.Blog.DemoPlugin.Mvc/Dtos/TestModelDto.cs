using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.DemoPlugin.Mvc.Models
{
    public class TestModelDto:BaseDto
    {
        public virtual string Message { get; set; }
    }
}
