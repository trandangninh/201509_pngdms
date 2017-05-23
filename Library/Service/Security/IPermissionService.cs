using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain.Security;
using Entities.Domain.Users;
using Service.Interface;
using Entities.Domain.Departments;
using Entities.Domain;

namespace Service.Security
{
    public interface IPermissionService : IBaseService<PermissionRecord>
    {
        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        Task<PermissionRecord> GetPermissionRecordBySystemName(string systemName);

        /// <summary>
        /// Gets permissions by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<List<PermissionRecord>> GetPermissionRecordsByCategory(string category);

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        Task InstallPermissions(IPermissionProvider permissionProvider);

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        Task UninstallPermissions(IPermissionProvider permissionProvider);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="username">username</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> Authorize(PermissionRecord permission, string username);


        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">user</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(PermissionRecord permission, User user);

        bool Authorize(PermissionRecord permission);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">user</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName, User user);

        bool Authorize(string permissionRecordSystemName);

        bool Authorize(string permissionRecordSystemName, UserRole userRole);

        bool Authorize(Department department);

        bool Authorize(Department department, User user);

        bool Authorize(Issue issue);

        bool Authorize(Issue issue, User user);

        bool Authorize(Line line);

        bool Authorize(Line line, User user);

        //Task<List<PermissionRecord>> GetPermissionRecorsOfRole(UserRole userRole);

        //Task AddRoleToPermissionRecord(Role role, PermissionRecord permissionRecord);

        //Task RemoveRoleFromPermissionRecord(Role role, PermissionRecord permissionRecord);
    }
}
