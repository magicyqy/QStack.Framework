using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.File
{
    public class ShardInfo
    {
        public string UploadId { get; set; }

        public int totalShard { get; set; }

        public int Shard { get; set; }

        public byte[] Data { get; set; }
    }
}
