using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Filters
{
    public class TitleFilter : IAsyncActionFilter
    {
        readonly INavigationService _navigationService;
        public TitleFilter(INavigationService navigationController)
        {
            this._navigationService = navigationController;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           
            var controller = context.Controller as Controller;
            if (controller != null)
            { 
                var navigationMenus = await _navigationService.GetAll<NavigationMenuDto>();
                var requestPath = context.HttpContext.Request.Path.ToString().ToLower();
                var current = navigationMenus.FirstOrDefault(m => requestPath.Equals(m.Url.ToLower()));

                controller.ViewData["Title"] = current?.Name;


            }
            await next();
           
        }
    }
}
