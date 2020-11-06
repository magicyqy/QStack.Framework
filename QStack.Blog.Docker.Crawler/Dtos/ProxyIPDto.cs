using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Dtos
{
    public class ProxyIPDto
    {
        public int Id { get; set; }
        [Description("IP")]
        public string IP { get; set; }
        [Description("端口")]
        public int Port { get; set; }
        [Description("匿名度")]
        public string Secret { get; set; }
        [Description("类型")]
        public string Type { get; set; }
        [Description("归属地")]
        public string Address { get; set; }
        public string CountryId { get; set; }
        [Description("运营商")]
        public string ISP { get; set; }
        [Description("响应速度")]
        public string RSpeed { get; set; }
        public DateTime Record_CreateTime { get; set; }
        [Description("最后验证时间")]
        public DateTimeOffset? LastValidTime { get; set; }
        [Description("有效期")]
        public DateTimeOffset? ExpireTime { get; set; }
    }
}
