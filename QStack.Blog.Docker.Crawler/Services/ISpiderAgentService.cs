using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Core.Entity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Services
{
    public interface ISpiderAgentService:IBaseService
    {
        Task<PageModel<AgentInfoDto>> GetSpiderAgentInfos(int page, int size, Expression<Func<AgentInfoDto, bool>> filterExpression = null, string orderbys = null);
        Task<PageModel<AgentHeartbeatDto>> GetSpiderAgentHeartBeats(int page, int size, Expression<Func<AgentHeartbeatDto, bool>> filterExpression = null, string orderbys = null);
        Task DeleteAgentInfo(string id);
    }
}