using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using QStack.Blog.Docker.Crawler.Dtos;
using QStack.Blog.Docker.Crawler.Models;
using QStack.Framework.Basic.Services;

using QStack.Framework.Core.Model;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Services
{
    [SessionInterceptor]
    public class SpiderAgentService : AbstractService<AgentInfo>, ISpiderAgentService
    {
        public SpiderAgentService(IMapper mapper)
        {
            Mapper = mapper;
        }
        [TransactionInterceptor]
        public async Task DeleteAgentInfo(string id)
        {
            await Daos.CurrentDao.DeleteAsync<AgentHeartbeat>(a => a.AgentId == id);

            await Daos.CurrentDao.DeleteById<AgentInfo>(id);
        }

        public async Task<PageModel<AgentHeartbeatDto>> GetSpiderAgentHeartBeats(int page, int size, Expression<Func<AgentHeartbeatDto, bool>> filterExpression = null, string orderbys = null)
        {
            var expression = Mapper.MapExpression<Expression<Func<AgentHeartbeat, bool>>>(filterExpression);

            var pageModel = await Daos.CurrentDao.QueryPage<AgentHeartbeat>(page, size, expression, new List<string>(), orderbys);
            return Mapper.Map<PageModel<AgentHeartbeatDto>>(pageModel);
        }

        public async Task<PageModel<AgentInfoDto>> GetSpiderAgentInfos(int page, int size, Expression<Func<AgentInfoDto, bool>> filterExpression = null, string orderbys = null)
        {
            var expression = Mapper.MapExpression<Expression<Func<AgentInfo, bool>>>(filterExpression);

            var pageModel = await Daos.CurrentDao.QueryPage<AgentInfo>(page, size, expression, new List<string>(), orderbys);
            return Mapper.Map<PageModel<AgentInfoDto>>(pageModel);
        }
    }
}
