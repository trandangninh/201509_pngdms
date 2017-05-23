using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Dds;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.Dds
{
    public class DdsMeetingDetailService : BaseService<DdsMeetingDetail>, IDdsMeetingDetailService
    {
        private const string DDSMEETINGDETAIL_BY_RESULT_KEY = "PG.DdsMeetingDetail.ByDdsMeetingResult-{0}";

        private readonly IRepositoryAsync<DdsMeetingDetail> _tRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        public DdsMeetingDetailService(IRepositoryAsync<DdsMeetingDetail> tRepositoryAsync, 
            ICacheManager cacheManager) : base(tRepositoryAsync, cacheManager)
        {
            this._tRepositoryAsync = tRepositoryAsync;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get { return "PG.DdsMeetingDetail."; }
        }

        public Task<DdsMeetingDetail> GetByDdsMeetingResult(int ddsMeetingResult)
        {
            var key = String.Format(DDSMEETINGDETAIL_BY_RESULT_KEY, ddsMeetingResult);
            return _cacheManager.Get(key, () => { return _tRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DdsMeetingResultId == ddsMeetingResult); });
            
        }
    }
}
