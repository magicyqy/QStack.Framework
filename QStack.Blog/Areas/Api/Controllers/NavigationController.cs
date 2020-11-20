using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel;
using System.Linq;
using QStack.Web.Areas.Api.Controllers;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]

    public class NavigationController : ApiBaseController
    {
        INavigationService _navigationService;

        public NavigationController(INavigationService navigationService)
        {
            this._navigationService = navigationService;
        }

        public async Task<ResponseResult<List<NavigationMenuDto>>> GetMenus()
        {
            var result = new ResponseResult<List<NavigationMenuDto>>();

            var list = (await _navigationService.GetAll<NavigationMenuDto>()).OrderBy(n=>n.FlowNo).ToList();
            list = GetForTree(list, null);
            result.Data = list;

            return result;
        }

        public async Task<ResponseResult<int>> PostMenu(NavigationMenuDto dto)
        {


            var item = await _navigationService.AddOrUpdate<NavigationMenuDto,int>(dto);
            var result = new ResponseResult<int>(item.Id);

            return result;
        }

        public async Task<IActionResult> DeleteMenu(int id)
        {
            var result = new ResponseResult();

          
            await _navigationService.DeleteByIdAsync(id);


            return Ok(result);
        }
    }
}