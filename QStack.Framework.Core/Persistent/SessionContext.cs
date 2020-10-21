
#define DEBUG_SESSIONCONTEXT

using Microsoft.Extensions.DependencyInjection;
using ServiceFramework.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace QStack.Framework.Core.Persistent
{
    public delegate void CreateDaoHandler(IDao dao);

    public class SessionContext:IDisposable,IAsyncDisposable
    {
        ConcurrentDictionary<string, object> AdditionalData { get; } = new ConcurrentDictionary<string, object>();

        public IEnviromentContext EnviromentContext { get; set; }
        public IEnumerable<LambdaExpression> QueryFilters { get; set; } = new List<LambdaExpression>();
        public Dictionary<string, IDao> Daos = new Dictionary<string, IDao>();

        public event CreateDaoHandler CreateDaoEvent;

        public Exception Exception;
      
        IEnumerable<IDaoFactory> daoFactories;
        public SessionContext(IServiceProvider serviceProvider)
        {
           
            daoFactories = serviceProvider.GetServices<IDaoFactory>();
        }
        
        public IDao GetDao(string factoryName)
        {
            if (Daos.ContainsKey(factoryName))
            {
#if DEBUG_SESSIONCONTEXT
                Console.WriteLine("获取Dao:{0}", Daos[factoryName].GetHashCode());
#endif
                return Daos[factoryName];
            }
            else
            {
              
                IDaoFactory daoFactory = daoFactories.First(dao => dao.FactoryName == factoryName);
                            
                IDao dao  = daoFactory.CreateDao(this);              

                Daos.Add(factoryName, dao);

                CreateDaoEvent?.Invoke(dao);

#if DEBUG_SESSIONCONTEXT
                Console.WriteLine($"创建并获取Dao:{dao.GetHashCode()} -{Thread.CurrentThread.ManagedThreadId}" );
#endif
                return dao;
            }
        }

       
        public void SetData(string name,object obj)
        {
            
            AdditionalData.AddOrUpdate(name, obj,(key,oldValue)=>obj);

        }
        public object GetData(string name)
        {
            if (AdditionalData.ContainsKey(name))
                return AdditionalData[name];
            return null;


        }

        #region SESSION_SCOPE
        public void AddScope()
        {

        }
        #endregion


        public void Clear()
        {
            //daoFactories = null;
            Daos?.Clear();
            AdditionalData?.Clear();
           
            
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(EnviromentContext?.GetHashCode());
        
            return hashCode.ToHashCode();
        }

        public void Dispose()
        {
            if(Daos!=null)
            {
                foreach(var dao in Daos.Values)
                {
                    dao.Dispose();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Daos != null)
            {
                foreach (var dao in Daos.Values)
                {
                    await dao.DisposeAsync();
                }
            }
            Console.WriteLine("dispose sessioncontext:" + this.GetHashCode());
        }

      
    }//end SessionContext

}//end namespace Persistent