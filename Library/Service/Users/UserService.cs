using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.Domain;

using RepositoryPattern.Repositories;
using Service.Users;
using Utils.Caching;
using System.Data.Entity;
using System;
using Utils;
using Entities.Domain.Users;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Service.Users
{
    public class UserService : BaseService<User>, IUserService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string USER_BY_GUID = "PG.user.guid-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : username
        /// </remarks>
        private const string USER_BY_USERNAME_KEY = "PG.user.byusername-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string USER_BY_ID_KEY = "PG.user.byid-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : email
        /// </remarks>
        private const string USER_BY_EMAIL_KEY = "PG.user.byemail-{0}";


        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string USER_ALL_KEY = "PG.user.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string USER_PATTERN_KEY = "PG.user.";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string USERROLE_ALL_KEY = "PG.userrole.all";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string USERROLE_PATTERN_KEY = "PG.userrole.";

        #endregion

        protected override string PatternKey
        {
            get { return "PG.User."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<UserRole> _userRoleRepositoryAsync;
        private readonly IRepositoryAsync<UserAllowInSupplyChain> _userSupplyChainRepositoryAsync;
        
        public UserService(ICacheManager cacheManager,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<UserRole> userRoleRepositoryAsync,
            IRepositoryAsync<UserAllowInSupplyChain> userSupplyChainRepositoryAsync)
            : base(userRepositoryAsync,cacheManager)
        {
            _userRepositoryAsync = userRepositoryAsync;
            _userRoleRepositoryAsync = userRoleRepositoryAsync;
            //_userMeetingRepositoryAsync = userMeetingRepositoryAsync;
            _userSupplyChainRepositoryAsync = userSupplyChainRepositoryAsync;
            _cacheManager = cacheManager;
        }
        #region User
        public User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;
            var key = String.Format(USER_BY_GUID, userGuid.ToString());
            return _cacheManager.Get(key, () => _userRepositoryAsync.Table.Where(x => !x.Deleted).FirstOrDefault(u => u.UserGuid == userGuid));
        }

        public User InsertGuestUser()
        {
            var user = new User
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                
                //Created = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guest);
            if (guestRole == null)
                throw new Exception("'Guests' role could not be loaded");
            user.UserRoles.Add(guestRole.Result);

            InsertUserAsync(user).Wait();

            return user;
        }

        public IPagedList<User> GetAllUsersAsync(string searchKeyword = null, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userRepositoryAsync.Table.AsQueryable();
            if (!string.IsNullOrEmpty(searchKeyword))
                query = query.Where(u =>
                    (u.Username != null && u.Username.Contains(searchKeyword)) ||
                    (u.Email != null && u.Email.Contains(searchKeyword)) ||
                    (u.FirstName != null && u.FirstName.Contains(searchKeyword)) ||
                    (u.LastName != null && u.LastName.Contains(searchKeyword)));

            if (active != null)
                query = query.Where(u => u.Active == active);

            query = query.OrderBy(u => u.Username);
            return new PagedList<User>(query, pageIndex, pageSize);

        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;
            var key = string.Format(USER_BY_USERNAME_KEY, username);
            return _cacheManager.Get(key, () => _userRepositoryAsync.Table.FirstOrDefaultAsync(u=>u.Username.Equals(username)));
        }

        public Task InsertUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _cacheManager.RemoveByPattern(USER_PATTERN_KEY);

            return _userRepositoryAsync.InsertAsync(user);
        }

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="createdFrom">Created date from; null to load all records</param>
        /// <param name="createdTo">Created date to; null to load all records</param>
        /// <returns>Number of deleted users</returns>
        
        public Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _cacheManager.RemoveByPattern(USER_PATTERN_KEY);

            return _userRepositoryAsync.UpdateAsync(user);
        }

        public Task DeleteUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _cacheManager.RemoveByPattern(USER_PATTERN_KEY);

            return _userRepositoryAsync.DeleteAsync(user);
        }
        public Task<User> GetUserByIdAsync(int id)
        {
            if (id==0)
                return null;

            var key = string.Format(USER_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _userRepositoryAsync.GetByIdAsync(id));
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var key = string.Format(USER_BY_EMAIL_KEY, email);
            return _cacheManager.Get(key, () => _userRepositoryAsync.Table.FirstOrDefaultAsync(u => u.Email == email));
        }

        //public Task<List<User>> GetUsersInMeeting(MeetingType meetingType)
        //{
        //    return _userMeetingRepositoryAsync.Table.Where(um => um.MeetingTypeId == (int) meetingType)
        //        .Select(um => um.User)
        //        .ToListAsync();
        //}

        //public Task DeleteAllUsersInMeeting(MeetingType meetingType)
        //{
        //    var users = _userMeetingRepositoryAsync.Table.Where(um => um.MeetingTypeId == (int) meetingType);

        //    return _userMeetingRepositoryAsync.DeleteAsync(users);
        //}

        //public Task AddUserToMeeting(MeetingType meetingType, User user)
        //{
        //    if(user == null)
        //        throw new ArgumentNullException("user");

        //    return AddUserToMeeting(meetingType, user.Id);
        //}

        //public Task AddUserToMeeting(MeetingType meetingType, int userId)
        //{
        //    return _userMeetingRepositoryAsync.InsertAsync(new UserAllowInMeeting()
        //    {
        //        MeetingType = meetingType,
        //        UserId = userId
        //    });
        //}

        public Task<List<User>> GetUsersInDms(DmsType dmsType)
        {
            return _userSupplyChainRepositoryAsync.Table.Where(um => um.DmsTypeId == (int)dmsType)
                .Select(um => um.User)
                .ToListAsync();
        }

        public Task DeleteAllUsersInDms(DmsType dmsType)
        {
            var users = _userSupplyChainRepositoryAsync.Table.Where(p => p.DmsTypeId == (int)dmsType);
            return _userSupplyChainRepositoryAsync.DeleteAsync(users);
        }

        public Task AddUserToDms(DmsType dmsType, User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return AddUserToDms(dmsType, user.Id);
        }

        public Task AddUserToDms(DmsType dmsType, int userId)
        {
            return _userSupplyChainRepositoryAsync.InsertAsync(new UserAllowInSupplyChain
            {
                DmsType = dmsType,
                UserId = userId
            });
        }
        private void HeaderStyle(ExcelRangeBase cell, string text)
        {
            cell.Value = text;
            cell.Style.Font.Size = 12;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
            cell.Style.Font.Color.SetColor(Color.White); // set color font
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void ExportToXlsx(Stream stream, IPagedList<User> report, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("User List");

                var header = new Dictionary<string, string>
                {
                    { "A1", "Account user ID"},
                    { "B1", "Full name"},
                    { "C1", "Role"},
                    { "D1", "Department"},
                    { "E1", "Active"},
                    { "F1", "Created date"}
                };

                foreach (var cell in header)
                {
                    HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
                }


                #region create headers

                worksheet.Column(1).Width = 25;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 50;
                worksheet.Column(4).Width = 50;
                worksheet.Column(5).Width = 10;
                worksheet.Column(6).Width = 15;

                worksheet.Cells[1, 1, report.Count + 1, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                #endregion

                #region create  content for excell           

                for (int i = 0; i < report.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = report[i].Username;
                    worksheet.Cells[i + 2, 2].Value = report[i].LastName + " " + report[i].FirstName;
                    worksheet.Cells[i + 2, 3].Value = String.Join(", ", report[i].UserRoles.Select(r=>r.Name));
                    worksheet.Cells[i + 2, 4].Value = String.Join(", ", report[i].Departments.Select(r => r.Name));
                    worksheet.Cells[i + 2, 5].Value = report[i].Active;
                    worksheet.Cells[i + 2, 6].Value = report[i].LastLoginDate.ToString("dd-MMM-yyyy");
                    
                }
                #endregion

                if (String.IsNullOrEmpty(path))
                {
                    xlPackage.Save(); // save to excell
                }
                else
                {
                    //Stream streamPath = File.Create(path);
                    //xlPackage.SaveAs(streamPath);

                    xlPackage.SaveAs(new FileInfo(path));

                    stream.Close();
                }
            }

        }

        #endregion

        #region User role
        public Task<List<UserRole>> GetAllUserRolesAsync() 
        {
            var key = String.Format(USERROLE_ALL_KEY);
            return _cacheManager.Get(key, () => _userRoleRepositoryAsync.Table.ToListAsync());
        }


        public UserRole GetUserRoleByIdAsync(int userId)
        {
            return _userRoleRepositoryAsync.GetById(userId);
        }


        public Task<UserRole> GetUserRoleBySystemName(string systemName)
        {
            return _userRoleRepositoryAsync.Table.FirstOrDefaultAsync(r => r.SystemName == systemName);
        }
        #endregion








        
        /*

        public Task CreateAsync(User user)
        {
            _cacheManager.RemoveByPattern(USER_PATTERN_KEY);

            return _userRepositoryAsync.InsertAsync(user);
        }

        

        public Task DeleteUser(User user)
        {
            _cacheManager.RemoveByPattern(USER_PATTERN_KEY);
            _userRepositoryAsync.Delete(user);
            return null;//_unitOfWorkAsync.SaveChangesAsync();
        }

        public ClaimsIdentity FindByUserId(string userId)
        {
            var claims = new ClaimsIdentity();
            //var listOfClaim = _userClaimRepository.Queryable().Where(p => p.UserId == userId);
            //foreach (var userClaim in listOfClaim)
            //{
            //    claims.AddClaim(new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            //}
            return claims;

        }

        public Task<int> DeleteAllClaimOfUser(string userId)
        {
            //var listOfClaim = _userClaimRepository.Queryable().Where(p => p.UserId == userId);
            //foreach (var userClaim in listOfClaim)
            //{
            //    _userClaimRepository.Delete(userClaim);
            //}
            return null;//_unitOfWorkAsync.SaveChangesAsync();
        }

        public Task<int> DeleteClaimOfUser(User user, Claim userClaim)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> InsertClaimForUser(Claim userClaim, string userId)
        {
            throw new System.NotImplementedException();
        }

        //public Task AddLoginAsync(User user, UserLoginInfo login)
        //{
        //    var newUserLogin = new UserLogin()
        //    {
        //        //Khang comment UserId = user.Id,
        //        LoginProvider = login.LoginProvider,
        //        ProviderKey = login.ProviderKey
        //    };
        //    //_userLoginRepository.Insert(newUserLogin);
        //    return null;//_unitOfWorkAsync.SaveChangesAsync();
        //}

        //public Task RemoveLoginAsync(User user, UserLoginInfo login)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        //{
        //    //var listUserLogin = _userLoginRepository.Queryable().Where(p => p.UserId == user.Id);
        //    var result = new List<UserLoginInfo>();
        //    //foreach (var userLoginInfo in listUserLogin)
        //    {
        //    //    result.Add(new UserLoginInfo(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey));
        //    }

        //    return Task.FromResult<IList<UserLoginInfo>>(result);
        //}

        //public string FindUserIdByLogin(UserLoginInfo login)
        //{
        //    //var userLogin = _userLoginRepository.Queryable().FirstOrDefault(p => p.ProviderKey == login.ProviderKey && p.LoginProvider == login.LoginProvider);
        //    //if (userLogin != null)
        //    //    return userLogin.UserId;
        //    //else 
        //    return null;
        //}

        public Task AddToRoleAsync(User user, string roleName)
        {
            var roleId = _roleRepositoryAsync.Table.FirstOrDefault(p => p.Name == roleName);
            if (roleId == null)
                return Task.FromResult<object>(null);

            var newUserRole = new UserRole()
            {
                //UserId = user.Id,
                //RoleId = roleId.Id
            };
            //_userRoleRepository.Insert(newUserRole);
            return null;//_unitOfWorkAsync.SaveChangesAsync();


        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            var roleId = _roleRepositoryAsync.Table.FirstOrDefault(p => p.Name == roleName);
            if (roleId == null)
                return Task.FromResult<object>(null);

            //var removedUserRole =
            //    _userRoleRepository.Queryable().FirstOrDefault(p => p.UserId == user.Id && p.RoleId == roleId.Id);
            //if (removedUserRole == null)
            //    return Task.FromResult<object>(null);
            //_userRoleRepository.Delete(removedUserRole);
            return null;//_unitOfWorkAsync.SaveChangesAsync(); 
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            IList<string> result = new List<string>();
            //var listUserUserRole = _userRoleRepository.Queryable().Where(p => p.UserId == user.Id).Select(p => p.RoleId).ToList();

            //foreach (var userRole in listUserUserRole)
            //{
            //    var role = _roleRepositoryAsync.Find(userRole);
            //    if (role != null) result.Add(role.Name);
            //}
            return Task.FromResult<IList<string>>(result);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            var roleId = _roleRepositoryAsync.Table.FirstOrDefault(p => p.Name == roleName);
            if (roleId == null)
                return Task.FromResult<bool>(false);
            //var checkedUserRole =
            //   _userRoleRepository.Queryable().FirstOrDefault(p => p.UserId == user.Id && p.RoleId == roleId.Id);
            //if (checkedUserRole == null)
            //    return Task.FromResult<bool>(false);
            return Task.FromResult<bool>(true);
        }

        //public Task<List<User>> GetAllUser()
        //{
        //    var key = string.Format(USER_ALL_KEY);
        //    return _cacheManager.Get(key, () => _userRepositoryAsync.GetAllUser());

        //}

        public List<User> GetAll()
        {
            var userRepository = _userRepositoryAsync.GetRepository<User>();
            return userRepository.Table.Where(p => p.Username != "Admin").OrderBy(p => p.Username).ToList();
        }

        public List<User> GetAllActive()
        {
            var userRepository = _userRepositoryAsync.GetRepository<User>();
            return userRepository.Table.Where(p => p.Username != "Admin" && p.IsActive == true).OrderBy(p => p.Username).ToList();
        }

        public Task<IList<Role>> GetRolesOfUserAsync(User user)
        {
            IList<Role> result = new List<Role>();
            //var listUserUserRole = _userRoleRepository.Queryable().Where(p => p.UserId == user.Id).Select(p => p.RoleId).ToList();

            //foreach (var userRole in listUserUserRole)
            {
           //     var role = _roleRepositoryAsync.Table.FirstOrDefault(p => p.Id == userRole);
           //     if (role != null) result.Add(role);
            }
            return Task.FromResult<IList<Role>>(result);
        }
         */
    }
}
