using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Articles;
using QStack.Framework.Basic.Services;
using QStack.Framework.Basic.ViewModel.Articles;

namespace QStack.Blog.Controllers
{
    public class TagController : Controller
    {
        private readonly IArticleService _articleService;
        public TagController(IArticleService articleService)
        {
            this._articleService = articleService;
        }
        [Route("/[controller]/{id:int}")]
        public async Task<IActionResult> Index(int id, int page = 1, int pageSize = 10)
        {
            var model = await _articleService.GetArticlesByTag(page, pageSize, id, a => a.State == ArticleState.Published, nameof(ArticleDto.PublishTime) + " desc");
            return View("/Views/Home/Index.cshtml", model);
        }
    }
}