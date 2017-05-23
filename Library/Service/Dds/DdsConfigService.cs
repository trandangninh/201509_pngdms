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
    public partial class DdsConfigService : BaseService<DdsConfig>, IDdsConfigService
    {
        #region constant for cache
        protected override string PatternKey
        {
            get { return "PG.DdsConfig."; }
        }

        private const string DDSCONFIG_BY_DEPARTMENT_KEY = "PG.DdsConfig.ByDepartment-{0}";
        #endregion

        private readonly IRepositoryAsync<DdsConfig> _ddsConfigRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;

        public DdsConfigService(IRepositoryAsync<DdsConfig> ddsConfigRepositoryAsync,
            ICacheManager cacheManager,
            IUserService userService)
            : base(ddsConfigRepositoryAsync, cacheManager)
        {
            this._ddsConfigRepositoryAsync = ddsConfigRepositoryAsync;
            this._cacheManager = cacheManager;
            this._userService = userService;
        }

        public List<DdsConfig> GetDdsConfigByDepartmentId(int departmentId)
        {
            if (departmentId <= 0)
                return null;

            //var key = string.Format(DDSCONFIG_BY_DEPARTMENT_KEY, departmentId);
            //return _cacheManager.Get(key, () => 
            return _ddsConfigRepositoryAsync.Table.Where(m => m.Measure.Dms.DepartmentId == departmentId).ToList();
        }
    }
}
