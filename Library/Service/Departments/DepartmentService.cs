using Entities.Domain.Departments;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Caching;
using Utils;
using System.Data.Entity;
using Entities.Domain.Users;

namespace Service.Departments
{
    public partial class DepartmentService : BaseService<Department>, IDepartmentService 
    {
        protected override string PatternKey
        {
            get { return "PG.Department."; }
        }
        #region constant for cache

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : departmentName
        /// </remarks>
        private const string DEPARTMENT_BY_NAME = "PG.Department.byname-{0}";

        #endregion

        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly ICacheManager _cacheManager;


        public DepartmentService(IRepositoryAsync<Department> departmentRepositoryAsync,
            ICacheManager cacheManager)
            : base(departmentRepositoryAsync, cacheManager)
        {
            this._departmentRepositoryAsync = departmentRepositoryAsync;
            this._cacheManager = cacheManager;
        }

        public IPagedList<Department> SearchDepartment(User user = null, bool? isActive = null, bool includeSupplyChain = true, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _departmentRepositoryAsync.Table.AsQueryable();
            if (user != null && user.Departments.Count() != 0)
            {
                var udIds = user.Departments.Select(d => d.Id).ToList();
                query = query.Where(d => udIds.Any(id => id == d.Id));
            }
            if(isActive.HasValue)
            {
                query = query.Where(d => d.Active == isActive.Value);
            }
            query = query.Where(d => d.DepartmentTypeId != (int) DepartmentType.SupplyChain || includeSupplyChain);
            query = query.OrderBy(d => d.Order);
            return new PagedList<Department>(query, pageIndex, pageSize);
        }

        public Task<Department> GetDepartmentByDepartmentName(string departmentName)
        {
            if (String.IsNullOrEmpty(departmentName))
                return null;
            var key = string.Format(DEPARTMENT_BY_NAME, departmentName);
            return _cacheManager.Get(key, () =>
                _departmentRepositoryAsync.Table.FirstOrDefaultAsync(d => d.Name == departmentName));
        }

        public Task<Department> GetSupplyChainDepartment()
        {
            return _departmentRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DepartmentTypeId == (int) DepartmentType.SupplyChain);
        }

        public override Task<IPagedList<Department>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PatternKey + "all");

            return _cacheManager.Get(key, () =>
            {
                var query = _departmentRepositoryAsync.Table.OrderBy(x => x.Order);
                return Task.FromResult(new PagedList<Department>(query, pageIndex, pageSize) as IPagedList<Department>);
            }
            );
        }

        public Task<List<Department>> GetDepartmentByIdsAsync(List<int> ids)
        {
            return _departmentRepositoryAsync.Table.Where(d => ids.Any(id => id == d.Id) && d.Active).ToListAsync();
        }
    }    
}
