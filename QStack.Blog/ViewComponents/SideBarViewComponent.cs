using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.ViewComponents
{
    public class SideBarViewComponent: ViewComponent
    {
        readonly IArticleService _articleService;
        public SideBarViewComponent(IArticleService articleService)
        {
            this._articleService = articleService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var result = await _articleService.GetArticles(1, 6, a => a.State == ArticleState.Published, $"{nameof(ArticleDto.HotTop)} desc,{nameof(ArticleDto.PageViews)} desc");
            var tags = await _articleService.GetTags();

            var statistics = await _articleService.StatisticsByMonth();

            return View(Tuple.Create(result.Data,tags, statistics));
        }
    }
}
