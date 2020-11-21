
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Models
{
    [QStack.Framework.Core.Persistent.IgnoredByMigrations]
    [Table("ProxyIPs")]
    public class ProxyIP:IEntityRoot
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
