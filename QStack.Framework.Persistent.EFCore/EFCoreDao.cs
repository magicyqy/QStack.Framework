using System.Data;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Entity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using AspectCore.Extensions.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using QStack.Framework.Core;
//using QStack.Framework.Basic.Model.Auth;

namespace QStack.Framework.Persistent.EFCore
{
    public class EFCoreDao : DbContext, IDao
    {
        //public DbSet<User> Users { get; set; }
        //public DbSet<Function> Functions { get; set; }
        //public DbSet<FunctionOperation> FunctionOperations { get; set; }
        //public DbSet<Group> GroupS { get; set; }
        //public DbSet<GroupRole> GroupRoleS { get; set; }
        //public DbSet<Operation> OperationS { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<RoleFunction> RoleFunctions { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        public int CacheKey { get; private set; }
        private readonly IEnumerable<Assembly> _entityAssemblies;
        private SessionContext _sessionContext;
        private readonly DaoFactoryOption _daoFactoryOption;
        public EFCoreDao(DbContextOptions options, DaoFactoryOption daoFactoryOption,SessionContext sessionContext=null) : base(options)
        {
            _daoFactoryOption = daoFactoryOption;
            _entityAssemblies = daoFactoryOption.EntityAssemblies;
            _sessionContext = sessionContext;

            //this must at the last
            CacheKey = GetCacheKey();
            //Console.WriteLine(string.Join(" \t", _entityAssemblies.Select(e => e.FullName)));
            //Console.WriteLine("sesstion hashcode:" + HashCode);
        }

