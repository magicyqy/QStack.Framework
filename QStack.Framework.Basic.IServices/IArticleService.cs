using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Cache;
using QStack.Framework.Core.Entity;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IArticleService : IBaseService
    {

        [Caching(CacheMethod.Remove,new string[]{ "GetArticles", "GetTags" })]
        Task<int> AddOrUpdateArticle(ArticleDto articleDto);
        Task AddPageViews(int id);

        Task AddZan(int id);
        Task<bool> DeleteCatagory(int id);
        Task<ArticleDto> GetArticle(int id);
        [Caching(CacheMethod.Get)]
        Task<PageModel<ArticleDto>> GetArticles(int page = 1, int size = 20, Expression<Func<ArticleDto, bool>> filterExp = null,string orderby=null, bool includeContent = false);
        Task<PageModel<ArticleDto>> GetArticlesByTag(int page = 1, int size = 20, int tagId = 0, Expression<Func<ArticleDto, bool>> filterExp = null, string orderBy = null, bool includeContent = false);
        Task<List<CatagoryDto>> GetCatagories();
        [Caching(CacheMethod.Get)]
        Task<List<TagDto>> GetTags();
        Task<int> PostCatagory(CatagoryDto dto);
        [Core.Cache.Caching(Core.Cache.CacheMethod.Get)]
        Task<Dictionary<DateTime, int>> StatisticsByMonth();
        [Caching(CacheMethod.Remove, new string[] { "GetTags" })]
        Task DeleteTag(int id);
        Task<bool> IsCascadeTag(int id);
    }
}
