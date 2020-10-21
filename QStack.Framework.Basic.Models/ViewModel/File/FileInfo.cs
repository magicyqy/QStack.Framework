using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace QStack.Framework.Basic.ViewModel.File
{
    public class FileInfoDto
    {



        public string MD5 { get; set; }


        public string Name { get; set; }


        public bool Dir { get; set; }


        public long Size { get; set; }


        public string FormattedSize { get; set; }


        public string Suffix { get; set; }


        public string ContentType { get; set; }


        public string Path { get; set; }
        [JsonIgnore]
        public string FullPath { get; set; }
        public string Url { get; set; }


        public DateTime UpdateTime { get; set; }

        public string Content { get; set; }
    }
}