        private  int GetCacheKey()
        {
            HashCode hash = new HashCode();
            hash.Add(_daoFactoryOption.GetHashCode());
            hash.Add(Convert.ToInt32(_sessionContext?.GetHashCode()));
          
            return hash.ToHashCode();
        }

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .ReplaceService<IModelCacheKeyFactory, MasterModelCacheKeyFactory>();
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            if (_entityAssemblies != null)
            {
                foreach (var type in _entityAssemblies.SelectMany(a=>a.ExportedTypes))
                {
                    if (type.IsClass && type != typeof(EntityBase) && type != typeof(EntityRoot) && typeof(IEntityRoot).IsAssignableFrom(type))
                    {
                        //Console.WriteLine(type.FullName);
                        var method = modelBuilder.GetType().GetMethods().Where(x => x.Name ==nameof(modelBuilder.Entity)).FirstOrDefault();

                        if (method != null)
                        {
                            method = method.MakeGenericMethod(new Type[] { type });
                            method.Invoke(modelBuilder, null);
                        }
                    }
                    if (type.GetCustomAttribute<IgnoredByMigrationsAttribute>() != null)
                    {
                        var annotation = modelBuilder.Entity(type).Metadata.FindAnnotation(RelationalAnnotationNames.TableName);
                        if (annotation == null)
                        {
                            modelBuilder.Entity(type).ToView(type.Name);
                        }
                        else
                            modelBuilder.Entity(type).ToView(annotation.Value.ToString());
                    }
                       
                }
            }
            //efcore默认只能通过fluentApi方式制定联合主键，以下使efcore支持类似ef的联合主键特性声明方式创建联合主键,
            foreach (var entity in modelBuilder.Model.GetEntityTypes()
                .Where(t =>
                    t.ClrType.GetProperties()
                        .Count(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))) > 1))
            {
                // get the keys in the appropriate order
                var orderedKeys = entity.ClrType
                    .GetProperties()
                    .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
                    .OrderBy(p =>
                        p.CustomAttributes.Single(x => x.AttributeType == typeof(ColumnAttribute))?
                            .NamedArguments?.Single(y => y.MemberName == nameof(ColumnAttribute.Order))
                            .TypedValue.Value ?? 0)
                    .Select(x => x.Name)
                    .ToArray();

                // apply the keys to the model builder
                modelBuilder.Entity(entity.ClrType).HasKey(orderedKeys);
                
            }
            //modelBuilder.UseIdentityAlwaysColumns();

            for (var i=0;i< _sessionContext?.QueryFilters?.Count();i++)
            {
                var lambda = _sessionContext.QueryFilters.ElementAt(i);
                modelBuilder.Entity(lambda.Parameters.Single().Type).HasQueryFilter(lambda);
            }
            //modelBuilder.SetQueryFilterOnAllEntities<EntityBase>(p =>p.CreateUserId==_UserId,_sessionContext );

        }
        //Func<Type,int?, LambdaExpression> CreateLambda => (type) =>
        //  {
             
        //      LambdaExpression lambdaExpression = null;
        //      if (_sessionContext?.QueryFilters?.Any() == true)
        //          lambdaExpression = _sessionContext.QueryFilters.FirstOrDefault(l => l.Parameters.Single().Type == type);
        //      if(lambdaExpression==null)
        //      {
        //          var parameter = Expression.Parameter(type, "m");
        //          var memeber = Expression.Property(parameter, nameof(EntityBase.CreateUserId));
        //          var value = Expression.Constant(userId);
        //          var expression = Expression.Equal(memeber, value);
        //          lambdaExpression = Expression.Lambda(expression, parameter);
        //      }
        //      return lambdaExpression;
              
        //  };
        public IEnumerable<Type> GetEntityTypes()
        {
            
            return base.Model.GetEntityTypes().Select(t => t.ClrType);
        }
       
        public async Task<T> AddAsync<T>(T model) where T : class
        {
            //this.Set<T>().a
            this.Attach(model).State = EntityState.Added;
            //this.Entry<T>(model)
            //var entityEntry = await base.AddAsync(model);
            await SaveChangesAsync();
            return this.Entry<T>(model).Entity;
        }

        public async Task CloseAsync()
        {
            await this.DisposeAsync();
        }

        public async Task<bool> Delete<T>(T model) where T : class
        {
            this.Set<T>().Remove(model);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteById<T>(object id) where T : class
        {
            this.Set<T>().Remove(this.Set<T>().Find(id));
            return await Task.FromResult(true);
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <typeparam name="T">实体约束</typeparam>
        /// <param name="entities">实体集合</param>

        public async Task Delete<T>(IEnumerable<T> entities) where T : class
        {
            this.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }
        /// <summary>
        /// 批量删除<br/>
        /// 例子：批量删除数据<br/>
        /// <example>
        /// <code>DeleteAsync(user=>user.State==State.已冻结)</code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">实体约束</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            int count = await this.Set<T>().Where(predicate).DeleteFromQueryAsync();

            return count;
        }
     
        public async Task<IEnumerable<T>> SqlQuery<T>(string sql, params object[] parameters) where T : class, new()
        {
            return await this.Database.SqlQuery<T>(sql, parameters);

        }

        /// <summary>
        /// 调用dao中的方法后，必需显式调用flush,才会同步到数据库
        /// </summary>
        public async Task Flush()
        {
            await this.SaveChangesAsync();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateEnityBaseProperties();
            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            UpdateEnityBaseProperties();
            return base.SaveChanges();
        }
        /// <summary>
        /// 自动添加创建人，修改时间等值
        /// </summary>
        private void UpdateEnityBaseProperties()
        {
          
            if (_sessionContext == null || _sessionContext.EnviromentContext == null)
                return;
            ChangeTracker.DetectChanges();

            #region 

            var objectStateEntryList = ChangeTracker.Entries().Where(obj => obj.State != EntityState.Unchanged);
            foreach (var entry in objectStateEntryList)
            {
                if (!typeof(EntityBase).IsAssignableFrom(entry.Entity.GetType()))
                    continue;
                var entity = entry.Entity as EntityBase;
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreateUserId = entity.CreateUserId ?? _sessionContext?.EnviromentContext.LoginUserId;
                        entity.CreateDate = entity.CreateDate ?? DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entity.LastModifyUserId = entity.LastModifyUserId ?? _sessionContext?.EnviromentContext.LoginUserId;
                        entity.LastModifyDate = entity.LastModifyDate ?? DateTime.Now;
                        break;
                    case EntityState.Deleted:
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }

        public IQueryable<T> DbSet<T>() where T : class
        {
            return (IQueryable<T>)this.Set<T>();
        }

        public async Task<T> Get<T>(object id) where T : class
        {
            return (T)await base.FindAsync(typeof(T), id);
        }

        public async Task<int> CountAsync<T>() where T:class
        {
            return await this.Set<T>().CountAsync();
        }
        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await this.Set<T>().CountAsync(predicate);
        }
        public async Task<bool> AnyAsync<T>() where T : class
        {
            return await this.Set<T>().AnyAsync();
        }

        public async Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] paths) where T : class
        {

            var queryable = this.Set<T>().AsQueryable();
            if(whereExpression!=null)
                queryable=queryable.Where(whereExpression);
            if (paths != null)
            {
                
                  queryable = queryable.Include(paths);

            }

            var list = await queryable.ToListAsync();
            return list;
        }
       
        public async Task<IList<T>> QueryInclude<T>(Expression<Func<T, bool>> whereExpression, params string[] paths ) where T : class
        {
            var set = this.Set<T>().AsQueryable();
            if (paths != null)
                set = set.Include(paths);
            var queryable = set.AsNoTracking().Where(whereExpression);
            return await (paths == null ? queryable : queryable.Include(paths)).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">如filed1 asc,field2 desc</param>
        /// <returns></returns>
        public async Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression=null, string strOrderByFileds = null) where T : class
        {
            var set = this.Set<T>().AsNoTracking();
            if(whereExpression!=null)
                set=set.Where(whereExpression);
            if (!string.IsNullOrWhiteSpace(strOrderByFileds))
                   set.OrderBy(strOrderByFileds);
         

            return await set.ToListAsync();
        }

        public async Task<IList<T>> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class
        {
            var set = this.Set<T>().AsNoTracking().Where(whereExpression);
            return await (isAsc ? set.OrderBy(orderByExpression) : set.OrderByDescending(orderByExpression))
                         .ToListAsync();
        }

      
        public async Task<T> SingleAsync<T>(Expression<Func<T,bool>> whereExpression) where T : class
        {
            var entity =await this.Set<T>().FirstOrDefaultAsync(whereExpression);

            return entity;
        }


        public async Task<T> QueryById<T>(object objId) where T : class
        {
            return await this.Set<T>().FindAsync(new object[] { objId });
        }




        public async Task<PageModel<T>> QueryPage<T>(int pageIndex = 1, int pageSize = 20,Expression<Func<T, bool>> whereExpression=null, Expression<Func<T, object>>[] paths=null, string strOrderByFileds = null) where T : class
        {

            var set = this.Set<T>().AsNoTracking();
            if(whereExpression!=null)
                set=set.Where(whereExpression);
            
            set = set.Include(paths);
            if (!string.IsNullOrWhiteSpace(strOrderByFileds))
                set = set.OrderBy(strOrderByFileds);
            return await QueryPage(set, pageIndex, pageSize);
        }
        public async Task<PageModel<T>> QueryPage<T>(int pageIndex = 1, int pageSize = 20, Expression<Func<T, bool>> whereExpression = null, IEnumerable<string> paths = null, string strOrderByFileds = null) where T : class
        {

            var set = this.Set<T>().AsNoTracking();
            if (whereExpression != null)
                set = set.Where(whereExpression);
            if(paths!=null)
                set = set.Include(paths);
            if (!string.IsNullOrWhiteSpace(strOrderByFileds))
                set = set.OrderBy(strOrderByFileds);
            return await QueryPage(set, pageIndex, pageSize);
        }
        public async Task<PageModel<T>> QueryPage<T>(IQueryable<T> set, int page, int size) where T : class
        {
            var pageResult = set.PageResult(page, size);

            var pageModel = new PageModel<T>() {
                Data = await pageResult.Queryable.ToListAsync(),
                TotalCount = pageResult.RowCount,
                Page = pageResult.CurrentPage,
                PageSize = pageResult.PageSize
               
            };
            return pageModel;
        }
        public async Task<T> AddOrUpdate<T,TKey>(T entity) where T : class
        {
            var entityEntry = this.Entry(entity);
            var exp = GetByIdExpression<T, TKey>(entityEntry);

            var navigationPropertyNames = this.GetIncludePaths(typeof(T),false);
            var dbVal = this.Set<T>().Include(navigationPropertyNames).FirstOrDefault(exp);
            
            if (dbVal != null)
            {
                //只更新主从表的导航属性，即子表主键，外键都是同一字段，且指向父表主键
                //Entry(dbVal).Navigations.Select(n=>n.)
                foreach (var meta in Entry(dbVal).Navigations.Select(n => n.Metadata))
                {
                    if (meta.IsCollection()) continue;
                    if (meta.ForeignKey.PrincipalEntityType.ClrType != dbVal.GetType())
                        continue;
                    var foreingnKeyName = meta.ForeignKey.Properties.Select(p => p.Name).First();
                    var navigationPrimaryKeyInfo = GetPrimaryKeyValue(entityEntry.Reference(meta.Name).TargetEntry);
                    if (foreingnKeyName != navigationPrimaryKeyInfo.Item1)
                        continue;
                    var newValue = navigationPrimaryKeyInfo.Item2;
                    
                    var oldValue = GetPrimaryKeyValue(Entry(dbVal).Reference(meta.Name).TargetEntry).Item2;
                    if(newValue?.ToString()!=oldValue?.ToString())
                         Entry(dbVal).Reference(meta.Name).CurrentValue= entityEntry.Reference(meta.Name).CurrentValue;
                    
                }
                
                Entry(dbVal).CurrentValues.SetValues(entity);
                entityEntry.State = EntityState.Detached;
                Set<T>().Update(dbVal);

                entity = dbVal;
            }
            else
            {
                this.Attach(entity).State = EntityState.Added;
                
            }
           await SaveChangesAsync();
            return entity;
        }

        public new void Update<T>(T entity) where T : class
        {
            base.Update(entity);
        }

        public async Task<int> Update<T>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression) where T : class
        {
            
            int count = this.Set<T>().Where(filterExpression).UpdateFromQuery(updateExpression);

            return await Task.FromResult(count);
        }



        public void BeginTransaction()
        {
            if (this.Database.CurrentTransaction == null)
            {
                this.Database.BeginTransaction();
            }
        }

        public async void Commit()
        {
            var transaction = this.Database.CurrentTransaction;
            if (transaction != null)
            {
                try
                {
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async void Rollback()
        {
            if (this.Database.CurrentTransaction != null)
            {
                await this.Database.CurrentTransaction.RollbackAsync();
            }
            
        }


        private  Expression<Func<T, bool>> GetByIdExpression<T,TKey>(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entityEntry) where T : class
        {

            var primaryInfo = GetPrimaryKeyValue(entityEntry);

            //if (keys.Length != 1)
            //    throw new Exception("GetByIdExpression works only with Entity which has one primary key column.");

            var param = Expression.Parameter(typeof(T), "p");
            var exp = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(param, primaryInfo.Item1),
                    ExpressionClosureFactory.GetField((TKey)primaryInfo.Item2)
                ),
                param
            );
            return exp;
        }

        private Tuple<string,object> GetPrimaryKeyValue(EntityEntry entityEntry)
        {
            if (entityEntry==null)
                return Tuple.Create<string,object>(null, null);
            var t = entityEntry.Entity.GetType();
            var primaryKeyName = entityEntry.Context.Model.FindEntityType(t).FindPrimaryKey().Properties
                .Select(x => x.Name).First();

            var primaryKeyField = t.GetProperty(primaryKeyName);


            if (primaryKeyField == null)
            {
                throw new Exception($"{t.FullName} does not have a primary key specified. Unable to exec AddOrUpdate call.");
            }
            var keyVal = primaryKeyField.GetReflector().GetValue(entityEntry.Entity);

            return Tuple.Create(primaryKeyName, keyVal);
        }

        public async Task AddRangeAsync(IEnumerable<object> entities)
        {
            await base.AddRangeAsync(entities);
        }
    }

    internal class ExpressionClosureFactory
    {
        public static MemberExpression GetField<TValue>(TValue value)
        {
            var closure = new ExpressionClosureField<TValue>
            {
                ValueProperty = value
            };

            return Expression.Field(Expression.Constant(closure), "ValueProperty");
        }

        class ExpressionClosureField<T>
        {
            public T ValueProperty;
        }
    }


}