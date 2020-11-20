using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Areas.Api.Models
{
    public class CaptchaInfo
    {
        public int Errcode { get; set; }
        public int Y { get; set; }

        public string Array { get; set; }
        public int ImgX { get; set; }

        public int ImgY { get; set; }

        public string Small { get; set; }

        public string Normal { get; set; }
               
    }
}
