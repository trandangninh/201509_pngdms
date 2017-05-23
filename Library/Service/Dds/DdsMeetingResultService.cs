using Entities.Domain.Dds;
using RepositoryPattern.Repositories;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Caching;
using System.Data.Entity;

namespace Service.Dds
{
    public partial class DdsMeetingResultService : BaseService<DdsMeetingResult>, IDdsMeetingResultService
    {
        #region constant for cache
        protected override string PatternKey
        {
            get { return "PG.DdsMeetingResult."; }
        }

        private const string DDSMEETINGRESULT_BY_MEASUREID_LINEID_DATE_KEY = "PG.DdsMeetingResult.ByMeasureIdLineIdDate-{0}-{1}-{2}";
        #endregion

        private readonly IRepositoryAsync<DdsMeetingResult> _ddsMeetingResultRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;

        public DdsMeetingResultService(IRepositoryAsync<DdsMeetingResult> ddsMeetingResultRepositoryAsync,
            ICacheManager cacheManager,
            IUserService userService)
            : base(ddsMeetingResultRepositoryAsync, cacheManager)
        {
            this._ddsMeetingResultRepositoryAsync = ddsMeetingResultRepositoryAsync;
            this._cacheManager = cacheManager;
            this._userService = userService;
        }

        //public List<DdsConfig> GetDdsConfigByDepartmentId(int departmentId)
        //{
        //    if (departmentId <= 0)
        //        return null;

        //    var key = string.Format(DDSCONFIG_BY_DEPARTMENT_KEY, departmentId);
        //    return _cacheManager.Get(key, () => _ddsConfigRepositoryAsync.Table.Where(m => m.Measure.Dms.DepartmentId == departmentId).ToList());
        //}
        public Task<DdsMeetingResult> GetDdsMeetingResultByMeasureIdAndLineIdAndDate(int measureId, int lineId, DateTime date)
        {
            if (measureId < 1 || lineId < 1)
                return null;
            var key = string.Format(DDSMEETINGRESULT_BY_MEASUREID_LINEID_DATE_KEY, measureId, lineId, date);
            return _cacheManager.Get(key, () => 
                        _ddsMeetingResultRepositoryAsync.Table.FirstOrDefaultAsync(d => d.MeasureId == measureId && d.LineId == lineId && d.DdsMeeting.CreatedDateTime == date)
                        );
        }
    }
}
