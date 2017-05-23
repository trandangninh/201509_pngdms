using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Service.Interface;
using Utils.Caching;

namespace Service.SupplyChain
{
    public class SupplyChainProductionPlanningService : ISupplyChainProductionPlanningService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainProductionPlanning_BY_ID_KEY = "PG.SupplyChainProductionPlanning.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainProductionPlanning_ALL_KEY = "PG.SupplyChainProductionPlanning.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainProductionPlanning_BY_DATE_KEY = "PG.SupplyChainProductionPlanning.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainProductionPlanning_PATTERN_KEY = "PG.SupplyChainProductionPlanning.";

        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainProductionPlanning> _supplyChainProductionPlanningRepositoryAsync;

        public SupplyChainProductionPlanningService(IRepositoryAsync<SupplyChainProductionPlanning> supplyChainProductionPlanningRepositoryAsync, 
            ICacheManager cacheManager)
        {
            _supplyChainProductionPlanningRepositoryAsync = supplyChainProductionPlanningRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<SupplyChainProductionPlanning> GetSupplyChainProductionPlanningById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainProductionPlanning_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _supplyChainProductionPlanningRepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainProductionPlanning>> GetAllSupplyChainProductionPlannings()
        {
            var key = string.Format(SupplyChainProductionPlanning_ALL_KEY);
            return _cacheManager.Get(key, () => _supplyChainProductionPlanningRepositoryAsync.Table.ToListAsync());
        }

        public Task<List<SupplyChainProductionPlanning>> GetSupplyChainProductionPlanningByDate( DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainProductionPlanning_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _supplyChainProductionPlanningRepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate ).ToListAsync());
        }

        public SupplyChainProductionPlanning GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var listResultInDay = _supplyChainProductionPlanningRepositoryAsync.Table.Where(p => p.CreatedDate < endDay && p.CreatedDate >= startDay);
            var result = listResultInDay.FirstOrDefault(p => measureCode.Contains(p.MeasureCode.ToString()) && p.type == type);
            return result;
        }

        public Task CreateAsync(SupplyChainProductionPlanning supplyChainProductionPlanning)
        {
            _cacheManager.RemoveByPattern(SupplyChainProductionPlanning_PATTERN_KEY);

            return _supplyChainProductionPlanningRepositoryAsync.InsertAsync(supplyChainProductionPlanning);
        }

        public Task UpdateAsync(SupplyChainProductionPlanning supplyChainProductionPlanning)
        {
            _cacheManager.RemoveByPattern(SupplyChainProductionPlanning_PATTERN_KEY);
            return _supplyChainProductionPlanningRepositoryAsync.UpdateAsync(supplyChainProductionPlanning);
        }

        public Task DeleteAsync(SupplyChainProductionPlanning supplyChainProductionPlanning)
        {
            _cacheManager.RemoveByPattern(SupplyChainProductionPlanning_PATTERN_KEY);

            return _supplyChainProductionPlanningRepositoryAsync.DeleteAsync(supplyChainProductionPlanning);
        }
    }
}
