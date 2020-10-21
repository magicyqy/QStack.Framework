using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.DemoPlugin.Mvc.Models
{
   
    public class TestModel:EntityBase
    {
        public virtual string Message { get; set; }
    }
}
