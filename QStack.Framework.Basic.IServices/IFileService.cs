using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.File;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    public interface IFileService:IBaseService
    {
        void CreateFolder(FileInfoDto folder);
        bool DeleteFile(FileInfoDto file);
        void DeleteSource(List<string> files);
   
        Task<FileInfoDto> GetFile(string filepath, Action<FileInfoDto> resultExcute = null);
       
        Task<List<FileInfoDto>> ListFileByKeyPrefix(string keyPrefix, Action<FileInfoDto> resultExcute = null);
        void MergeShard(ShardMergeInfo shardMergeInfo);
        Task<FileInfoDto> Save(FileInfoDto fileInfo, byte[] fileBuffer);
        Task SaveShard(ShardInfo shard);
    }
}