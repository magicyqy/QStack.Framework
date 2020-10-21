using AutoMapper;
using Microsoft.Extensions.Configuration;
using MimeTypes;
using MimeDetective;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Basic.ViewModel.File;
using QStack.Framework.Core;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtfUnknown;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace QStack.Framework.Basic.Services
{
    public class LocalFileService :AbstractService<UploadFile>, IFileService
    {
        //根目录
        private string basePath;

        //分片文件目录
        private string shardPath;

        //根地址
        private string baseUrl;

        private FileManagerOptions _fileManagerOptions;

        public LocalFileService(IMapper mapper,IHostEnvironment environment,IOptions<FileManagerOptions> options)
        {
            Mapper = mapper;
            basePath = environment.ContentRootPath;
            _fileManagerOptions = options.Value;
            shardPath = Path.Combine(basePath, "shards");
            baseUrl = new Uri(basePath).AbsoluteUri;
        }
        //[SessionInterceptor]
        public async Task<FileInfoDto> Save(FileInfoDto fileInfo, byte[] fileBuffer)
        {
            //var uploadFileDto = await this.Get<UploadFileDto>(f => f.MD5Code == fileInfo.MD5);
            //if (uploadFileDto == null)
            //{
                var filePath = Path.Combine(basePath, fileInfo.Path.Trim('/'), fileInfo.Name);
                try
                {
                    if (!File.Exists(filePath))
                    {
                        using (var file = File.Create(filePath))
                        {
                            await file.WriteAsync(fileBuffer);

                        }
                    }

                    fileInfo.Url = new Uri(filePath).PathAndQuery;

                }
                catch (Exception e)
                {
                    throw new ServiceFrameworkException("上传失败", e);
                }
               
            //    uploadFileDto = new UploadFileDto
            //    {
            //        MD5Code = fileInfo.MD5,
            //        RUrl = fileInfo.Url,
            //        Filename = fileInfo.Name,
            //        Extention = Path.GetExtension(fileInfo.Name).ToLower()

            //    };

            //    await this.AddOrUpdate<UploadFileDto, int>(uploadFileDto);
            //}
            //else
            //{
            //    fileInfo = GetFileInfo(Path.Combine(basePath, uploadFileDto.RUrl));
            //    //fileInfo.Url = uploadFileDto.RUrl;
            //    //fileInfo.Name = uploadFileDto.Filename;
            //    //fileInfo.Path=uploadFileDto.
            //}

            return fileInfo;
        }


        public bool DeleteFile(FileInfoDto file)
        {
            return false;
        }


        public void CreateFolder(FileInfoDto folder)
        {

            var directory = Path.Combine(basePath, folder.Path.Trim('/'), folder.Name.Trim('/'));

            if (Directory.Exists(directory))
            {
                throw new ServiceFrameworkException("文件夹已存在");
            }
            Directory.CreateDirectory(directory);
        }


        public async Task<List<FileInfoDto>> ListFileByKeyPrefix(string keyPrefix,Action<FileInfoDto> resultExcute=null)
        {
            keyPrefix = keyPrefix.Trim('/');
            string path = Path.Combine(basePath, keyPrefix);
            var resultList = new List<FileInfoDto>();
            var entryNames = Directory.EnumerateFileSystemEntries(path);
            var exccludePaths = _fileManagerOptions?.FileViewExcludePaths.Select(path => new Uri(Path.Combine(basePath, path)).LocalPath);
            foreach (var entryName in entryNames)
            {
              
                if (exccludePaths!=null&& exccludePaths.Any(path =>  path== entryName))
                    continue;
                var fileInfoDto =await GetFileInfo(entryName,false);
                resultExcute?.Invoke(fileInfoDto);
                resultList.Add(fileInfoDto);

            }

            return resultList;
        }
        public async Task<FileInfoDto> GetFile(string filepath, Action<FileInfoDto> resultExcute = null)
        {
            var fileInfoDto =await GetFileInfo(Path.Combine(basePath, filepath.Trim('/')));
            resultExcute?.Invoke(fileInfoDto);
            return fileInfoDto;
        }
        private async Task<FileInfoDto> GetFileInfo(string fullpath,bool withContent=true)
        {

            var attributes = File.GetAttributes(fullpath);
           
            FileInfoDto fileInfoDto = new FileInfoDto {  Size = 0,FullPath=fullpath };
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                fileInfoDto.Dir = true;
                fileInfoDto.Name = Path.GetFileName(fullpath);
                fileInfoDto.Path =Path.GetRelativePath(basePath, Directory.GetParent(fullpath).FullName);
                
            }
            else
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                fileInfoDto.Name = fileInfo.Name;
                fileInfoDto.Path = Path.GetRelativePath(basePath, Directory.GetParent(fullpath).FullName);
                fileInfoDto.Size = fileInfo.Length;
                fileInfoDto.UpdateTime = fileInfo.LastWriteTime;
                fileInfoDto.Url = new Uri(fileInfo.FullName).AbsoluteUri;
                fileInfoDto.ContentType = GetMimeType(fileInfo.FullName);
                if (withContent)
                {
                     var charsetResult=CharsetDetector.DetectFromFile(fileInfo);
                     if(charsetResult.Detected!=null)
                    {
                        var encoding = charsetResult.Detected.Encoding;
                        
                        fileInfoDto.Content = await File.ReadAllTextAsync(fileInfo.FullName,encoding);
                       
                    }
                    else
                        fileInfoDto.Content = "not support Blob file";
                   
                }
                
                
            }

            return fileInfoDto;
        }

        public string GetMimeType(string fullPath)
        {
            string fileType = null;
            try
            {
                fileType = MimeTypeMap.GetMimeType(Path.GetExtension(fullPath));
            }
            catch { }
            if(fileType.IsNullOrEmpty())
            {
                try
                {
                    using (var stream = new FileInfo(fullPath).OpenRead())
                        fileType = stream.GetFileType().Mime;
                }
                catch { }
            }
            return fileType;
        }
        public async Task SaveShard(ShardInfo shard)
        {

            try
            {

                var shardFileName = Path.Combine(shardPath, shard.UploadId, $"{shard.UploadId}_{shard.Shard}_{shard.totalShard}");
                using (var file = File.Create(shardFileName))
                {
                    await file.WriteAsync(shard.Data);

                }

            }
            catch (IOException e)
            {
                throw new ServiceFrameworkException("保存分片文件失败", e);
            }

        }

        public void MergeShard(ShardMergeInfo shardMergeInfo)
        {

            var shardParent = Path.Combine(shardPath, shardMergeInfo.UploadId);
            var fileNames = Directory.EnumerateFiles(shardParent, $"{shardMergeInfo.UploadId}_*");
            fileNames = fileNames.OrderBy(f => f.Split("_")[1]);

            var outFilePath = Path.Combine(basePath, shardMergeInfo.Prefix, shardMergeInfo.FileName);
            MergeFile(fileNames.ToArray(), outFilePath);
            //如果没有出现异常，则删除分片文件

            if (Directory.Exists(shardParent))
            {

                Directory.Delete(shardParent, true);

            }

        }


        public void DeleteSource(List<string> files)
        {
            foreach (var file in files)
            {
                var rPath = file.Trim('/');
                var filePath = Path.Combine(basePath, rPath);
                var attributes = File.GetAttributes(filePath);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.Delete(filePath, true);
                }
                else
                {
                    File.Delete(filePath);
                }

            }
        }

        public static void MergeFile(string[] inputFilePaths, string outputFilePath)
        {

            Console.WriteLine("Number of files: {0}.", inputFilePaths.Length);
            using (var outputStream = File.Create(outputFilePath))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        // Buffer size can be passed as the second argument.
                        inputStream.CopyTo(outputStream);
                    }
                    Console.WriteLine("The file {0} has been processed.", inputFilePath);
                }
            }
        }

    }
}
