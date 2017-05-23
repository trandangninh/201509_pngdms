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
    public class DdsMeetingPrDetailService : BaseService<DdsMeetingPrDetail>, IDdsMeetingPrDetailService
    {
        private const string DDSMEETINGDETAIL_BY_RESULT_KEY = "PG.DdsMeetingPrDetail.ByDdsMeetingResult-{0}";

        private readonly IRepositoryAsync<DdsMeetingPrDetail> _tRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        public DdsMeetingPrDetailService(IRepositoryAsync<DdsMeetingPrDetail> tRepositoryAsync, 
            ICacheManager cacheManager) : base(tRepositoryAsync, cacheManager)
        {
            this._tRepositoryAsync = tRepositoryAsync;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get { return "PG.DdsMeetingPrDetail."; }
        }

        public Task<DdsMeetingPrDetail> GetByDdsMeetingResult(int ddsMeetingResult)
        {
            var key = String.Format(DDSMEETINGDETAIL_BY_RESULT_KEY, ddsMeetingResult);
            return _cacheManager.Get(key, () => { return _tRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DdsMeetingResultId == ddsMeetingResult); });
            
        }
    }
}
