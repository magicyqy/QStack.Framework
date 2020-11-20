using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Comon
{
    public class WebLog 
    {
        public DateTime RequestTime { get; set; } = DateTime.Now;

        public string IP { get; set; }

        public string ResponseSource { get; set; }

        public string RequestSource { get; set; }

        public string HttpResult { get; set; }

        public string RequestUrl { get; set; }
    }
}
