
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QStack.Framework.Core;
using QStack.Framework.Core.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QStack.Framework.Persistent.EFCore
{
    
    public class EFCoreDaoFactory : IDaoFactory
    {
       
        private readonly DaoFactoryOption _options;
        private readonly DbContextOptionsBuilder _dbContextOptionsBuilder;
       

        public  DbContextOptions dbContextOptions => _dbContextOptionsBuilder.Options;
        public EFCoreDaoFactory(DaoFactoryOption options, DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            _options = options;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public string FactoryName => _options?.FactoryName;

        public IDao CreateDao()
        {
            return CreateDao(null,null);
         
        }

        public IDao CreateDao(Func<DbContextOptions, DbContextOptions> configureDbContextOptions = null, SessionContext sessionContext=null)
        {
            if (_dbContextOptionsBuilder == null)
                throw new ServiceFrameworkException($"create {this.GetType().FullName} failed, the DbContextOptionsBuilder can not be null.");
            
            var dbOptions  =configureDbContextOptions?.Invoke(_dbContextOptionsBuilder.Options);
           
            return new EFCoreDao(dbOptions?? _dbContextOptionsBuilder.Options, _options, sessionContext);
        }

        public IDao CreateDao(SessionContext sessionContext)
        {
           return CreateDao(null, sessionContext);
        }

        public void AddExtraEntityAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies != null)
            {
                foreach(var item in assemblies)
                    _options.EntityAssemblies.Add(item);
                _options.UpdateCacheVersion();
            }
        }

       

        public void RemoveExtraEntityAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                return;
            var names = assemblies.Select(x => x.GetName().Name);

            _options.EntityAssemblies.RemoveWhere(a => names.Contains(a.GetName().Name));
            _options.UpdateCacheVersion();
        }
       
        public DaoFactoryOption DaoFactoryOption => _options;

       
    }
}
