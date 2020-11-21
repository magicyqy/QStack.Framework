using QStack.Blog.Areas.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Basic.ViewModel.Articles;

using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QStack.Framework.Core.Model;

namespace QStack.Blog.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        readonly NavigationController _navigationController;
        public NavigationViewComponent(NavigationController navigationController)
        {
            this._navigationController = navigationController;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result =await _navigationController.GetMenus();

            var breadCrumbs=   GenerateBreadCrumb(result.Data);
           

            return View(Tuple.Create(result.Data, breadCrumbs));
        }

        private List<NavigationMenuDto> GenerateBreadCrumb(List<NavigationMenuDto> menus)
        {
            var list = new List<NavigationMenuDto>();
            var currentController = ViewContext.RouteData.Values["Controller"]?.ToString().ToLower() ;
            int? catagoryId=null;
            bool isArticle = false;
            switch (currentController)
            {
                case "catagory":
                    var id = ViewContext.RouteData.Values["id"]?.ToString();
                    catagoryId =Convert.ToInt32(id);
                    break;
                case "article":
                    catagoryId = (ViewContext.ViewData.Model as ArticleDto).CatagoryId;
                    isArticle = true;
                    break;
                case "tag":
                    var tid = Convert.ToInt32(ViewContext.RouteData.Values["id"]);
                    var articleDto = (ViewContext.ViewData.Model as PageModel<ArticleDto>).Data.FirstOrDefault();
                    var tagName = articleDto?.Tags?.FirstOrDefault(t => t.Id == tid)?.Name;
                   
                    if(tagName.IsNotNullAndWhiteSpace())
                        list.Add(new NavigationMenuDto { Name = tagName });
                    list.Add(new NavigationMenuDto { Name = "标签" });
                    break;
                case "shop":
                   var menuName=  menus.FirstOrDefault(m=>"/shop".Equals(m.Url.ToLower()))?.Name;
                   list.Add(new NavigationMenuDto { Name = menuName });
                    break;
                case "search":
                    list.Add(new NavigationMenuDto { Name = $"搜索\"{ViewContext.HttpContext.Request.Query["q"]}\"" });
                    break;
                default:
                    break;
            } 
          
        
            if (catagoryId.HasValue)
            {
              
                var path = "/catagory/" + catagoryId;
                
                GetTreePath(menus, path, list);
                if(list.Count()>0)
                   GetParentPath(menus, list[0].ParentId, list);
                
            }
            list.Reverse();
            if (isArticle)
                list.Add(new NavigationMenuDto { Name = "文章" });
            return list;
        }

        private void GetTreePath(List<NavigationMenuDto> menus, string path, List<NavigationMenuDto> list)
        {
            foreach (var node in menus)
            {
               
                if (node.Url.ToLower().Equals(path))
                {
                    list.Add(node);
                    break;
                }
                else
                {
                    if (node.Children!=null&&node.Children.Count()>0)
                    {
                        GetTreePath(node.Children, path, list);
                    }
                }
            }
            
        }

        private void GetParentPath(List<NavigationMenuDto> menus, int? nodeId, List<NavigationMenuDto> list)
        {

            foreach (var node in menus)
            {

                if (node.Id== nodeId)
                {
                    list.Add(node);

                    GetParentPath(menus, node.ParentId, list);
                    break;
                }
                else
                {
                    if (node.Children != null && node.Children.Count() > 0)
                    {
                        GetParentPath(node.Children, nodeId, list);
                    }
                }
            }
           
        }
    }
}
