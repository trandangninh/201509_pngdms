using Entities.Domain.Suppliers;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;
using Entities.Domain.ClassificationDefects;

namespace Service.Suppliers
{
    public class ScMeasureTargetService : BaseService<ScMeasureTarget>, IScMeasureTargetService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string SCMEASURETARGET_BY_SUPPLIERIDANDSCMEASUREID = "PG.ScMeasureTarget.ListScMeasureTargetBySupplierIdAndScMeasureId-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string SCMEASURETARGET_BY_SUPPLIERID = "PG.ScMeasureTarget.ListScMeasureTargetBySupplierId-{0}";

        protected override string PatternKey
        {
            get { return "PG.ScMeasureTarget."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<ScMeasureTarget> _scMeasureTargetRepositoryAsync;

        public ScMeasureTargetService(ICacheManager cacheManager,
            IRepositoryAsync<ScMeasureTarget> scMeasureTargetRepositoryAsync)
            : base(scMeasureTargetRepositoryAsync, cacheManager)
        {
            _scMeasureTargetRepositoryAsync = scMeasureTargetRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// get score card measure target by supplier identiy and scmeasure identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <returns></returns>
        public Task<ScMeasureTarget> GetScMeasureTargetBySupplierIdAndScMeasureId(int supplierId, int scMeasureId)
        {
            var key = string.Format(SCMEASURETARGET_BY_SUPPLIERIDANDSCMEASUREID, supplierId, scMeasureId);
            return _cacheManager.Get(key, () => _scMeasureTargetRepositoryAsync.Table.FirstOrDefaultAsync(x => x.SupplierId == supplierId && x.ScMeasureId == scMeasureId));
        }

        /// <summary>
        /// get all ScMeasureTarget by supplier identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public Task<List<ScMeasureTarget>> GetAllScMeasureTargetBySupplierId(int supplierId)
        {
            var key = string.Format(SCMEASURETARGET_BY_SUPPLIERID, supplierId);
            return _cacheManager.Get(key, () => _scMeasureTargetRepositoryAsync.Table.Where(x => x.SupplierId == supplierId).ToListAsync());
        }
    }       
}
