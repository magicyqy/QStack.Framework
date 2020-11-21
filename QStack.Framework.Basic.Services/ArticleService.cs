using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Articles;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Model;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    [SessionInterceptor]

    public class ArticleService : AbstractService<Article>, IArticleService
    {

        public ArticleService(IMapper mapper)
        {

            Mapper = mapper;

        }

        #region article
        [TransactionInterceptor]
        public async Task AddPageViews(int id)
        {
            var entity = await Daos.CurrentDao.SingleAsync<Article>(d => d.Id == id);

            entity.PageViews++;
           await  Daos.CurrentDao.Flush();
        }
        //[TransactionInterceptor]
        public async Task AddZan(int id)
        {
            var entity = await Daos.CurrentDao.SingleAsync<Article>(d => d.Id==id);

            entity.ZanNum++;
            await Daos.CurrentDao.Flush();
        }


        public async Task<ArticleDto> GetArticle(int id)
        {
            List<Expression<Func<Article, object>>> expressions = new List<Expression<Func<Article, object>>>();
            expressions.Add(a => a.Catagory);
            expressions.Add(a => a.ArticleTags.Select(at => at.Tag));

            expressions.Add(a => a.ArticleContent);
            var articles = await Daos.CurrentDao.QueryInclude<Article>(a => a.Id == id, expressions.ToArray());

            return Mapper.Map<ArticleDto>(articles.FirstOrDefault());
        }

    
        public async Task<PageModel<ArticleDto>> GetArticles(int page = 1, int size = 20, Expression<Func<ArticleDto, bool>> filterExp = null, string orderBy = null, bool includeContent = false)
        {
            var newFilterExp = Mapper.MapExpression<Expression<Func<Article, bool>>>(filterExp);

            return await GetArticles(page, size, newFilterExp, orderBy, includeContent);
        }


        public async  Task<PageModel<ArticleDto>> GetArticlesByTag(int page = 1, int size = 20,int tagId=0, Expression<Func<ArticleDto, bool>> filterExp = null, string orderBy = null, bool includeContent = false)
        {
            var newFilterExp = Mapper.MapExpression<Expression<Func<Article, bool>>>(filterExp);
            newFilterExp = PredicateBuilder.And<Article>(a => a.ArticleTags.Any(at => at.TagId == tagId),newFilterExp);
            return await GetArticles(page, size, newFilterExp, orderBy, includeContent);
        }

        private async Task<PageModel<ArticleDto>> GetArticles(int page=1,int size=20,Expression<Func<Article,bool>> filterExp=null, string orderBy=null, bool includeContent = false)
        {
            List<Expression<Func<Article, object>>> expressions = new List<Expression<Func<Article, object>>>();
            expressions.Add(a => a.Catagory);
            expressions.Add(a => a.ArticleTags.Select(at => at.Tag));
            if (includeContent)
                expressions.Add(a => a.ArticleContent);
            var pageModel = await Daos.CurrentDao.QueryPage<Article>(page, size, filterExp, expressions.ToArray(), orderBy);

            return new PageModel<ArticleDto>()
            {
                Data = Mapper.Map<List<Article>, List<ArticleDto>>(pageModel.Data),
                TotalCount = pageModel.TotalCount,
                Page = pageModel.Page,
                PageSize = pageModel.PageSize
               
            };
        }


        [TransactionInterceptor]
        public async Task<int> AddOrUpdateArticle(ArticleDto articleDto)
        {
            var id = 0;
            var article = Mapper.Map<Article>(articleDto);
            if (article.CatagoryId.HasValue)
                article.Catagory.Id = article.CatagoryId.Value;
            else
                article.Catagory = null;
            if (article.Id <= 0)
            {
                article.CreateDate = DateTime.Now;
                id = (await Daos.CurrentDao.AddAsync(article)).Id;
            }
            else
            {
                var entity = await Daos.CurrentDao.AddOrUpdate<Article, int>(article);
                //非从属表的导航属性另外维护
                var allkeys = article.ArticleTags.Select(at => at.TagId);
                entity.ArticleTags = entity.ArticleTags.Where(a => allkeys.Contains(a.TagId)).ToList();
                var existKeys = entity.ArticleTags.Select(at => at.TagId);
                foreach (var newItem in article.ArticleTags.Where(at => !existKeys.Contains(at.TagId)))
                {

                    entity.ArticleTags.Add(newItem);
                }

                await Daos.CurrentDao.Flush();
                //await Daos.CurrentDao.AddRangeAsync(article.ArticleTags);

                //if (article.ArticleContent?.Article == null)
                //    article.ArticleContent.Article = article;
                id = entity.Id;
            }
            return id;
        }

        public async Task<Dictionary<DateTime, int>> StatisticsByMonth()
        {
            //此处groupby 如果是datetimeoffset? 表达式不通过，所以导致将整个实体类都换回datetime?
            var dbset =Daos.CurrentDao.DbSet<Article>().Where(a=>a.State==Enum.ArticleState.Published).Select(a=>new { a.PublishTime.Value.Year, a.PublishTime.Value.Month, a.Id });

            var groupby = dbset.GroupBy(a => new { a.Year,a.Month });
            var results= groupby.Select(g => new { Date=new DateTime(g.Key.Year,g.Key.Month,1),  Count = g.Count() });

            Dictionary<DateTime, int> dic = new Dictionary<DateTime, int>();
            foreach(var item in results)
            {
                dic.TryAdd(item.Date, item.Count);
            }

            return await Task.FromResult(dic);
        }

        #endregion
        #region 类别catagory
        public async Task<List<CatagoryDto>> GetCatagories()
        {


            var list = await Daos.CurrentDao.Query<Catagory>();

            return Mapper.Map<List<CatagoryDto>>(list);
        }

        public async Task<int> PostCatagory(CatagoryDto dto)
        {

            return (await Daos.CurrentDao.AddOrUpdate<Catagory, int>(Mapper.Map<Catagory>(dto))).Id;
        }
        [TransactionInterceptor]
        public async Task<bool> DeleteCatagory(int id)
        {
           return await Daos.CurrentDao.DeleteById<Catagory>(id);
        }
        #endregion

        #region 标签tag
        public async Task<List<TagDto>> GetTags()
        {


            var list = await Daos.CurrentDao.Query<Tag>();

            return Mapper.Map<List<TagDto>>(list);
        }

        public async Task DeleteTag(int id)
        {
            var isCascade = await IsCascadeTag(id);

            await Daos.CurrentDao.DeleteById<Tag>(id);
        }

        public async Task<bool> IsCascadeTag(int id)
        {
            var isCascade = Daos.CurrentDao.DbSet<Article>().Any(a => a.ArticleTags.Count(t => t.TagId == id) > 0);

            return await Task.FromResult(isCascade);
        }
        #endregion

    }
}
