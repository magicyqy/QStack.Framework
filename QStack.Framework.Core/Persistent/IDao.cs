///////////////////////////////////////////////////////////
//  IDao.cs

using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using QStack.Framework.Core.Entity;

namespace QStack.Framework.Core.Persistent
{

    public interface IDao:IDisposable, IAsyncDisposable
    {

        Task<T> Get<T>(object id) where T : class;
     
        Task<T> AddAsync<T>(T model) where T:class;

        Task<bool> DeleteById<T>(object id)  where T:class;

        Task<bool> Delete<T>(T model)  where T:class;

        Task Delete<T>(IEnumerable<T> entities)  where T:class;
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate)  where T:class;

        /// <summary>
        /// 表达式批量更新 TODO:此方法不支持复杂导航属性更新<br/>
        /// <example>
        /// For example:
        /// <code>
        /// Update(user=>user.Id,u=>new User{Name="abc"})
        /// </code>       
        /// </example>
        /// </summary>     
        /// <typeparam name="T"></typeparam>
        /// <param name="filterExpression">过滤条件表达式</param>
        /// <param name="updateExpression">更新表达式</param>
        /// <returns></returns>
        Task<int> Update<T>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression)  where T:class;
        void Update<T>(T entity) where T : class;
        Task<T> QueryById<T>(object objId) where T:class;




        IQueryable<T> DbSet<T>()  where T:class;

        /// <summary>
        /// 表达式查询
        /// </summary>
        /// <example>
        /// <code>
        /// Query(entity=>entity.property==somevalue,"filed1 asc,field2 desc")
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">如</param>
        /// <returns></returns>
        Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression=null, string strOrderByFileds=null) where T:class;
        Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true)  where T:class;

        Task<PageModel<T>> QueryPage<T>(int intPageIndex = 1, int intPageSize = 20, Expression<Func<T, bool>> whereExpression=null, Expression<Func<T, object>>[] paths = null, string strOrderByFileds = null)  where T:class;



        Task<IEnumerable<T>> SqlQuery<T>(string sql, params object[] parameters) where T: class ,new();

        /// <summary>
        /// NoTacking query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression,params Expression<Func<T, object>>[] paths)  where T:class;
        /// <summary>
        /// NoTacking query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params string[] paths)  where T:class;
       

        void BeginTransaction();
        void Commit();

        void Rollback();

        /// <summary>
        /// 手动刷新
        /// </summary>
        /// <returns></returns>
        Task Flush();
        /// <summary>
        /// 释放连接
        /// </summary>
        /// <returns></returns>
        Task CloseAsync();
      
        
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class;
        Task AddRangeAsync(IEnumerable<object> entities);
        Task<PageModel<T>> QueryPage<T>(IQueryable<T> set, int page, int size) where T : class;
        /// <summary>
        /// 添加或者更新实体<br/><br/>
        /// 注意：该方法更新时，只维护从属表的更新（即子表的主键与外键为同一字段，且指向父表主键），其他关联属性需自行处理
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <typeparam name="TKey">实体主键</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddOrUpdate<T,TKey>(T entity) where T : class;
        Task<PageModel<T>> QueryPage<T>(int pageIndex = 1, int pageSize = 20, Expression<Func<T, bool>> whereExpression = null, IEnumerable<string> paths = null, string strOrderByFileds = null) where T : class;
        IEnumerable<Type> GetEntityTypes();
       
    }//end IDao

}//end namespace Persistent