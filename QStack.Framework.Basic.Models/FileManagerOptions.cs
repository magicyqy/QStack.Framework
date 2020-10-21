using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Model
{
    public class FileManagerOptions
    {
        public long UploadLimitSize { get; set; } = 2147483648L;

        /// <summary>
        /// 允许上传类型
        /// </summary>
        public List<string> AllowedFileTypes { get; set; } = new List<string> { "image/jpg", "image/png", "image/jpeg", "application/x-zip-compressed"};

        [Obsolete]
        /// <summary>
        /// 上传图片默认路径<br/>
        /// 新版文件管理已不用该字段 2020-8-25
        /// </summary>
        public string UploadFilePath { get; set; } ="upload" ;

        /// <summary>
        /// 新版文件管理浏览忽略路径或文件
        /// </summary>
        public List<string> FileViewExcludePaths { get; set; } = new List<string>();
    }
}
