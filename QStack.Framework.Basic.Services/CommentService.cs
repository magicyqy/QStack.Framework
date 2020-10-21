using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Articles;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Entity;
using QStack.Framework.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services
{
    [QStack.Framework.Core.Persistent.SessionInterceptor]
    public class CommentService:AbstractService<Comment>,ICommentService
    {
        public CommentService(IMapper mapper)
        {
            Mapper = mapper;
        }


        public async Task<PageModel<CommentDto>> GetCommentPage(int pageIndex=1,int pageSize=5,int? articleId=null,int? commentId=null)
        {
            var exp = PredicateBuilder.True<Comment>();
            if (articleId > 0)
                exp = PredicateBuilder.And<Comment>(exp, c => c.ArticleId == articleId);
            if(commentId>0)
                exp = PredicateBuilder.And<Comment>(exp, c => c.ParentId == commentId);
            else
                exp = PredicateBuilder.And<Comment>(exp, c => c.ParentId == null);
            var queryable = Daos.CurrentDao.DbSet<Comment>().Where(exp).OrderBy(c=>c.CreateTime).Select(c => new CommentWithChildren
            {
                Comment = c,
                Children = c.Children.OrderBy(c=>c.CreateTime).Take(pageSize),
                ChildrenCount=c.Children.Count()

            });
            //var items = queryable.ToList();
            var pageResult = await  Daos.CurrentDao.QueryPage<CommentWithChildren>(queryable, pageIndex, pageSize);
            var pageModel = new PageModel<CommentDto>()
            {
                Data =Mapper.Map<List<CommentDto>>( pageResult.Data.Select(d=> { d.Comment.ChildrenCount = d.ChildrenCount; return d.Comment; }).ToList()),
              
                Page = pageResult.Page,
                PageSize = pageResult.PageSize,
                TotalCount = pageResult.TotalCount
            };
            pageModel.ActualTotalCount = pageResult.TotalCount + pageModel.Data.Sum(d => d.ChildrenCount);
            var pp = pageModel.PageCount;
            return pageModel;
        }
    }


    class CommentWithChildren
    {
        public Comment Comment { get; set; }
        public IEnumerable<Comment> Children { get; set; }
        public int ChildrenCount { get; set; }
    }
}
