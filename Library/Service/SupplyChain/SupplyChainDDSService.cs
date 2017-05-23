using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;

using RepositoryPattern.Repositories;
using Service.Interface;
using Service.SupplyChain;
using Utils.Caching;

namespace Service.Implement
{
    public class SupplyChainDDSService : ISupplyChainDDSService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainDDS_BY_ID_KEY = "PG.SupplyChainDDS.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainDDS_ALL_KEY = "PG.SupplyChainDDS.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainDDS_BY_DATE_KEY = "PG.SupplyChainDDS.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainDDS_PATTERN_KEY = "PG.SupplyChainDDS.";

        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainDDS> _supplyChainDDSRepositoryAsync;

        public SupplyChainDDSService(IRepositoryAsync<SupplyChainDDS> repository, 
            IRepositoryAsync<SupplyChainDDS> SupplyChainDDSRepositoryAsync, 
            ICacheManager cacheManager)

        {
            _supplyChainDDSRepositoryAsync = SupplyChainDDSRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<SupplyChainDDS> GetSupplyChainDDSById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainDDS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _supplyChainDDSRepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainDDS>> GetAllSupplyChainDDSs()
        {
            var key = string.Format(SupplyChainDDS_ALL_KEY);
            return _cacheManager.Get(key, () => _supplyChainDDSRepositoryAsync.Table.ToListAsync());
        }
        public Task<List<SupplyChainDDS>> GetSupplyChainDDSByDate(DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainDDS_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _supplyChainDDSRepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate ).ToListAsync());
        }

        public SupplyChainDDS GetSupplyChainDDSMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var listResultInDay = _supplyChainDDSRepositoryAsync.Table.Where(p => p.CreatedDate < endDay && p.CreatedDate >= startDay);
            var result = listResultInDay.FirstOrDefault(p => measureCode.Contains(p.MeasureCode.ToString()) && p.type == type);
            return result;
        }


        public Task CreateAsync(SupplyChainDDS SupplyChainDDS)
        {
            _cacheManager.RemoveByPattern(SupplyChainDDS_PATTERN_KEY);

            return _supplyChainDDSRepositoryAsync.InsertAsync(SupplyChainDDS);
        }

        public Task UpdateAsync(SupplyChainDDS SupplyChainDDS)
        {
            _cacheManager.RemoveByPattern(SupplyChainDDS_PATTERN_KEY);
            return _supplyChainDDSRepositoryAsync.UpdateAsync(SupplyChainDDS);
        }

        public Task DeleteAsync(SupplyChainDDS SupplyChainDDS)
        {
            _cacheManager.RemoveByPattern(SupplyChainDDS_PATTERN_KEY);

            return _supplyChainDDSRepositoryAsync.DeleteAsync(SupplyChainDDS);
        }
    }
}
