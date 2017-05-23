using Entities.Domain.ScMeasures;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;

namespace Service.ScMeasures
{
    public class ScMeasureService : BaseService<ScMeasure>, IScMeasureService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string SCMEASURE_LISTPAGED_KEY = "PG.ScMeasure.ListPaged-{0}-{1}-{2}-{3}";

        private const string SCMEASURE_BY_CODE = "PG.ScMeasure.ByCode-{0}";

        private const string SCMEASURE_BY_NAME = "PG.ScMeasure.ByName-{0}";

        protected override string PatternKey
        {
            get { return "PG.ScMeasure."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<ScMeasure> _scMeasureRepositoryAsync;

        public ScMeasureService(ICacheManager cacheManager,
            IRepositoryAsync<ScMeasure> scMeasureRepositoryAsync)
            : base(scMeasureRepositoryAsync,cacheManager)
        {
            _scMeasureRepositoryAsync = scMeasureRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all ScMeasure
        /// </summary>
        /// <returns>paged list ScMeasure</returns>
        public Task<IPagedList<ScMeasure>> GetAllScMeasureAsync(string searchBySCMeasureName, string searchBySCMeasureCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(SCMEASURE_LISTPAGED_KEY, searchBySCMeasureName, searchBySCMeasureCode, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _scMeasureRepositoryAsync.Table.AsQueryable();

                if (!string.IsNullOrEmpty(searchBySCMeasureName))
                    query = query.Where(s => s.Name.Contains(searchBySCMeasureName));

                if (!string.IsNullOrEmpty(searchBySCMeasureCode))
                    query = query.Where(s => s.Code.Contains(searchBySCMeasureCode));

                query = query.OrderBy(c => c.DisplayOrder);
                return Task.FromResult(new PagedList<ScMeasure>(query, pageIndex, pageSize) as IPagedList<ScMeasure>);
            }
            );
        }

        /// <summary>
        /// check name of scMeasure existed or not
        /// </summary>
        /// <param name="code"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckCodeHasExisted(string code)
        {
            return Task.FromResult(_scMeasureRepositoryAsync.Table.Any(c => c.Code == code));
        }

        /// <summary>
        /// get scMeasure by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<ScMeasure> GetScMeasureByCodeAsync(string code)
        {
            var key = string.Format(SCMEASURE_BY_CODE, code);
            return _cacheManager.Get(key, () => _scMeasureRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Code == code));
        }

        /// <summary>
        /// get scMeasure by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<ScMeasure> GetScMeasureByNameAsync(string name)
        {
            var key = string.Format(SCMEASURE_BY_NAME, name);
            return _cacheManager.Get(key, () => _scMeasureRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Name == name));
        }
        /// <summary>
        /// delete list scMeasures
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public Task DeleteScMeasuresAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId is null");
            var deletedScMeasures = _scMeasureRepositoryAsync.Table.Where(i => listId.Contains(i.Id));
            _cacheManager.RemoveByPattern(PatternKey);
            return _scMeasureRepositoryAsync.DeleteAsync(deletedScMeasures);
        }
    }
}
