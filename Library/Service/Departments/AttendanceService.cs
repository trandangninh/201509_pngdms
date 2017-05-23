using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Service.Users;
using Utils.Caching;
using Entities.Domain.Users;

namespace Service.Departments
{
    public class AttendanceService : BaseService<AttendancePerDay>, IAttendanceService
    {
        protected override string PatternKey
        {
            get { return "PG.attendance."; }
        }

        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string ATTENDANCE_BY_TWO_DATE_KEY = "PG.attendance.bytwodate.{0}-{1}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string ATTENDANCE_BYDATE_AND_TYPEKEY = "PG.attendance.bydataandtype.{0}-{1}";

        
        /// <summary>
        /// 
        /// </summary>
        private const string USER_OF_ATTENDANCE_BY_ID_KEY = "PG.attendance.user.byid-{0}";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_NAME_OF_ATTENDANCE_BY_ID_KEY = "PG.attendance.user.name.byid-{0}";   

        /// <summary>
        /// 
        /// </summary>
       
        private const string USER_NAME_NOT_IN_ATTENDANCE_BY_ID_KEY = "PG.attendance.notin.user.name.byid-{0}";
         /// <summary>
        /// 
        /// </summary>
        private const string USER_ID_OF_ATTENDANCE_BY_ID_KEY = "PG.attendance.user.id.byid-{0}";
        
        #endregion

        private readonly IRepositoryAsync<AttendancePerDay> _attendanceRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<UserInAttendance> _userAttendanceRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IUserService _userService;
        //private readonly IUnitOfWorkAsync _unitOfWorkAsync;


        public AttendanceService(IRepositoryAsync<AttendancePerDay> repository, 
            ICacheManager cacheManager, 
            IRepositoryAsync<UserInAttendance> userAttendanceRepositoryAsync, 
            IRepositoryAsync<User> userRepositoryAsync,
            IUserService userService)
            : base(repository,cacheManager)
        {
            _attendanceRepositoryAsync = repository;
            _cacheManager = cacheManager;
            _userAttendanceRepositoryAsync = userAttendanceRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _userService = userService;
        }

        public Task<List<AttendancePerDay>> GetAttendancesByTwoDate(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddHours(23).AddMinutes(59);
            var key = string.Format(ATTENDANCE_BY_TWO_DATE_KEY, fromDate, toDate);
            return _attendanceRepositoryAsync.Table.Where(p => p.CreatedDate >= fromDate && p.CreatedDate <= toDate).ToListAsync();
        }

        public List<AttendancePerDay> GetAttendancesByDateAndType(DateTime datetime, AttendanceType? type)
        {
            var typestring = type != null ? type.ToString() : "null";
            var key = string.Format(ATTENDANCE_BYDATE_AND_TYPEKEY, datetime.ToShortDateString(), typestring);

            return _cacheManager.Get(key, () =>
            {
                var startDay = datetime.Date;
                var endDay = startDay.AddDays(1);
                var result = _attendanceRepositoryAsync.Table
                    .Where(p => p.CreatedDate <= endDay && p.CreatedDate >= startDay);
                if (type != null)
                    result = result.Where(p => p.Type == type);

                return result.ToList();
            });
        }

        public Task<List<UserInAttendance>> GetUserInAttendance(int attendanceId)
        {
            if (attendanceId <= 0)
                return null;
            var key = string.Format(USER_OF_ATTENDANCE_BY_ID_KEY, attendanceId);
            return _cacheManager.Get(key, () => _userAttendanceRepositoryAsync.Table.Where(p => p.AttendanceId == attendanceId && p.IsAttend).ToListAsync());
        }

        public List<string> GetUsernameInAttendance(int attendanceId)
        {
            if (attendanceId <= 0)
                return null;
            var key = string.Format(USER_NAME_OF_ATTENDANCE_BY_ID_KEY, attendanceId);
            return _cacheManager.Get(key, () =>
            {
                var listUserLine = _userAttendanceRepositoryAsync.Table
                    .Where(p => p.AttendanceId == attendanceId && p.IsAttend == true);
                return listUserLine.Select(userLine => userLine.User.Username).ToList();
            });
        }

        public List<string> GetUsernameNotInAttendance(int attendanceId)
        {
            if (attendanceId <= 0)
                return null;
            var key = string.Format(USER_NAME_NOT_IN_ATTENDANCE_BY_ID_KEY, attendanceId);
            return _cacheManager.Get(key, () =>
            {
                var listUserLine = _userAttendanceRepositoryAsync.Table
                    .Where(p => p.AttendanceId == attendanceId && p.IsAttend == false).ToList();
                return listUserLine.Select(userLine => userLine.User.Username).ToList();
            });
        }

        public List<int> GetUserIdInAttendance(int attendanceId)
        {
            if (attendanceId <= 0)
                return null;
            var key = string.Format(USER_ID_OF_ATTENDANCE_BY_ID_KEY, attendanceId);
            return _cacheManager.Get(key, () =>
            {

                return _userAttendanceRepositoryAsync.Table.Where(p => p.AttendanceId == attendanceId && p.IsAttend == true).Select(p => p.UserId).ToList();
               
            });
        }

        public Task CreateUserInAttendanceAsync(UserInAttendance userInAttendance)
        {
            if (userInAttendance == null)
                throw new ArgumentNullException("userInAttendance");

            _cacheManager.RemoveByPattern(PatternKey);

            return _userAttendanceRepositoryAsync.InsertAsync(userInAttendance);
        }

        public Task DeleteUserInAttendanceAsync(UserInAttendance userInAttendance)
        {
            if (userInAttendance == null)
                throw new ArgumentNullException("userInAttendance");

            _cacheManager.RemoveByPattern(PatternKey);

            return _userAttendanceRepositoryAsync.DeleteAsync(userInAttendance);
        }

        public Task DeletaAllUserInAttendance(int attendanceId)
        {
            _cacheManager.RemoveByPattern(PatternKey);

            var listLine = _userAttendanceRepositoryAsync.Table.Where(p => p.AttendanceId == attendanceId);
  
            return _userAttendanceRepositoryAsync.DeleteAsync(listLine);
        }
    }
}
