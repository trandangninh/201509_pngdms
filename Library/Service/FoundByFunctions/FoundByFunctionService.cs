using Entities.Domain.FoundByFunction;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;

namespace Service.FoundByFunctions
{
    public partial class FoundByFunctionService : BaseService<FoundByFunction>, IFoundByFunctionService
    {
        protected override string PatternKey
        {
            get { return "FoundByFunction."; }
        }

        #region constant for cache

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : FoundByFunction Name
        /// </remarks>
        private const string FoundByFunction_BY_NAME = "FoundByFunction.byname-{0}";
        private const string FoundByFunction_GET_LIST = "FoundByFunction.getlist";

        #endregion

        private readonly IRepositoryAsync<FoundByFunction> _foundByFunctionRepositoryAsync;
        private readonly ICacheManager _cacheManager;

        public FoundByFunctionService(IRepositoryAsync<FoundByFunction>  foundByFunctionRepositoryAsync, ICacheManager cacheManager)
            : base( foundByFunctionRepositoryAsync, cacheManager)
        {
            this._foundByFunctionRepositoryAsync =  foundByFunctionRepositoryAsync;
            this._cacheManager = cacheManager;
        }

        public Task<FoundByFunction> GetFoundByFunctionByName(string Name)
        {
            if (String.IsNullOrEmpty(Name))
                return null;
            var key = string.Format(FoundByFunction_BY_NAME, Name);
            return _cacheManager.Get(key, () =>
                _foundByFunctionRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Name == Name));
        }
        public Task<IPagedList<FoundByFunction>> GetListFoundByFunctions(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = FoundByFunction_GET_LIST;

            return _cacheManager.Get(key, () =>
            {
                var query = _foundByFunctionRepositoryAsync.Table.AsQueryable();
                return Task.FromResult(new PagedList<FoundByFunction>(query.OrderBy(l => l.Id), pageIndex, pageSize) as IPagedList<FoundByFunction>);
            });
        }
    }
}
