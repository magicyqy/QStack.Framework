using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Comon
{

    public class HttpException : Exception
    {
        Dictionary<int, string> statusCodeDescription = new Dictionary<int, string>
        {
            {400,"请求无效" },
            {401,"请求无效，未授权" },
            {403,"禁止访问-发生错误" },
            {404,"页面不存在" },
            {405,"资源被禁止" },
            {406,"无法接受" },
            {407,"要求代理身份验证" },
            {500,"内部服务器错误" },
            {501,"页面值指定了未实现的配置" },
            {502," 网关错误" },
            {503,"服务不可用" },
            {504,"网关超时" },
            {505,"http版本不受支持" }
        };
        public int StatusCode { get; }
        public new string Message { get; }
        public HttpException(int statusCode) :this(statusCode,null)
        {

        }

        public HttpException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            if (message == null && statusCodeDescription.TryGetValue(statusCode, out message))
                Message = message;
        }
    }
}
