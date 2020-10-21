using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QStack.Blog.Areas.Api.Models;
using Microsoft.AspNetCore.Mvc;

using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Entity;
using QStack.Framework.SearchEngine.Interfaces;
using QStack.Framework.SearchEngine.Imps;
using System.Linq.Expressions;
using QStack.Framework.Util;
using QStack.Framework.Basic;

namespace QStack.Blog.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]

    public class ArticleController : ApiBaseController
    {
        private readonly IArticleService _articleService;
        readonly ISearchEngine _searchEngine;
        public ArticleController(IArticleService articleService,ISearchEngine searchEngine)
        {
            this._articleService = articleService;
            _searchEngine = searchEngine;
        }

        #region article
        public async Task<IActionResult> Post(ArticleDto articleDto)
        {
            bool isNew = articleDto.Id <= 0 ? true : false ;


            if (articleDto.State == ArticleState.Published&&!articleDto.PublishTime.HasValue)
                articleDto.PublishTime = DateTime.Now;
            var id=await _articleService.AddOrUpdateArticle(articleDto);
            Task.Run(
                
                async () => {
                    var article =await _articleService.GetArticle(id);
                    var indexConfig = new LuceneIndexable<ArticleDto, int>(
                            article,
                            article.Id,
                            new LuceneIndexBehavior<ArticleDto>()
                                .Include(new Expression<Func<ArticleDto, object>>[] { a => a.Id, a => a.CreateDate, a => a.CreateDate, a => a.LastModifyDate, a => a.Summary, a => a.Title, a => a.ArticleContentHtml })
                                .ForMember(a => a.ArticleContentHtml, html => html == null ? null : html.NoHTML(), a=>true)
                                .ForMember(a => a.Id, null, id => true)
                        );
                    if (isNew)
                        _searchEngine.LuceneIndexer.Add(indexConfig);
                    else
                        _searchEngine.LuceneIndexer.Update(indexConfig);
                }
            );
            var result = new ResponseResult<int>(id);
          
            return Ok(result);
        }

       
        public async Task<IActionResult> Get(int id)
        {

            var result = new ResponseResult<ArticleDto>();

            var articleDto = await _articleService.GetArticle(id);

            if (articleDto == null)
            {
                result.Message = nameof(BusinessCode.Record_NotFound);
                result.Code = BusinessCode.Record_NotFound;
            }
            result.Data = articleDto;
           
     
            return Ok(result);
        }

        public async Task<ResponseResult<PageModel<ArticleDto>>> GetArticles(DataTableOption query)
        {
            //var result = new ResponseResult<PageModel<ArticleDto>>();
            //var exp = PredicateBuilder.True<ArticleDto>();
            //if (articleSearch.CatagoryId > 0)
            //    exp=exp.And(a => a.CatagoryId == articleSearch.CatagoryId);
            //if(articleSearch.DateRange!=null && articleSearch.DateRange.Length>0&&articleSearch.DateRange[0].HasValue)
            //    exp = exp.And(a => a.CreateDate >= articleSearch.DateRange[0].Value);
            //if (articleSearch.DateRange != null && articleSearch.DateRange.Length > 1 && articleSearch.DateRange[1].HasValue)
            //    exp = exp.And(a => a.CreateDate<= articleSearch.DateRange[1].Value);
            //var articleDtos = await _articleService.GetArticles(articleSearch.Page, articleSearch.Size,exp);

            //result.Data = articleDtos;


            return await base.SearchQuery<ArticleDto,IArticleService>(query);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = new ResponseResult();

            await _articleService.DeleteByIdAsync(id);

           
            return Ok(result);
        }

        public async Task<IActionResult> PostHotTop(int aid,int hotTop)
        {

            await _articleService.Update<ArticleDto>(a => a.Id == aid, a => new ArticleDto { HotTop =hotTop });

            return Ok(new ResponseResult());
        }
        public async Task<IActionResult> PostState(int aid, ArticleState state)
        {
            var article = await _articleService.Get<ArticleDto>(a => a.Id == aid);
            if (!article.PublishTime.HasValue && state == ArticleState.Published)
                await _articleService.Update<ArticleDto>(a => a.Id == aid, a => new ArticleDto { State = state, PublishTime = DateTime.Now });
            else
                await _articleService.Update<ArticleDto>(a => a.Id == aid, a => new ArticleDto
                {
                    State = state
                });

            return Ok(new ResponseResult());
        }

        #endregion

        #region catagory

        public async Task<IActionResult> GetCatagories(int? id)
        {
            var result = new ResponseResult<List<CatagoryDto>>();

            var list = await _articleService.GetCatagories();
            list = GetForTree(list, null);
            result.Data = list;

            return Ok(result);
        }

        public async Task<IActionResult> PostCatagory(CatagoryDto catagoryDto)
        {
          
              
            var id= await _articleService.PostCatagory(catagoryDto);
            var result = new ResponseResult<int>(id);

            return Ok(result);
        }

        public async Task<IActionResult> DeleteCatagory(int id)
        {
            var result = new ResponseResult();
            
            if(await _articleService.Count<ArticleDto>(a=>a.CatagoryId==id)>0)
            {
                result.Message = nameof(BusinessCode.Record_Cascade_Exist);
                result.Code = BusinessCode.Record_Cascade_Exist;
            }
            else
                await _articleService.DeleteCatagory(id);


            return Ok(result);
        }
        #endregion

        #region tag
        public async Task<IActionResult> GetTags()
        {
            var result = new ResponseResult<List<TagDto>>();

            var tags = await _articleService.GetTags();

            result.Data = tags;

            return Ok(result);
        }
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = new ResponseResult();

            if (await _articleService.IsCascadeTag(id))
            {
                result.Message = nameof(BusinessCode.Record_Cascade_Exist);
                result.Code = BusinessCode.Record_Cascade_Exist;
            }
            else
                await _articleService.DeleteTag(id);


            return Ok(result);
        }
        #endregion

        #region other
        public async Task<IActionResult> Zan(int id)
        {
            await _articleService.AddZan(id);

            return Ok(new ResponseResult());
        }
        #endregion

    }
}