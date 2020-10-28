using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Blog.Areas.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Entity;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]

    public class GroupController : ApiBaseController
    {
        readonly IGroupService _groupService;
        readonly IUserService _userService;
        
        public GroupController(IGroupService groupService,IUserService userService)
        {
            _groupService = groupService;
            this._userService = userService;
        }

        public async Task<ResponseResult<List<GroupDto>>> GetAll()
        {
            var list= await _groupService.GetAll<GroupDto>();
            list = GetForTree<GroupDto>(list, null);
            return new ResponseResult<List<GroupDto>>(list);
        }

        public async Task<ResponseResult<int>> Post(GroupDto dto)
        {

            
            var item = await _groupService.AddOrUpdate<GroupDto, int>(dto);
            var result = new ResponseResult<int>(item.Id);

            return result;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = new ResponseResult();

            if (await _userService.CountAsync<UserDto>(r => r.GroupId == id) > 0)
            {
                result.Message = nameof(BusinessCode.Record_Cascade_Exist);
                result.Code = BusinessCode.Record_Cascade_Exist;
            }
            else
                await _groupService.DeleteByIdAsync(id);


            return Ok(result);
        }
    }
}