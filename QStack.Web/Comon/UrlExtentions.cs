using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Comon
{
    public static class UrlExtentions
    {
        public static string GetArticleUrl(this IUrlHelper url,int id)
        {
            return $"/article/{id}.html";
        }
        public static string GetProductUrl(this IUrlHelper url, int id)
        {
            return $"{url.RouteUrl("default",new { controller = "shop", action = "detail", id = id })}.html";
        }
    }
}
