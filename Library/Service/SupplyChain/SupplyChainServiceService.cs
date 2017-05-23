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
    public class SupplyChainServiceService : ISupplyChainServiceService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainService_BY_ID_KEY = "PG.SupplyChainService.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainService_ALL_KEY = "PG.SupplyChainService.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainService_BY_DATE_KEY = "PG.SupplyChainService.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainService_PATTERN_KEY = "PG.SupplyChainService.";

        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainService> _supplyChainServiceRepositoryAsync;

        public SupplyChainServiceService(IRepositoryAsync<SupplyChainService> supplyChainServiceRepositoryAsync, 
            ICacheManager cacheManager)
        {
            _supplyChainServiceRepositoryAsync = supplyChainServiceRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<SupplyChainService> GetSupplyChainServiceById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainService_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _supplyChainServiceRepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainService>> GetAllSupplyChainServices()
        {
            var key = string.Format(SupplyChainService_ALL_KEY);
            return _cacheManager.Get(key, () => _supplyChainServiceRepositoryAsync.Table.ToListAsync());
        }

        public List<SupplyChainService> GetAllSupplyChainServicesNotAsync()
        {
            throw new NotImplementedException();
        }

        

        public Task<List<SupplyChainService>> GetSupplyChainServiceByDate( DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainService_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _supplyChainServiceRepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate ).ToListAsync());
        }

        public SupplyChainService GetSupplyChainServiceMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var listResultInDay = _supplyChainServiceRepositoryAsync.Table.Where(p => p.CreatedDate < endDay && p.CreatedDate >= startDay);
            var result = listResultInDay.FirstOrDefault(p => measureCode.Contains(p.MeasureCode.ToString()) && p.type == type);
            return result;
        }


        public Task CreateAsync(SupplyChainService supplyChainService)
        {
            _cacheManager.RemoveByPattern(SupplyChainService_PATTERN_KEY);

            return _supplyChainServiceRepositoryAsync.InsertAsync(supplyChainService);
        }

        public Task UpdateAsync(SupplyChainService supplyChainService)
        {
            _cacheManager.RemoveByPattern(SupplyChainService_PATTERN_KEY);
            return _supplyChainServiceRepositoryAsync.UpdateAsync(supplyChainService);
        }

        public Task DeleteAsync(SupplyChainService supplyChainService)
        {
            _cacheManager.RemoveByPattern(SupplyChainService_PATTERN_KEY);

            return _supplyChainServiceRepositoryAsync.DeleteAsync(supplyChainService);
        }
    }
}
