using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.IServices;

namespace QStack.Blog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        public ArticleController(IArticleService articleService)
        {
            this._articleService = articleService;
        }
        [Route("/[controller]/{id:int}.html")]
        public async Task<IActionResult> Article(int id)
        {
            var article = await _articleService.GetArticle(id);
            await _articleService.AddPageViews(id);
            return View(article);
        }
    }
}