using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.ViewModel.File;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    public class UploadService
    {
        public UploadService(IMapper mapper)
        {

            //Mapper = mapper;

        }

        public void CreateFolder(FileInfoDto folder)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFile(FileInfoDto file)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteSource(List<FileInfoDto> files)
        {
            throw new System.NotImplementedException();
        }

        public List<FileInfoDto> ListFileByKeyPrefix(string keyPrefix)
        {
            throw new System.NotImplementedException();
        }

        public void MergeShard(ShardMergeInfo shardMergeInfo)
        {
            throw new System.NotImplementedException();
        }

        public Task<FileInfoDto> Save(FileInfoDto fileInfo, byte[] fileBuffer)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveShard(ShardInfo shard)
        {
            throw new System.NotImplementedException();
        }
    }
}
