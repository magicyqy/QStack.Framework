using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.CommonSearch;
using QStack.Framework.Core.Model;
using QStack.Framework.Util;
using QStack.Web.Areas.Api.Controllers;
using QStack.Web.Areas.Api.Models;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class CommentController : ApiBaseController
    {
        ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            this._commentService = commentService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> PostComment(CommentDto model)
        {

            if(!model.Email.IsEmail()||
                model.NickName.IsNullOrWhiteSpace() ||
                model.CommentText.IsNullOrWhiteSpace())
            {
                return BadRequest();
            }

            model.RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString();
            
            model = await _commentService.AddOrUpdate<CommentDto, int>(model);
            var result = new ResponseResult<CommentDto>(model);

            return Ok(result);
        }

        public async Task<IActionResult> Get(int id)
        {

            var result = new ResponseResult<CommentDto>();

            var item = await _commentService.QueryById<CommentDto>(id);

            if (item == null)
            {
                result.Message = nameof(BusinessCode.Record_NotFound);
                result.Code = BusinessCode.Record_NotFound;
            }
            result.Data = item;


            return Ok(result);
        }

        public async Task<ResponseResult> SearchQuery(DataTableOption query)
        {

            return await base.SearchQuery<CommentDto,ICommentService>(query);
        }
        [AllowAnonymous]
        public async Task<ResponseResult<PageModel<CommentDto>>> GetCommentPage(int page,int pageSize,int? articleId=null,int? commentParentId=null)
        {
            var data= await _commentService.GetCommentPage(page, pageSize, articleId, commentParentId);

            var result = new ResponseResult<PageModel<CommentDto>>(data);

            return result;
        }

    }
}