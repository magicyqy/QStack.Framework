using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace QStack.Web.Areas.Api.Controllers
{
    [Area("api")]
   
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ApiBaseController : ControllerBase
    {
       
        public UserDto CurrentUser
        {
            get
            {
                if(User?.Identity.IsAuthenticated==false)
                    throw new UnauthorizedAccessException("login time out");
                var clains = User?.Claims;
               
                   
                return new UserDto
                {
                    Id = Convert.ToInt32(clains.FirstOrDefault(c => c.Type.Equals("Id"))?.Value),
                    Name = clains.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value
                };
            }
        }

        /// <summary>
        /// 更新记录创建人或修改人<br/>
        /// 已废弃，公用字段更新已用dao中的ChangeTracker跟踪实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        [Obsolete]
        protected void UpdateModel<T>(T dto) where T : BaseDto
        {
            if (dto.CreateUserId <= 0)
            {
                dto.CreateUserId = CurrentUser?.Id;
                dto.CreateDate = DateTime.Now;
            }
            else
            {
                dto.LastModifyDate = DateTime.Now;
                dto.LastModifyUserId = CurrentUser?.Id;
            }
        }
       
        [NonAction]
        public virtual JsonResult Json(object data)
        {
            return new JsonResult(data);
        }
        [NonAction]
        public virtual JsonResult Json(object data, JsonSerializerSettings serializerSettings)
        {
            return new JsonResult(data, serializerSettings);
        }
       
        //public  string CurrentHost =>$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        [NonAction]
        public  string UriCombine(string baseUri, string relativeUri)
        {
            if (!baseUri.EndsWith("/"))
            {
                baseUri = baseUri + "/";
            }
            var uri = new Uri(baseUri);
            return new Uri(uri, relativeUri).AbsoluteUri;
        }
        [NonAction]
        protected List<T> GetForTree<T>(List<T> list, int? parentId)
        {
            List<T> tempList = new List<T>();
            foreach (T co in list)
            {
                var property = typeof(T).GetProperty("ParentId");
                var value = property.GetValue(co);
                if (Convert.ToInt32(value) == parentId||(value==null&&parentId==null))
                {
                    var propertyChildren = typeof(T).GetProperty("Children");
                    var propertyId = typeof(T).GetProperty("Id");
                    propertyChildren.SetValue(co, GetForTree(list, Convert.ToInt32(propertyId.GetValue(co))));
                    tempList.Add(co);

                    continue;
                }
            }
            return tempList;
        }

        /// <summary>
        /// 通用查询方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        [NonAction]
        public virtual async Task<ResponseResult<PageModel<T>>> SearchQuery<T,TService>(DataTableOption query) where T:class where TService:IBaseService
        {
            var service = HttpContext.RequestServices.GetService<TService>();
            var result = new ResponseResult<PageModel<T>>();
            //var pagin = new PageModel<T> { PageSize = query.Length, Page = query.Start / query.Length };
            var expression = query.AsExpression<T>();
            var order = query.GetOrderBy<T>();

            result.Data =await service.QueryPage(expression, query.PageIndex, query.PageSize, order,query.IncludePaths);
           
            return result;

        }
       
    }
}