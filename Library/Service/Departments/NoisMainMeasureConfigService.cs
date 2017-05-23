using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

using RepositoryPattern.Repositories;
using Service.Interface;
using Utils.Caching;

namespace Service.Implement
{
    public class NoisMainMeasureConfigService : INoisMainMeasureConfigService
    {

        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : linecode
        /// {1}: measurecode
        /// {2}:date
        /// </remarks>
        private const string MEASURECONFIG_BY_LINECODE_MEASURECONFIGCODE__KEY = "PG.noismeasure.byid-{0}-{1}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MEASURECONFIG_PATTERN_KEY = "PG.noismeasure.";

      

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<NoisMainMeasureConfig> _mainMeasurRepositoryAsync;

        public NoisMainMeasureConfigService(IRepositoryAsync<NoisMainMeasureConfig> hardCodeRepositoryAsync, 
            IRepositoryAsync<NoisMainMeasureConfig> hardcodeRepositoryAsync, 
            ICacheManager cacheManager)

        {
            _mainMeasurRepositoryAsync = hardcodeRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task CreateAsync(NoisMainMeasureConfig hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURECONFIG_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.InsertAsync(hardCodeClass);
        }

        public Task UpdateAsync(NoisMainMeasureConfig hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURECONFIG_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.UpdateAsync(hardCodeClass);
        }

        public Task DeleteAsync(NoisMainMeasureConfig hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURECONFIG_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.DeleteAsync(hardCodeClass);
        }

        public Task<List<NoisMainMeasureConfig>> GetListMeasureLineAndMeasure(
            List<string> listLineCode, List<string> listMeasureType)
        {

            return _mainMeasurRepositoryAsync.GetMainMeasureByListLineIdAndDateAndMeasureAsync(listLineCode, listMeasureType);
        }

        public NoisMainMeasureConfig GetMainMeasureByLineCodeAndMeasureCode( string lineCode, string measureCode)
        {
            var key = string.Format(MEASURECONFIG_BY_LINECODE_MEASURECONFIGCODE__KEY, lineCode, measureCode);
            return _cacheManager.Get(key, () => _mainMeasurRepositoryAsync.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                lineCode, measureCode));
        }
    }
}
