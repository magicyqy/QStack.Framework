using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Articles;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Cache;
using QStack.Framework.Core.Model;
using QStack.Framework.SearchEngine;
using QStack.Framework.SearchEngine.Imps;
using QStack.Framework.SearchEngine.Interfaces;
using QStack.Framework.Util;

namespace QStack.Blog.Controllers
{
    public class SearchController : Controller
    {
        readonly ISearchEngine _searchEngine;
        readonly IArticleService _articleService;
        readonly ICache _cache;
        public SearchController(ISearchEngine searchEngine, IArticleService articleService,ICache cache)
        {
            _searchEngine = searchEngine;
            _articleService = articleService;
            _cache = cache;
        }
        [Route("/[controller]/{q?}/{page:int?}/{size:int?}")]
        public async Task<IActionResult> Index(string q,[Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")] int page = 1, [Range(1, 50, ErrorMessage = "页大小必须在0到50之间")] int size = 20)
        {
            q = q?.Trim();
           
            ViewBag.Keyword = q;
            string key = "Search:" + HttpContext.Connection.RemoteIpAddress.ToString(); 
            if (_cache.Exists(key) && _cache.Get<string>(key) != q)
            {
                //var hotSearches = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Week").Take(10).ToList();
                //ViewBag.hotSearches = hotSearches;
                ViewBag.ErrorMsg = "10秒内只能搜索1次！";
                return View(new PageModel<ArticleDto>());
            }

         ;
            //var list1 = _luceneService.SearchMerchs(q);
            var searchResults = _searchEngine.ScoredSearch<ArticleDto>(new SearchOptions(q, page, size, new string[] { nameof(ArticleDto.Title), nameof(ArticleDto.Summary), nameof(ArticleDto.ArticleContentHtml) }));
            ViewBag.Elapsed = searchResults.Elapsed;
            var data = searchResults.Results.Select(r => r.Entity).ToList();
            var ids = data.Select(a => a.Id).ToList();
            var models=await _articleService.QueryInclude< ArticleDto>(a=>ids.Contains(a.Id),nameof(Article.ArticleContent),nameof(Article.Catagory));

          
            var pageModel = new PageModel<ArticleDto>() { Page = page, PageSize = size, TotalCount= searchResults.TotalHits};
             var highLighter = _searchEngine.GetHighLighter(fragmentSize: 200, maxNumFragments: 1);
            foreach(var model in models)
            {
                
                model.Title = highLighter.HighLight(q, model.Title?.NoHTML());
                //model.Summary = highLighter.HighLight(q, model.Summary?.NoHTML());
                model.ArticleContentHtml = highLighter.HighLight(q, model.Summary?.NoHTML()+ model.ArticleContentHtml?.NoHTML());
                pageModel.Data.Add(model);
            }
            return View(pageModel);
        }

        public async Task<IActionResult> Init()
        {
            var pageModel = await _articleService.GetArticles(1,10000, a => a.State == ArticleState.Published,"id desc",true);
            //articles = articles.Where(a => a.State == ArticleState.Published).ToList();

            //_luceneService.AddOrUpdateIndex<ArticleDto>(pageModel.Data,
            //    a => { a.ArticleContentHtml=a.ArticleContentHtml?.NoHTML(); },
            //        a=>a.Id,a=>a.CreateDate,a=>a.LastModifyDate,a=>a.Summary,a=>a.Title,a=>a.ArticleContentHtml);

            _searchEngine.CreateIndex<ArticleDto, int>(
                pageModel.Data.ToArray(),
                a => a.Id,
                new LuceneIndexBehavior<ArticleDto>()
                .Include(new Expression<Func<ArticleDto, object>>[] { a => a.Id, a => a.CreateDate, a => a.CreateDate, a => a.LastModifyDate, a => a.Summary, a => a.Title, a => a.ArticleContentHtml })
                .ForMember(a => a.ArticleContentHtml, html => html == null ? null : html.NoHTML(),null)
                .ForMember(a => a.Id,null, id => true));
            return Ok("ok");
        }


        //private Expression<Func<T,TMember>> CreateMemberSelector<T, TMember>(T target)
    }
}