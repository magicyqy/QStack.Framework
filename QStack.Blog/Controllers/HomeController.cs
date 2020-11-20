using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Basic.Enum;

namespace QStack.Blog.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly IArticleService _articleService;

        public HomeController(IArticleService articleService)
        {
            
            this._articleService = articleService;
        }

        public virtual async  Task<IActionResult> Index(int page=1,int pageSize=10)
        {
           
            var result =await _articleService.GetArticles(page, pageSize, a=>a.State==ArticleState.Published, $"{nameof(ArticleDto.PublishTime)} desc");

            return View(result);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
