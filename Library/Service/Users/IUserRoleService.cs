using Entities.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Users
{
    public interface IUserRoleService : IBaseService<UserRole>
    {
        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <returns>paged list user roles</returns>
        Task<IPagedList<UserRole>> GetAllUserRolesAsync(bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check role name has existed
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckRoleNameHasExisted(string name);

        /// <summary>
        /// get userRole by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<UserRole> GetUserRoleByNameAsync(string name);
        /// <summary>
        /// Get user roles by identities
        /// </summary>
        /// <param name="ids">User role identities</param>
        /// <returns></returns>
        Task<List<UserRole>> GetUserRolesByIdsAsync(List<int> ids);
    }
}
