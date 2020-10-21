using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Core.Entity;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface ICommentService : IBaseService
    {
        Task<PageModel<CommentDto>> GetCommentPage(int pageIndex = 1, int pageSize = 10, int? articleId = null, int? commentId = null);
    }
}
