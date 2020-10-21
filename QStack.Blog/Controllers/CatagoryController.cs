using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Util;

namespace QStack.Blog.Controllers
{
    public class CatagoryController : Controller
    {
        private readonly IArticleService _articleService;

        public CatagoryController (IArticleService articleService)
        {

            this._articleService = articleService;
        }
        [Route("/[controller]/{id:int?}")]
        public async Task<IActionResult> Index(int id=0,int page = 1, int pageSize = 10)
        {
            var catagorylist = await _articleService.GetCatagories();
            List<int> ids = new List<int>();
            if (id > 0)
            {
                ids = GetSonNodes(id, catagorylist).Select(c => c.Id).ToList();
                ids.Add(id);
            }
              
            else
                ids = GetSonNodes(null, catagorylist).Select(c => c.Id).ToList();
            
            var exp = PredicateBuilder.True<ArticleDto>(c => c.State == ArticleState.Published);
            if (ids.Count() > 0)
                exp = PredicateBuilder.And<ArticleDto>(exp, c => ids.Contains(c.CatagoryId.Value));
            var result = await _articleService.GetArticles(page, pageSize, exp, $"{nameof(ArticleDto.PublishTime)} desc");
            //var currentCatagory= catagorylist.FirstOrDefault(c=>c.Id==id);
            return View(result);
        }

        private List<CatagoryDto> GetSonNodes(int? id, List<CatagoryDto> nodeList)
        {
            var query = nodeList.Where(c => c.ParentId == id);

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonNodes(t.Id, nodeList))).ToList();
        }
    }
}