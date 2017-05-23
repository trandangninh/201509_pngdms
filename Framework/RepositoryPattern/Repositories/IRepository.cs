#region
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
#endregion

namespace RepositoryPattern.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(T entity);
        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<T> entities);
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(T entity);
        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);
        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<T> entities);
        /// <summary>
        /// Get a table
        /// </summary>
        IDbSet<T> Table { get; }

        /*
        T Find(params object[] keyValues);
        IQueryable<T> SelectQuery(string query, params object[] parameters);
        
        void InsertRange(IEnumerable<T> entities);
        void InsertGraph(T entity);
        void InsertGraphRange(IEnumerable<T> entities);
        void Delete(object id);
        IQueryFluent<T> Query(IQueryObject<T> queryObject);
        IQueryFluent<T> Query(Expression<Func<T, bool>> query);
        IQueryFluent<T> Query();
        IQueryable Queryable(ODataQueryOptions<T> oDataQueryOptions);
        IQueryable<T> Queryable(); */
        //IRepository<T> GetRepository<T>() where T : BaseEntity;
       
    }
}