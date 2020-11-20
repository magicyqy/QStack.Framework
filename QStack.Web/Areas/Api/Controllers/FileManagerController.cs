using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QStack.Web.Areas.Api.Models;
using QStack.Web.Comon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QStack.Framework.Basic;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Services;
using QStack.Framework.Basic.ViewModel.File;
using QStack.Framework.Util;

namespace QStack.Web.Areas.Api.Controllers
{
    [Route("{area:exists}/system/file")]
    public class FileManagerController : ApiBaseController
    {
        FileManagerOptions _uploadOptions;
        readonly IFileService _fileService;
        IWebHostEnvironment _webHostEnvironment;
        public FileManagerController(IFileService fileService,
            IOptions<FileManagerOptions> uploadOptions,
            IWebHostEnvironment webHostEnvironment)
        {
            _fileService = fileService;
            _uploadOptions = uploadOptions.Value;
            _webHostEnvironment = webHostEnvironment;
        }
       
        [HttpPost("folder")]
        public async Task<IActionResult> CreateFolder(FileInfoDto fileInfoDto)
        {
            _fileService.CreateFolder(fileInfoDto);
            return await Task.FromResult(Ok(new ResponseResult()));
        }


        [HttpGet]
        public async Task<ResponseResult<List<FileInfoDto>>> ListFiles(string prefix)
        {
           var list= await _fileService.ListFileByKeyPrefix(prefix, UrlConverter);
            return new ResponseResult<List<FileInfoDto>>(list);
        }
        [HttpPost]
        public async Task<ResponseResult<dynamic>> UploadFile([FromForm]UploadInfo uploadInfo)
        {
            var formFile = uploadInfo.File;
            var data = new ResponseResult<dynamic>();
            if (formFile == null)
            {
                data.Message = nameof(BusinessCode.Image_Empty);
                data.Code = BusinessCode.Image_Empty;
                return data;
            }
            if (_uploadOptions.AllowedFileTypes?.Contains(formFile.ContentType.ToLower()) == false)
            {
                data.Message = nameof(BusinessCode.Image_Type_Error);
                data.Code = BusinessCode.Image_Type_Error;
                return data;
            }
            if (formFile.Length > _uploadOptions.UploadLimitSize)
            {
                data.Message = nameof(BusinessCode.Image_Size_Error); ;
                data.Code = BusinessCode.Image_Size_Error;
                return data;
            }
            byte[] bytes = new byte[formFile.Length];
            using (var stream = formFile.OpenReadStream())
            {
                
                await stream.ReadAsync(bytes, 0, (int)formFile.Length);
               
            }
            var md5Code = Encrypt.Md5(bytes, null, null);
            var fileInfoDto = new FileInfoDto
            {
                MD5 = md5Code,
                Name = formFile.FileName,
                Size = formFile.Length,
                UpdateTime = DateTime.Now,
                Dir = false,
                ContentType=formFile.ContentType,
                Path = uploadInfo.Prefix,
                
            };
            fileInfoDto=await _fileService.Save(fileInfoDto, bytes);
            data.Data = fileInfoDto;
            return data;
        }

        //{"path":"/111.txt","name":"111.txt","size":35,"extension":".txt","modified":"2020-08-11T14:23:15.900564765Z","mode":493,"isDir":false,"type":"text","content":"ddfgdfgdfgdd\nfgcfgdfgfgh\nghgjghjhjk"}
        [HttpGet("view")]
        public async Task<ResponseResult<dynamic>> GetFileContent(string filepath)
        {
            var fileInfo = await _fileService.GetFile(filepath, UrlConverter);
            return new ResponseResult<dynamic>(fileInfo);
        }
        private void UrlConverter(FileInfoDto fileInfoDto)
        {
            var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, fileInfoDto.FullPath);
            fileInfoDto.Url = new Uri(new Uri(HttpContext.Request.GetHostUri()), relativePath).ToString();
            if(fileInfoDto.Url.StartsWith(@"file://"))
                fileInfoDto.Url = new Uri(new Uri(HttpContext.Request.GetHostUri()), "/images/no_preview.png").ToString();
        }
       [HttpPost("shard")]
        public async Task<ResponseResult> UploadShard(ShardInfo shard)
        {
          
            await _fileService.SaveShard(shard);
            return new ResponseResult();
        }

        [HttpPost("mergeShard")]
        public async Task<ResponseResult> MergeShard(ShardMergeInfo shardMergeInfo)
        {
           
            _fileService.MergeShard(shardMergeInfo);
            return await Task.FromResult( new ResponseResult());
        }

        [HttpDelete]
        public async Task<ResponseResult> deleteSource(List<string> prefix)
        {
            
            _fileService.DeleteSource(prefix);
            return await Task.FromResult(new ResponseResult());
        }

    }

    public class UploadInfo
    {
        public string Prefix { get; set; }
        public IFormFile File { get; set; }
    }
}