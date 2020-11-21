using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{

    public interface IBaseService
    {

        Task<T> AddAsync<T>(T model);

        Task<bool> DeleteByIdAsync(object id);


        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T> AddOrUpdate<T,TKey>(T model) where T : class;
        Task<int> Update<T>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression) where T : class;

        Task<T> QueryById<T>(object objId) where T : class;

      

       
    
        Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds) where T : class;
        Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class;

        Task<PageModel<T>> QueryPage<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null, IEnumerable<string> includePath = null) where T : class;

        //Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] paths) where T : class;
        Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params string[] paths) where T : class;
        Task<T> Get<T>(Expression<Func<T, bool>> filterExpression);
        Task<List<T>> GetAll<T>();
        Task<int> CountAsync<T>(Expression<Func<T, bool>> filterExpression);
        Task<int> CountAsync<T>();
        Task<bool> AnyAsync();
    }
}
