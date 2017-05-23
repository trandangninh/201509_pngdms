using Entities.Domain;
using Entities.Domain.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utils;

namespace Service.Users
{
    public interface IUserService : IBaseService<User>
    {
        #region User
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        IPagedList<User> GetAllUsersAsync(string searchKeyword = null, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Task of user</returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task UpdateUserAsync(User user);
        /// <summary>
        /// Insert user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task InsertUserAsync(User user);
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        Task DeleteUserAsync(User user);
        /// <summary>
        /// Get user by identify
        /// </summary>
        /// <param name="id">Identify</param>
        /// <returns>User</returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>User</returns>
        Task<User> GetUserByEmailAsync(string email);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingType"></param>
        /// <returns></returns>
        //Task<List<User>> GetUsersInMeeting(MeetingType meetingType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingType"></param>
        /// <returns></returns>
        //Task DeleteAllUsersInMeeting(MeetingType meetingType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        //Task AddUserToMeeting(MeetingType meetingType, User user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //Task AddUserToMeeting(MeetingType meetingType, int userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsType"></param>
        /// <returns></returns>
        Task<List<User>> GetUsersInDms(DmsType dmsType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsType"></param>
        /// <returns></returns>
        Task DeleteAllUsersInDms(DmsType dmsType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task AddUserToDms(DmsType dmsType, User user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddUserToDms(DmsType dmsType, int userId);
        /// <summary>
        /// Get user by guid
        /// </summary>
        /// <param name="userGuid">user guid</param>
        /// <returns></returns>
        User GetUserByGuid(Guid userGuid);
        User InsertGuestUser();
        void ExportToXlsx(Stream stream, IPagedList<User> report, string path);
        #endregion

        #region User role
        Task<List<UserRole>> GetAllUserRolesAsync();

        UserRole GetUserRoleByIdAsync(int userId);
        Task<UserRole> GetUserRoleBySystemName(string systemName);
        #endregion


        /*



        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task  CreateAsync(User user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task DeleteUser(User user);

        #region claim

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ClaimsIdentity FindByUserId(string userId);

        /// <summary>
        ///  Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteAllClaimOfUser(string userId);

        /// <summary>
        /// Deletes a claim from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userClaim"></param>
        /// <returns></returns>
        Task<int> DeleteClaimOfUser(User user, Claim userClaim);

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> InsertClaimForUser(Claim userClaim, string userId);



        #endregion

        #region login

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
       // Task AddLoginAsync(User user, UserLoginInfo login);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        //Task RemoveLoginAsync(User user, UserLoginInfo login);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //Task<IList<UserLoginInfo>> GetLoginsAsync(User user);

        //string FindUserIdByLogin(UserLoginInfo login);

        #endregion

        #region role

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task AddToRoleAsync(User user, string roleName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task RemoveFromRoleAsync(User user, string roleName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<string>> GetRolesAsync(User user);

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> IsInRoleAsync(User user, string roleName);

        #endregion

        #region manage

        //Task<List<User>> GetAllUser();

        List<User> GetAll();

        List<User> GetAllActive();

        #endregion



        #region extra function
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<Role>> GetRolesOfUserAsync(User user);
        #endregion
        */

        
    }
}
