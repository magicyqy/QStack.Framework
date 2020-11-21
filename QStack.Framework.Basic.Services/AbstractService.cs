using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using QStack.Framework.Basic.IServices;
using ServiceFramework.Common;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using QStack.Framework.Core.Model;

namespace QStack.Framework.Basic.Services
{
    [SessionInterceptor]
    [ServiceObject]
    public abstract class AbstractService<TEntity> : IBaseService where TEntity : class
    {
        public virtual IDaoCollection Daos { get; set; }
        public virtual IMapper Mapper { get; set; }


        [TransactionInterceptor]
        public virtual async Task<T> AddAsync<T>(T dto)
        {
             TEntity entity = Mapper.Map<TEntity>(dto);
             entity= await Daos.CurrentDao.AddAsync(entity);
               
             return Mapper.Map<T>(entity);
           
        }

        public virtual async Task<T> Get<T>(Expression<Func<T, bool>> filterExpression)
        {
            var filterExp = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(filterExpression);            
            var user=await Daos.CurrentDao.SingleAsync(filterExp);
            return Mapper.Map<T>(user);
        }

        [TransactionInterceptor]
        public virtual async Task<bool> Delete(object id) 
        {
            
            return await Daos.CurrentDao.DeleteById<TEntity>(id);
        }
      

        [TransactionInterceptor]
        public virtual async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var expression = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(predicate);
            return await Daos.CurrentDao.DeleteAsync(expression);
        }
        [TransactionInterceptor]
        public virtual async Task<bool> DeleteByIdAsync(object id)
        {
            return await Daos.CurrentDao.DeleteById<TEntity>(id);
        }


        public virtual async Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds=null) where T : class
        {
            var expression = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);
            var queryList = await Daos.CurrentDao.Query<TEntity>(expression, strOrderByFileds);
            IList<T> resultList = Mapper.Map<IList<T>>(queryList);
            return resultList;
        }

        public virtual async Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class
        {
            var expWhere = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);
            var expOrderBy= Mapper.MapExpression<Expression<Func<TEntity, object>>>(orderByExpression);
            var queryList = await Daos.CurrentDao.Query<TEntity>(expWhere, expOrderBy,isAsc);
            IList<T> resultList = Mapper.Map<IList<T>>(queryList);
            return resultList;
        }

        public virtual async  Task<T> QueryById<T>(object objId) where T:class
        {
            var entity = await Daos.CurrentDao.QueryById<TEntity>(objId);
            return Mapper.Map<T>(entity);
        }

      

        public virtual async Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params string[] paths) where T : class
        {
            var expWhere = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);
            var queryList=await Daos.CurrentDao.QueryInclude(expWhere, paths);
            return Mapper.Map<IList<T>>(queryList);
        }


        //public virtual async Task<IList<T>> QueryInclue<T, TProperty>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, TProperty>> include, Expression<Func<TProperty,object>> thenInclude) where T : class
        //{
        //    var expWhere = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);

        //    var includeExp = Mapper.Map<Expression<Func<TEntity, TProperty>>>(include);
        //    var thenIncludeExp = Mapper.Map<Expression<Func<TProperty, object>>>(thenInclude);

        //    var queryList = await Daos.CurrentDao.QueryInclude<TEntity, TProperty>(expWhere, includeExp, thenIncludeExp);
        //    return Mapper.Map<IList<T>>(queryList);
        //}

        public virtual async Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] paths) where T : class
        {
            var expWhere = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);
            var expressionArray = new List<Expression<Func<TEntity, object>>>();
            if (paths!=null)
            {
                
                foreach (var path in paths)
                {
                    expressionArray.Add(Mapper.Map<Expression<Func<TEntity, object>>>(path));
                }
            }
            var queryList =await Daos.CurrentDao.QueryInclude<TEntity>(expWhere, expressionArray.ToArray());
            return Mapper.Map<IList<T>>(queryList);
        }

        public virtual async Task<PageModel<T>> QueryPage<T>(Expression<Func<T, bool>> whereExpression, int pageIndex = 1, int pageSize = 20, string strOrderByFileds = null,IEnumerable<string> includePath=null) where T : class
        {
            var expWhere = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereExpression);
            var queryList = await Daos.CurrentDao.QueryPage<TEntity>(pageIndex, pageSize,expWhere, includePath,  strOrderByFileds);
           
            return Mapper.Map<PageModel<T>>(queryList);
        }
        [TransactionInterceptor]
        public virtual async Task<T> AddOrUpdate<T,TKey>(T model) where T : class
        {
            var entity= Mapper.Map<TEntity>(model);
            
            
            var Value=await Daos.CurrentDao.AddOrUpdate<TEntity, TKey>(entity);
            return Mapper.Map<T>(Value);
         }
        [TransactionInterceptor]
        public virtual async Task<int> Update<T>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression) where T : class
        {
            var filterExp = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(filterExpression);
            var updateExp = Mapper.MapExpression<Expression<Func<TEntity, TEntity>>>(updateExpression);
            var str = updateExp.ToExpressionString();
            return await Daos.CurrentDao.Update(filterExp, updateExp);
        }

        public virtual async Task<List<T>> GetAll<T>()
        {
            var list = await Daos.CurrentDao.Query<TEntity>();

            return Mapper.Map<List<T>>(list);

        }

        public virtual async Task<int> CountAsync<T>(Expression<Func<T, bool>> filterExpression)
        {
            var filterExp = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(filterExpression);
            return await Daos.CurrentDao.CountAsync<TEntity>(filterExp);
        }
        public virtual async Task<int> CountAsync<T>()
        {
             return await Daos.CurrentDao.CountAsync<TEntity>();
         
        }

        public virtual async Task<bool> AnyAsync()
        {
            return await Daos.CurrentDao.AnyAsync<TEntity>();
        }
    }
}
