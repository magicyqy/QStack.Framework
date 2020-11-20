using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Web.Areas.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Entity;

namespace QStack.Web.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class RoleController : ApiBaseController
    {
        readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<ResponseResult<PageModel<RoleDto>>> GetPages(DataTableOption query)
        {
            return await base.SearchQuery<RoleDto, IRoleService>(query);
        }

        public async Task<ResponseResult<int>> Post(RoleDto dto)
        {

           
            var item = await _roleService.AddOrUpdate<RoleDto, int>(dto);
            var result = new ResponseResult<int>(item.Id);

            return result;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = new ResponseResult();

            var roleDto = await _roleService.QueryById<RoleDto>(id);
            if(roleDto == null)
            {
                result.Message = nameof(BusinessCode.Record_NotFound);
                result.Code = BusinessCode.Record_NotFound;

            }
            else if("admin".Equals(roleDto.Name))
            {
                result.Message = nameof(BusinessCode.Permission_NotAllowed);
                result.Code = BusinessCode.Permission_NotAllowed;
            }
            else
                await _roleService.Delete(id);


            return Ok(result);
        }
        public async Task<ResponseResult<List<RoleFunctionDto>>> getPermissions(int id)
        {
            var result = new ResponseResult<List<RoleFunctionDto>>();
            result.Data = await _roleService.GetPermissions(id);

            return result;
        }


            public async Task<ResponseResult> PostPermission(RolePermission rolePermission)
        {
            var result = new ResponseResult();
            if (rolePermission != null&& rolePermission.FunctionIds!=null)
            {
                var permissons = new List<RoleFunctionDto>();
                foreach(var fid in rolePermission.FunctionIds)
                {
                    permissons.Add(new RoleFunctionDto { RoleId = rolePermission.RoleId, FunctionId = fid });
                }

                await _roleService.UpdateRolePermission(rolePermission.RoleId, permissons);
            }

            return result;
        }
    }
}