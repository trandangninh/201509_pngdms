using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.Departments
{
    public class MeasureSupplyChainService : IMeasureSupplyChainService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string MeasureSupplyChain_BY_ID_KEY = "PG.MeasureSupplyChain.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string MeasureSupplyChain_ALL_KEY = "PG.MeasureSupplyChain.all";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string MeasureSupplyChain_BY_CODE_KEY = "PG.MeasureSupplyChain.bycode-{0}";

        private const string MeasureSupplyChain_BY_DMSCODE_MASURECODE_KEY = "PG.MeasureSupplyChain.bycode-{0}-{1}";
       
        private const string MeasureSupplyChain_PATTERN_KEY = "PG.MeasureSupplyChain.";

        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<MeasureSupplyChain> _measureSupplyChainRepositoryAsync;
        public MeasureSupplyChainService(IRepositoryAsync<MeasureSupplyChain> measureSupplyChainRepositoryAsync, 
            ICacheManager cacheManager)
        {
            _measureSupplyChainRepositoryAsync = measureSupplyChainRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<MeasureSupplyChain> GetMeasureSupplyChainById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(MeasureSupplyChain_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _measureSupplyChainRepositoryAsync.GetByIdAsync(id));
        }

        public Task<MeasureSupplyChain> GetMeasureSupplyChainByCode(string code)
        {
            if (String.IsNullOrEmpty(code))
                return null;
            var key = string.Format(MeasureSupplyChain_BY_CODE_KEY, code);
            return _cacheManager.Get(key,
                () =>
                    _measureSupplyChainRepositoryAsync.Table
                        .FirstOrDefaultAsync(x => x.MeasureSupplyChainCode == code));
        }

        public Task<List<MeasureSupplyChain>> GetAllMeasureSupplyChains()
        {
            var key = string.Format(MeasureSupplyChain_ALL_KEY);
            return _cacheManager.Get(key, () => _measureSupplyChainRepositoryAsync.Table.ToListAsync());
        }
        public Task<List<MeasureSupplyChain>> GetMeasureSupplyChainByDmsCode(string dmsCode)
        {
            throw new NotImplementedException();
        }

        public Task<MeasureSupplyChain> GetMeasureSupplyChainByDmsCodeAndMeasureCode(string dmsCode, string code)
        {

            if (String.IsNullOrEmpty(code))
                return null;
            var key = string.Format(MeasureSupplyChain_BY_DMSCODE_MASURECODE_KEY,dmsCode, code);
            return _cacheManager.Get(key,
                () =>
                    _measureSupplyChainRepositoryAsync.Table
                        .FirstOrDefaultAsync(x => x.MeasureSupplyChainCode == code && x.DmsCode==dmsCode));
        }


        public Task CreateAsync(MeasureSupplyChain measureSupplyChain)
        {
            _cacheManager.RemoveByPattern(MeasureSupplyChain_PATTERN_KEY);

            return _measureSupplyChainRepositoryAsync.InsertAsync(measureSupplyChain);
        }

        public Task UpdateAsync(MeasureSupplyChain measureSupplyChain)
        {
            _cacheManager.RemoveByPattern(MeasureSupplyChain_PATTERN_KEY);
            return _measureSupplyChainRepositoryAsync.UpdateAsync(measureSupplyChain);
        }

        public Task DeleteAsync(MeasureSupplyChain measureSupplyChain)
        {
            _cacheManager.RemoveByPattern(MeasureSupplyChain_PATTERN_KEY);

            return _measureSupplyChainRepositoryAsync.DeleteAsync(measureSupplyChain);
        }
    }
}
