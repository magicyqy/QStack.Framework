using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Model.ViewModel.Auth;
using QStack.Framework.Core.Cache;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IDataAuthService : IBaseService
    {
        [Caching(CacheMethod.Get)]
        Dictionary<string, List<EntityInfoDto>> GetDaoFactoryEntityInfo();
        /// <summary>
        /// 获取指定实体的相应的数据权限规则表达式
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="factoryName">数据仓库</param>
        /// <param name="ruleState">规则状态，默认为使用中</param>
        /// <returns>如果不存在返回null</returns>
        System.Threading.Tasks.Task<System.Linq.Expressions.Expression<Func<T, bool>>> GetExpression<T>(string factoryName, DataAuthRuleState ruleState = DataAuthRuleState.Using) where T : EntityBase;
        Task InitQueryFilters();
        Task<int> UpdateState(int id, DataAuthRuleState state);
    }
}
