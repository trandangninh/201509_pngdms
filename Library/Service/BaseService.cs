using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;
using RepositoryPattern.Repositories;
using Utils.Caching;
using Utils;

namespace Service
{
    public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
    {
        private readonly IRepositoryAsync<T> _tRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        public BaseService(IRepositoryAsync<T> tRepositoryAsync,
            ICacheManager cacheManager)
        {
            this._tRepositoryAsync = tRepositoryAsync;
            this._cacheManager = cacheManager;
        }

        protected abstract string PatternKey { get; }

        public virtual Task InsertAsync(T enity)
        {
            if (enity == null)
                throw new ArgumentNullException("enity");
            _cacheManager.RemoveByPattern(PatternKey);

            return _tRepositoryAsync.InsertAsync(enity);
        }
        public virtual Task<T> GetByIdAsync(int id)
        {
            var key = string.Format(PatternKey + "byid-{0}", id);

            return _cacheManager.Get(key, () => _tRepositoryAsync.GetByIdAsync(id));
        }

        public virtual Task UpdateAsync(T enity)
        {
            if (enity == null)
                throw new ArgumentNullException("enity");
            _cacheManager.RemoveByPattern(PatternKey);

            return _tRepositoryAsync.UpdateAsync(enity);
        }

        public virtual Task DeleteAsync(T enity)
        {
            if (enity == null)
                throw new ArgumentNullException("enity");
            _cacheManager.RemoveByPattern(PatternKey);

            return _tRepositoryAsync.DeleteAsync(enity);
        }

        public virtual Task<IPagedList<T>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PatternKey + "all");

            return _cacheManager.Get(key, () =>
            {
                var query = _tRepositoryAsync.Table.OrderBy(x => x.Id);
                return Task.FromResult(new PagedList<T>(query, pageIndex, pageSize) as IPagedList<T>);
            }
                );
        }
    }
}
