using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using Utils.Caching;
using Utils;

namespace Service.Departments
{
    public class MeasureService : BaseService<Measure>, IMeasureService
    {
        protected override string PatternKey
        {
            get { return "PG.measure."; }
        }

        #region constand

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : code
        /// </remarks>
        private const string MEASURE_BY_CODE_TYPE_KEY = "PG.measure.bycode-{0}-{1}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : measure code
        /// {1} : dmsId
        /// </remarks>
        private const string MEASURE_BY_MEASURECODEANDDMSID_KEY = "PG.measure.bymeasurecodeanddmsid-{0}-{1}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : code
        /// </remarks>
        private const string MEASURES_BY_DMSID_KEY = "PG.measure.bydmsid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : departmentId
        /// </remarks>
        private const string ALLMEASURE_BY_DEPARTMENTID_KEY = "PG.measure.bydepartmentid-{0}";

        #endregion


        private readonly IRepositoryAsync<Measure> _measureRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly ICacheManager _cacheManager;

        public MeasureService(IRepositoryAsync<Measure> measureRepository, 
            ICacheManager cacheManager,
            IRepositoryAsync<User> userRepositoryAsync) : base(measureRepository, cacheManager)
        {
            _measureRepositoryAsync = measureRepository;
            _cacheManager = cacheManager;
            _userRepositoryAsync = userRepositoryAsync;
        }


        public Task<Measure> GetMeasureByCode(string code,int dmsLiquidType)
        {
            if (String.IsNullOrEmpty(code))
                return null;
            var key = string.Format(MEASURE_BY_CODE_TYPE_KEY, code, dmsLiquidType);
            return _cacheManager.Get(key, () =>
                _measureRepositoryAsync.Table.FirstOrDefaultAsync(x => x.MeasureCode == code && x.Dms.DepartmentId == dmsLiquidType));
        }

        public Task<List<Dms>> GetListDmsByMeasureCode(List<string> listMeasureCode)
        {
            var listMeasure = _measureRepositoryAsync.Table.Include(p=>p.Dms).Where(p => listMeasureCode.Contains(p.MeasureCode));
            return listMeasure.Select(p => p.Dms).ToListAsync();
        }

        public Task<Measure> GetMeasureByMeasureCodeAndDmsId(string measureCode, int dmsId)
        {
            var key = string.Format(MEASURE_BY_MEASURECODEANDDMSID_KEY, measureCode, dmsId);
            return _cacheManager.Get(key, () => _measureRepositoryAsync.Table.FirstOrDefaultAsync(m => m.MeasureCode == measureCode && m.DmsId == dmsId));
        }

        public Task UpdateMeasureOwner(Measure measure, IEnumerable<string> usernames)
        {
            var deletionUsers = measure.Users.Where(u => !usernames.Contains(u.Username)).ToList();
            foreach (var user in deletionUsers)
            {
                measure.Users.Remove(user);
            }
            foreach (var username in usernames)
            {
                if (measure.Users.FirstOrDefault(u => u.Username == username) == null)
                {
                    var user = _userRepositoryAsync.Table.FirstOrDefaultAsync(u => u.Username == username);
                    measure.Users.Add(user.Result);
                }
            }
            return UpdateAsync(measure);
        }

        public Task<IPagedList<Measure>> GetAllMeasureByDmsId(int? dmsId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (dmsId <= 0)
                return null;

            var key = string.Format(MEASURES_BY_DMSID_KEY, dmsId);
            //return _cacheManager.Get(key, () => Task.FromResult(new PagedList<Measure>(_measureRepositoryAsync.Table.Where(d => d.DmsId == dmsId).OrderBy(d => d.Id), pageIndex, pageSize) as IPagedList<Measure>));
            
            return _cacheManager.Get(key, () =>
            {
                var query = _measureRepositoryAsync.Table.OrderBy(d => d.Order).AsQueryable();
                if (dmsId.HasValue)
                {
                    query = query.Where(d => d.DmsId == dmsId.Value);
                }
                return Task.FromResult(new PagedList<Measure>(query, pageIndex, pageSize) as IPagedList<Measure>);
            });
        }

        public List<Measure> GetAllMeasureByDepartmentId(int departmentId)
        {
            if (departmentId <= 0)
                return null;

            //var key = string.Format(ALLMEASURE_BY_DEPARTMENTID_KEY, departmentId);
            //return _cacheManager.Get(key, () =>
            return _measureRepositoryAsync.Table.Where(m => m.Dms.DepartmentId == departmentId && m.Active && m.Dms.Active).OrderBy(m => m.Order).ToList();
            //);
        }
    }
}
