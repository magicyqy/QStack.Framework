using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Contracts
{
    public class PluginOptions
    {
        public bool Enable { get; set; }
        /// <summary>
        /// 插件安装根路径
        /// </summary>
        public string InstallBasePath { get; set; } = Path.Combine("app_data", "plugins");
    }
}
