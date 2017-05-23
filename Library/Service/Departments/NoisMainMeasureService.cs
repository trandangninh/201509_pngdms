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
    public class NoisMainMeasureService : INoisMainMeasureService
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
        private const string MEASURE_BY_LINECODE_MEASURECODE_DATE_KEY = "PG.noismeasure.byid-{0}-{1}-{2}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MEASURE_PATTERN_KEY = "PG.noismeasure.";

      

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<NoisMainMeasure> _mainMeasurRepositoryAsync;
        public NoisMainMeasureService(IRepositoryAsync<NoisMainMeasure> hardcodeRepositoryAsync, 
            ICacheManager cacheManager)

        {
            _mainMeasurRepositoryAsync = hardcodeRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task CreateAsync(NoisMainMeasure hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURE_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.InsertAsync(hardCodeClass);
        }

        public Task UpdateAsync(NoisMainMeasure hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURE_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.UpdateAsync(hardCodeClass);
        }

        public Task DeleteAsync(NoisMainMeasure hardCodeClass)
        {
            _cacheManager.RemoveByPattern(MEASURE_PATTERN_KEY);
            return _mainMeasurRepositoryAsync.DeleteAsync(hardCodeClass);
        }

        public Task<List<NoisMainMeasure>> GetListMeasureByDateAndLineAndMeasure(DateTime createdDate, List<string> listLineCode, List<string> listMeasureType )
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            //var notAsyncRepo = repository.GetRepository<NoisMainMeasure>();
            //var listResultInDay = notAsyncRepo.Table.Where(p => p.CreatedDateTime < endDay && p.CreatedDateTime >= startDay);
            //var result = listResultInDay.Where(p => listLineCode.Contains(p.LineHardCode.ToString()));
            //result = result.Where(p => listMeasure.Contains(p.TypeHardCode.ToString()));
            //return result.ToListAsync();
            return null;


           return  _mainMeasurRepositoryAsync.GetMainMeasureByListLineIdAndDateAndMeasureAsync(createdDate,
                listLineCode, listMeasureType);
        }

        public NoisMainMeasure GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime createdDate, string lineCode, string measureCode)
        {
            var key = string.Format(MEASURE_BY_LINECODE_MEASURECODE_DATE_KEY, lineCode,measureCode,createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _mainMeasurRepositoryAsync.GetMainMeasureByLineCodeAndMeasureCodeAndDate(createdDate,
                lineCode, measureCode));
        }
    }
}
