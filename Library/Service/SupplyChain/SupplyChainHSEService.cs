using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.SupplyChain
{
    public class SupplyChainHSEService : ISupplyChainHSEService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainHSE_BY_ID_KEY = "PG.SupplyChainHSE.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainHSE_ALL_KEY = "PG.SupplyChainHSE.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainHSE_BY_DATE_KEY = "PG.SupplyChainHSE.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainHSE_PATTERN_KEY = "PG.SupplyChainHSE.";

        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainHSE> _supplyChainHSERepositoryAsync;

        public SupplyChainHSEService(IRepositoryAsync<SupplyChainHSE> supplyChainHSERepositoryAsync, 
            ICacheManager cacheManager)

        {
            _supplyChainHSERepositoryAsync = supplyChainHSERepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<SupplyChainHSE> GetSupplyChainHSEById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainHSE_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _supplyChainHSERepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainHSE>> GetAllSupplyChainHSEs()
        {
            var key = string.Format(SupplyChainHSE_ALL_KEY);
            return _cacheManager.Get(key, () => _supplyChainHSERepositoryAsync.Table.ToListAsync());
        }
        public Task<List<SupplyChainHSE>> GetSupplyChainHSEByDate( DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainHSE_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _supplyChainHSERepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate ).ToListAsync());
        }

        public SupplyChainHSE GetSupplyChainHSEMeasureCodeAndDate(string measureCode, DateTime createdDate)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var listResultInDay = _supplyChainHSERepositoryAsync.Table.Where(p => p.CreatedDate < endDay && p.CreatedDate >= startDay);
            var result = listResultInDay.FirstOrDefault(p => measureCode.Contains(p.MeasureCode.ToString()));
            return result;
        }


        public Task CreateAsync(SupplyChainHSE supplyChainHSE)
        {
            _cacheManager.RemoveByPattern(SupplyChainHSE_PATTERN_KEY);

            return _supplyChainHSERepositoryAsync.InsertAsync(supplyChainHSE);
        }

        public Task UpdateAsync(SupplyChainHSE supplyChainHSE)
        {
            _cacheManager.RemoveByPattern(SupplyChainHSE_PATTERN_KEY);
            return _supplyChainHSERepositoryAsync.UpdateAsync(supplyChainHSE);
        }

        public Task DeleteAsync(SupplyChainHSE supplyChainHSE)
        {
            _cacheManager.RemoveByPattern(SupplyChainHSE_PATTERN_KEY);

            return _supplyChainHSERepositoryAsync.DeleteAsync(supplyChainHSE);
        }
    }
}
