using ServiceFramework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QStack.Framework.Core.Persistent
{
    public interface IDaoFactory
    {
        
      
        string FactoryName {get;}

        IDao CreateDao();
     

        /// <summary>
        /// 添加实体程序集 ,主要用于插件实体类加载<br/>
        /// 运行时添加后，efcore须通过<seealso cref="ModelCacheKeyFactory"/>方式重新触发createDao,
        /// 因为OnModelCreating会缓存
        /// </summary>
        /// <param name="assemblies"></param>
        void AddExtraEntityAssemblies(IEnumerable<Assembly> assemblies);
        void RemoveExtraEntityAssemblies(params Assembly[] assemblies);
        DaoFactoryOption DaoFactoryOption { get; }

        IDao CreateDao(SessionContext sessionContext);
    }
}
