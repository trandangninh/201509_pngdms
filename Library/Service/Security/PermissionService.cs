using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Departments;
using Entities.Domain.Security;
using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using Service.Interface;
using Service.Users;
using Utils;
using Utils.Caching;

namespace Service.Security
{
    public class PermissionService : BaseService<PermissionRecord>, IPermissionService
    {
        protected override string PatternKey
        {
            get { return "PG.permission."; }
        }

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user role ID
        /// {1} : permission system name
        /// </remarks>
        private const string PERMISSIONS_ALLOWED_KEY = "PG.permission.allowed-{0}-{1}";

        private readonly IRepositoryAsync<PermissionRecord> _permissionRecordRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IUserRoleService _userRoleService;


        public PermissionService(IRepositoryAsync<PermissionRecord> permissionRecordRepositoryAsync,
            ICacheManager cacheManager, 
            IUserService userService,
            IWorkContext workContext)
            : base(permissionRecordRepositoryAsync, cacheManager)

        {
            _permissionRecordRepositoryAsync = permissionRecordRepositoryAsync;
            _cacheManager = cacheManager;
            _userService = userService;
            _workContext = workContext;
        }

        public Task<PermissionRecord> GetPermissionRecordBySystemName(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                return null;
            return _permissionRecordRepositoryAsync.Table.FirstOrDefaultAsync(p => p.SystemName == systemName);
            
        }

        public Task<List<PermissionRecord>> GetPermissionRecordsByCategory(string category)
        {
            return _permissionRecordRepositoryAsync.Table.Where(p=>p.Category == category).ToListAsync();

        }

        public async Task InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionRecordBySystemName(permission.SystemName);
                //List<Role> listRoleToAdd = new List<Role>();
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord
                    {
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category,
                    };


                    //default customer role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        //var role =await _roleService.GetRoleByName(defaultPermission.CustomerRoleSystemName);
                        //if (role != null)
                        //{
                        //    var permissionOfRole = await GetPermissionRecorsOfRole(role);
                        //    var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                        //                                  where p.SystemName == permission1.SystemName
                        //                                  select p).Any();
                        //    var mappingExists = (from p in permissionOfRole
                        //                         where p.SystemName == permission1.SystemName
                        //                         select p).Any();
                        //    if (defaultMappingProvided && !mappingExists)
                        //    {
                        //        //add role for permission
                        //        //listRoleToAdd.Add(role);
                        //    }   
                        //}
                    }

                    //save new permission
                    await InsertAsync(permission1);
                    //foreach (var role in listRoleToAdd)
                    //{
                    //    await AddRoleToPermissionRecord(role, permission1);
                    //}
                }
            }
        }

        public Task UninstallPermissions(IPermissionProvider permissionProvider)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Authorize(PermissionRecord permission, string username)
        {
            if (permission == null)
                return false;

            if (String.IsNullOrEmpty(username))
                return false;

            var user = await _userService.GetUserByUsernameAsync(username);
            //return false;
            if (user == null) return false;
            return Authorize(permission.SystemName, user);
        }

        public bool Authorize(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            //if (user == null)
            //    return false;

           
            //return false;

            return Authorize(permission.SystemName, user);
        }

        public bool Authorize(PermissionRecord permission)
        {
            //if (_workContext.CurrentUser == null)
            //    return false;

            return Authorize(permission, _workContext.CurrentUser);
        }

        public bool Authorize(string permissionRecordSystemName)
        {
            //if (_workContext.CurrentUser == null)
             //   return false;

            return Authorize(permissionRecordSystemName, _workContext.CurrentUser);
        }

        public bool Authorize(string permissionRecordSystemName, User user)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            if (user != null)
            {
                foreach (var role in user.UserRoles)
                    if (Authorize(permissionRecordSystemName, role))
                        //yes, we have such permission
                        return true;
                if (user.IsLeader())
                {
                    var meetingRole = _userService.GetUserRoleBySystemName(SystemUserRoleNames.MeetingLeaders).Result;
                    if (Authorize(permissionRecordSystemName, meetingRole))
                        return true;
                }
            }
            else
            {
                var guestRole = _userService.GetUserRoleBySystemName(SystemUserRoleNames.Guest).Result;
                return Authorize(permissionRecordSystemName, guestRole);
            }
            return false;
        }


        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="userRole">Customer role</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public bool Authorize(string permissionRecordSystemName, UserRole userRole)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            string key = string.Format(PERMISSIONS_ALLOWED_KEY, userRole.Id, permissionRecordSystemName);
            return _cacheManager.Get(key, () =>
            {
                foreach (var permission1 in userRole.PermissionRecords)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        public bool Authorize(Department department)
        {
            return Authorize(department, _workContext.CurrentUser);
        }

        public bool Authorize(Department department, User user)
        {
            if (department == null)
                return false;
            if (user == null)
                return false;

            if (user.IsAdmin() || user.Departments.Count() == 0)
                return true;

            return user.Departments.Any(d => d.Id == department.Id);
        }

        public bool Authorize(Issue issue)
        {
            return Authorize(issue, _workContext.CurrentUser);
        }

        public bool Authorize(Issue issue, User user)
        {
            if (issue == null)
                return false;
            if (user == null)
                return false;

            if (user.IsAdmin())
                return true;
            if (issue.UserOwnerId == user.Id)
                return true;

            return user.IsLeader(issue.DepartmentId);
        }
        public bool Authorize(Line line)
        {
            return Authorize(line, _workContext.CurrentUser);
        }

        public bool Authorize(Line line, User user)
        {
            if (line == null)
                return false;
            if (user == null)
                return false;

            if (user.IsAdmin())
                return true;
            if (line.Users.Any(u=>u.Id == user.Id))
                return true;

            return user.IsLeader(line.DepartmentId);
        }
        #region role and permission

        //public Task AddRoleToPermissionRecord(Role role, PermissionRecord permissionRecord)
        //{

        //    var rolePermission =
        //        _rolePermissionRepositoryAsync.Table
        //            .FirstOrDefault(p => p.PermissionId == permissionRecord.Id && p.RoleId == role.Id);
        //    if (rolePermission == null)
        //    {
        //        _rolePermissionRepositoryAsync.Insert(new RolePermission()
        //        {
        //            PermissionId = permissionRecord.Id,
        //            RoleId = role.Id
        //        });
        //        return null;//_unitOfWorkAsync.SaveChangesAsync();
        //    }

        //    return Task.FromResult<int>(0);

        //}

        //public Task RemoveRoleFromPermissionRecord(Role role, PermissionRecord permissionRecord)
        //{
        //    var rolePermission =
        //        _rolePermissionRepositoryAsync.Table
        //            .FirstOrDefault(p => p.PermissionId == permissionRecord.Id && p.RoleId == role.Id);
        //    if (rolePermission != null)
        //    {
        //        _rolePermissionRepositoryAsync.Delete(rolePermission);
        //        return null;//_unitOfWorkAsync.SaveChangesAsync();
        //    }
        //    return Task.FromResult<int>(0);
        //}

        #endregion
    }
}
