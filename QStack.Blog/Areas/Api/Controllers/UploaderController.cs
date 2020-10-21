using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QStack.Blog.Areas.Api.Models;
using QStack.Blog.Comon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Services;
using QStack.Framework.Basic.ViewModel;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class UploaderController : ApiBaseController
    {
        FileManagerOptions _uploadOptions;
        readonly IFileService _uploadService;
        public UploaderController(IOptions<FileManagerOptions> uploadOptions, IFileService uploadService) {
            _uploadOptions = uploadOptions.Value;
            _uploadService = uploadService;
        }

        /// <summary>
        /// 上传图片,多文件，可以使用 postman 测试，
        /// 如果是单文件，可以 参数写 IFormFile file1
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpPost]        
        public async Task<ResponseResult<dynamic>> Upload([FromServices]IWebHostEnvironment environment)
        {
            var data = new ResponseResult<dynamic>();
            string path = string.Empty;
          
            IFormFileCollection files = null;

            try
            {
                files = Request.Form.Files;
            }
            catch (Exception)
            {
                files = null;
            }

            if (files == null || !files.Any()) 
            { 
                data.Message =nameof(BusinessCode.Image_Empty);
                data.Code = BusinessCode.Image_Empty;
                return data; 
            }
            //格式限制
          

            string folderpath = Path.Combine(environment.WebRootPath, _uploadOptions.UploadFilePath);
            if (!System.IO.Directory.Exists(folderpath))
            {
                System.IO.Directory.CreateDirectory(folderpath);
            }

            if (files.Any(c => _uploadOptions.AllowedFileTypes?.Contains(c.ContentType.ToLower())==true))
            {
                if (files.Sum(c => c.Length) <= _uploadOptions.UploadLimitSize)
                {
                    //foreach (var file in files)
                    var file = files.FirstOrDefault();
                    string strpath = Path.Combine(_uploadOptions.UploadFilePath, DateTime.Now.ToString("MMddHHmmss") + file.FileName);
                    path = Path.Combine(environment.WebRootPath, strpath);
                    string md5Code = string.Empty;
                    UploadFileDto uploadFileDto;
                    int uploadFileId = 0;
                    using (var stream =  file.OpenReadStream())
                    {
                       
                        md5Code = GetFileMD5(stream);
                        uploadFileDto = await _uploadService.Get<UploadFileDto>(f => f.MD5Code == md5Code);
                        if (uploadFileDto == null)
                        {
                            using (var filestream = System.IO.File.Create(path))
                            {
                                await file.CopyToAsync(filestream);
                            }
                            uploadFileDto = new UploadFileDto
                            {
                                MD5Code = md5Code,
                                RUrl = strpath.Replace("\\", "/"),
                                Filename = file.FileName,
                                Extention = Path.GetExtension(file.FileName).ToLower()

                            };
                           
                            uploadFileId = (await _uploadService.AddOrUpdate<UploadFileDto, int>(uploadFileDto)).Id;
                        }
                        else
                            uploadFileId = uploadFileDto.Id;
                        strpath = UriCombine(Request.GetHostUri(), uploadFileDto.RUrl);
                        //strpath = uploadFileDto.RUrl;
                    }                   
                   
                    data = new ResponseResult<dynamic>()
                    {
                        Data =new { url = strpath, id = uploadFileId }

                     };
                    return data;
                }
                else
                {
                    data.Message = nameof(BusinessCode.Image_Size_Error); ;
                    data.Code = BusinessCode.Image_Size_Error;
                    return data;
                }
            }
            else

            {
                data.Message = nameof(BusinessCode.Image_Type_Error);
                data.Code = BusinessCode.Image_Type_Error;
                return data;
            }
        }


        string GetFileMD5(Stream file)
        {
            try
            {

                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }

        }

    }
}