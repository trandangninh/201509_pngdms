using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;

namespace Service.Users
{
    public class UserRoleService : BaseService<UserRole>, IUserRoleService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string USERROLE_ALL_KEY = "PG.UserRole.All-{0}-{1}-{2}";

        private const string USERROLE_BY_NAME = "PG.UserRole.ByName-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string USERROLE_PATTERN_KEY = "PG.UserRole.";

        protected override string PatternKey
        {
            get { return "PG.UserRole."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<UserRole> _userRoleRepositoryAsync;

        public UserRoleService(ICacheManager cacheManager,
            IRepositoryAsync<UserRole> userRoleRepositoryAsync)
            : base(userRoleRepositoryAsync,cacheManager)
        {
            _userRoleRepositoryAsync = userRoleRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <returns>paged list user roles</returns>
        public Task<IPagedList<UserRole>> GetAllUserRolesAsync(bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {

            var key = string.Format(USERROLE_ALL_KEY, isActive.HasValue ? (isActive.Value ? 1 : 0) : -1, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _userRoleRepositoryAsync.Table.AsQueryable();
                if (isActive.HasValue)
                    query = query.Where(u => u.IsActive == isActive.Value);

                query = query.OrderBy(u => u.SystemName);
                return Task.FromResult(new PagedList<UserRole>(query, pageIndex, pageSize) as IPagedList<UserRole>);
            }
            );
        }

        /// <summary>
        /// check role name has existed
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckRoleNameHasExisted(string name)
        {
            return Task.FromResult(_userRoleRepositoryAsync.Table.Any(ur => ur.Name == name));
        }

        /// <summary>
        /// get userRole by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<UserRole> GetUserRoleByNameAsync(string name)
        {
            var key = string.Format(USERROLE_BY_NAME, name);
            return _cacheManager.Get(key, () => _userRoleRepositoryAsync.Table.FirstOrDefaultAsync(u => u.Name == name));
        }

        /// <summary>
        /// Get user roles by identities
        /// </summary>
        /// <param name="ids">User role identities</param>
        /// <returns></returns>
        public Task<List<UserRole>> GetUserRolesByIdsAsync(List<int> ids)
        {
            return _userRoleRepositoryAsync.Table.Where(r => ids.Contains(r.Id)).ToListAsync();
        }
    }
}
