using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Areas.Api.Models
{
    public static class BusinessCode
    {
        public const int Password_Invalid = 40001;
        public const int User_NotFound = 40002;
        public const int User_UnActive = 40003;
        public const int User_Freeze = 40004;
        public const int Password_Params_Invalid = 40005;

        public const int Image_Empty = 40100;
        public const int Image_Size_Error = 40101;   
        public const int Image_Type_Error = 40102;

        public const int Record_Cascade_Exist = 40200;

        public static int Record_NotFound = 40400;

        public static int Captcha_Required = 40300;
        public static int Captcha_Code_NotExist = 40301;
        public static int Captcha_Pos_NotExist = 40302;
        public static int Captcha_Pos_NotValid = 40303;
        public static int Captcha_Retry_Exceed = 40304;
        public static int Captcha_Pos_Error = 40305;
        public static int Captcha_NotValid = 40306;

        public static int Permission_NotAllowed = 40900;

        public static int Params_Error = 40901;
    }
}
