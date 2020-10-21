using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Blog
{
    public class JWTTokenOptions
    {
        
        /// <summary>
        /// 谁颁发的
        /// </summary>
        public string Issuer { get; set; } = "magicyqy.com";

        
        /// <summary>
        /// 颁发给谁
        /// </summary>
        public string Audience { get; set; } = "any";

       
        /// <summary>
        /// 令牌密码
        /// </summary>
        public string SecurityKey { get; private set; } = "{71D2BC3C-435F-4947-93E9-B038B79A3379}";

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(180);

        /// <summary>
        /// 修改密码，重新创建数字签名
        /// </summary>
        /// <param name="value"></param>
        public void SetSecurityKey(string value)
        {
            SecurityKey = value;

            CreateKey();
        }

       
        /// <summary>
        /// 对称秘钥
        /// </summary>
        public SymmetricSecurityKey Key { get; set; }

       
        /// <summary>
        /// 数字签名
        /// </summary>
        public SigningCredentials Credentials { get; set; }

        public JWTTokenOptions()
        {
            CreateKey();
        }

        private void CreateKey()
        {
            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        }
    }
}
