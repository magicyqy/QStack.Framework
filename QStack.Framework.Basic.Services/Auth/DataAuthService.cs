using AutoMapper;
using Microsoft.Extensions.Configuration;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.Model.ViewModel.Auth;
using QStack.Framework.Core;
using QStack.Framework.Core.Cache;
using QStack.Framework.Core.DataPrivilege;
using QStack.Framework.Core.Model;
using QStack.Framework.Core.Persistent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services.Auth
{
    [SessionInterceptor]
    public class DataAuthService:AbstractService<DataAuthRule>,IDataAuthService
    {
        private IEnumerable<IDaoFactory> daoFactories;
        private IEnviromentContext enviroment;
        private ICache cache;
        IConfiguration configuration;
        private  string keyPrefix => configuration["CacheSettings:KeyPrefix"] ?? "SFCache";
        private  string DATAAUTHRULE_CACHE_KEY=> $"{keyPrefix}_{nameof(DataAuthService)}_{nameof(DataAuthService.InitQueryFilters)}";
        SessionContext sessionContext;
        public DataAuthService(IMapper mapper,
            IEnumerable<IDaoFactory> daoFactories,
            IEnviromentContext enviroment,
            ICache cache, IConfiguration configuration,SessionContext sessionContext)
        {
            Mapper = mapper;
            this.daoFactories = daoFactories;
            this.enviroment = enviroment;
            this.cache = cache;
            this.configuration = configuration;
            this.sessionContext = sessionContext;
        }

        /// <summary>
        /// 获取所有数据库工厂名及关联的实体对象
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,List<EntityInfoDto>> GetDaoFactoryEntityInfo()
        {
            var result = new Dictionary<string, List<EntityInfoDto>>();
            if (this.daoFactories != null)
            {

                foreach(var factory in daoFactories)
                {
                    var clrTypes = Daos[factory.FactoryName].GetEntityTypes().Where(t => typeof(EntityBase).IsAssignableFrom(t));
               
                     var infoList= clrTypes.Select(t => new EntityInfoDto { EntityName = t.Name, EntityType = t.AssemblyQualifiedName, Name = t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName??t.Name });
                                 
                    if(!result.ContainsKey(factory.FactoryName))
                    {
                        result.Add(factory.FactoryName, infoList.ToList());
                    }
                }
            }
            return result;
        }

        public async Task<Expression<Func<T,bool>>> GetExpression<T>(string factoryName, DataAuthRuleState ruleState=DataAuthRuleState.Using) where T : EntityBase
        {
            var rule =await Daos.CurrentDao.SingleAsync<DataAuthRule>(d => d.Repository == factoryName && typeof(T).AssemblyQualifiedName == d.EntityType&&d.RuleState== ruleState);
            if (rule == null)
                return null;
            var ruleGroup = JsonSerializer.Deserialize<RuleGroup>(rule.RuleGroup);
          
            var envDic = enviroment.ToEnviromentDictionary();


            return (Expression<Func<T, bool>>)ruleGroup.GetExpression(envDic);
        }

        public async Task InitQueryFilters()
        {
            List<RuleGroup> ruleGroups = await cache.GetAsync(this.DATAAUTHRULE_CACHE_KEY,
                async () => {
                    var list = await Daos.CurrentDao.Query<DataAuthRule>(d => d.RuleState == DataAuthRuleState.Using);

                    List<RuleGroup> ruleGroups = new List<RuleGroup>();
                    foreach (var item in list)
                    {
                        ruleGroups.Add(JsonSerializer.Deserialize<RuleGroup>(item.RuleGroup));
                    }
                    return ruleGroups;
                }, TimeSpan.FromMinutes(1800));
            var expressions = new List<LambdaExpression>();
            var envDic = enviroment.ToEnviromentDictionary();
            foreach(var rule in ruleGroups)
            {
                expressions.Add(rule.GetExpression(envDic));
            }
            sessionContext.QueryFilters = expressions;
            sessionContext.EnviromentContext = enviroment;
            //sessionContext.Clear();//强制清空缓存的dao
            
        }

        public async Task<int> UpdateState(int id,DataAuthRuleState state)
        {
            int ruleId = await Daos.CurrentDao.Update<DataAuthRule>(i => i.Id == id, i => new DataAuthRule { RuleState = state });
            cache.Remove(this.DATAAUTHRULE_CACHE_KEY);
            return ruleId;
        }
    }
}
