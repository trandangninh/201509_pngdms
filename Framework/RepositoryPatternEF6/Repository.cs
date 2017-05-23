#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using RepositoryPattern;
using RepositoryPattern.DataContext;
using RepositoryPattern.Repositories;

#endregion

namespace RepositoryPatternEF6
{
    public class Repository<T> : IRepositoryAsync<T> where T : BaseEntity
    {
        private readonly IDbContextAsync _contextAsync;
        private IDbSet<T> _entities;
        
        public Repository(IDbContextAsync contextAsync)
        {
            this._contextAsync = contextAsync;
        }

        #region old code
        //public virtual TEntity Find(params object[] keyValues)
        //{
        //    return _dbSet.Find(keyValues);
        //}

        //public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        //{
        //    return _dbSet.SqlQuery(query, parameters).AsQueryable();
        //}

        //public virtual void Insert(TEntity entity)
        //{
        //    //((IObjectState) entity).ObjectState = ObjectState.Added;
        //    _dbSet.Attach(entity);
        //    _contextAsync.SyncObjectState(entity);
        //}

        //public virtual void InsertRange(IEnumerable<TEntity> entities)
        //{
        //    foreach (var entity in entities)
        //        Insert(entity);
        //}

        //public virtual void InsertGraph(TEntity entity)
        //{
        //    _dbSet.Add(entity);
        //}

        //public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        //{
        //    _dbSet.AddRange(entities);
        //}

        //public virtual void Update(TEntity entity)
        //{
        //    //((IObjectState) entity).ObjectState = ObjectState.Modified;
        //    _dbSet.Attach(entity);
        //    _contextAsync.SyncObjectState(entity);
        //}

        //public virtual void Delete(object id)
        //{
        //    var entity = _dbSet.Find(id);
        //    Delete(entity);
        //}

        //public virtual void Delete(TEntity entity)
        //{
        //    //((IObjectState) entity).ObjectState = ObjectState.Deleted;
        //    _dbSet.Attach(entity);
        //    _contextAsync.SyncObjectState(entity);
        //}

        //public IQueryFluent<TEntity> Query()
        //{
        //    return new QueryFluent<TEntity>(this);
        //}

        //public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        //{
        //    return new QueryFluent<TEntity>(this, queryObject);
        //}

        //public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        //{
        //    return new QueryFluent<TEntity>(this, query);
        //}

        //public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        //{
        //    return await _dbSet.FindAsync(keyValues);
        //}

        //public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        //{
        //    return await _dbSet.FindAsync(cancellationToken, keyValues);
        //}

        //public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        //{
        //    return await DeleteAsync(CancellationToken.None, keyValues);
        //}

        //public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        //{
        //    var entity = await FindAsync(cancellationToken, keyValues);
        //    if (entity == null) return false;

        //    _dbSet.Attach(entity);
        //    _dbSet.Remove(entity);
        //    return true;
        //}

        //public IQueryable<TEntity> QueryableAsync()
        //{
        //    return Set<TEntity>();
        //}

        //public Task InsertAsync(TEntity entity)
        //{
        //    //entity.ObjectState = ObjectState.Added;
        //    this._contextAsync.Set<TEntity>().Add(entity);
        //    return _contextAsync.SaveChangesAsync();
        //}

        //public Task UpdateAsync(TEntity entity)
        //{
        //    return _contextAsync.SaveChangesAsync();
        //}

        //public Task DeleteAsync(TEntity entity)
        //{
        //    this._contextAsync.Set<TEntity>().Remove(entity);
        //    return _contextAsync.SaveChangesAsync();
        //}

        //internal IQueryable<TEntity> Select(
        //    Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    List<Expression<Func<TEntity, object>>> includes = null,
        //    int? page = null,
        //    int? pageSize = null)
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    if (includes != null)
        //        foreach (var include in includes)
        //            query = query.Include(include);

        //    if (orderBy != null)
        //        query = orderBy(query);

        //    if (filter != null)
        //        query = query.AsExpandable().Where(filter);

        //    if (page != null && pageSize != null)
        //        query = query.Skip((page.Value - 1)*pageSize.Value).Take(pageSize.Value);

        //    return query;
        //}

        //internal async Task<IEnumerable<TEntity>> SelectAsync(
        //    Expression<Func<TEntity, bool>> query = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    List<Expression<Func<TEntity, object>>> includes = null,
        //    int? page = null,
        //    int? pageSize = null)
        //{
        //    return Select(query, orderBy, includes, page, pageSize).AsEnumerable();
        //}

        //public IQueryable Queryable(ODataQueryOptions<TEntity> oDataQueryOptions)
        //{
        //    return oDataQueryOptions.ApplyTo(_dbSet);
        //}

        //public IQueryable<TEntity> Queryable()
        //{
        //    return Set<TEntity>();
        //}

        //public new IDbSet<TEntity> Set<TEntity>() where TEntity : Entity
        //{
        //    return (_contextAsync as DbContext).Set<TEntity>();
        //}

        //public IRepository<T> GetRepository<T>() where T : IObjectState
        //{
        //    return _unitOfWork.Repository<T>();
        //}


        ///// <summary>
        ///// Gets a table
        ///// </summary>
        ////public virtual IQueryable<TEntity> Table
        ////{
        ////    get
        ////    {
        ////        return this.Entities;
        ////    }
        ////}
        /////// <summary>
        /////// Entities
        /////// </summary>
        ////protected virtual IDbSet<TEntity> Entities
        ////{
        ////    get
        ////    {
        ////        if (_entities == null)
        ////            _entities = _context.Set<TEntity>();
        ////        return _entities;
        ////    }
        ////}
        #endregion

        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _contextAsync.Set<T>();
                return _entities;
            }
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);

                this._contextAsync.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach(var entity in entities)
                    this.Entities.Add(entity);

                this._contextAsync.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._contextAsync.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);

                this._contextAsync.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this.Entities.Remove(entity);

                this._contextAsync.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public IDbSet<T> Table {
            get
            {
                return this.Entities;
            }
        }

        public Task<T> GetByIdAsync(object id)
        {
            return (this.Entities as DbSet<T>).FindAsync(id);
        }

        public Task InsertAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);

                return this._contextAsync.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public Task InsertAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");
                foreach (var entity in entities)
                    this.Entities.Add(entity);

                return this._contextAsync.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public Task UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                return this._contextAsync.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public Task DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);

                return this._contextAsync.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }

        public Task DeleteAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");
                foreach(var entity in entities)
                    this.Entities.Remove(entity);

                return this._contextAsync.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);

                throw fail;
            }
        }
    }
}