using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Web.Areas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Model;
using QStack.Framework.Core.CommonSearch;

namespace QStack.Web.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class FunctionController : ApiBaseController
    {
        readonly IFunctionService _functionService;
        readonly IRoleService _roleService;

        public FunctionController(IFunctionService functionService, IRoleService roleService)
        {
            _functionService = functionService;
            _roleService = roleService;
        }

        public async Task<ResponseResult<List<FunctionDto>>> GetAll()
        {
       
            var list = await _functionService.GetAll<FunctionDto>();
            list = GetForTree<FunctionDto>(list.OrderBy(f=>f.Id).ToList(), null);
            return new ResponseResult<List<FunctionDto>>(list);
        }
        public async Task<ResponseResult<PageModel<FunctionDto>>> GetPages(DataTableOption query)
        {
            return await base.SearchQuery<FunctionDto, IFunctionService>(query);
        }

        public async Task<ResponseResult<int>> Post(FunctionDto dto)
        {


            var item = await _functionService.AddOrUpdate<FunctionDto, int>(dto);
            var result = new ResponseResult<int>(item.Id);

            return result;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = new ResponseResult();

            if(await _functionService.CountAsync<FunctionDto>(i=>i.ParentId==id)>0)
            {
                result.Message = nameof(BusinessCode.Record_Cascade_Exist);
                result.Code = BusinessCode.Record_Cascade_Exist;
            }
            else
                await _functionService.Delete(id);


            return Ok(result);
        }
    }

}