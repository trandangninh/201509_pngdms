using System;
using Entities.Domain.Departments;
using Entities.Domain.Meetings;
using RepositoryPattern.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;
using Service.Users;
using Entities.Domain.Users;
using Utils;

namespace Service.Meetings
{
    public partial class MeetingService : BaseService<Meeting>, IMeetingService
    {
        protected override string PatternKey
        {
            get { return "PG.Meeting."; }
        }

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : department Id
        /// </remarks>
        private const string MEETING_BY_DEPARTMENT_KEY = "PG.Meeting.ByDepartment-{0}";
        #region constant for cache

        #endregion

        private readonly IRepositoryAsync<Meeting> _meetingRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;

        public MeetingService(IRepositoryAsync<Meeting> meetingRepositoryAsync,
            ICacheManager cacheManager,
            IUserService userService, 
            IRepositoryAsync<Department> departmentRepositoryAsync)
            : base(meetingRepositoryAsync, cacheManager)
        {
            this._meetingRepositoryAsync = meetingRepositoryAsync;
            this._cacheManager = cacheManager;
            this._userService = userService;
            this._departmentRepositoryAsync = departmentRepositoryAsync;
        }

        public override Task<IPagedList<Meeting>> GetAllAsync(int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from m in _meetingRepositoryAsync.Table
                join d in _departmentRepositoryAsync.Table on m.DepartmentId equals d.Id
                where d.Active
                select m;
            return Task.FromResult(new PagedList<Meeting>(query.OrderBy(m=>m.DepartmentId), pageIndex, pageSize) as IPagedList<Meeting>);
        }

        public Task<Meeting> GetMeetingByDepartmentId(int departmentId)
        {
            if (departmentId <= 0)
                return null;

            var key = string.Format(MEETING_BY_DEPARTMENT_KEY, departmentId);
            return _cacheManager.Get(key, () => _meetingRepositoryAsync.Table.FirstOrDefaultAsync(m => m.DepartmentId == departmentId));
        }

        public async Task UpdateMeetingLeader(int meetingId, int oldLeaderId, int newLeaderId)
        {
            if (oldLeaderId == newLeaderId)
                return;
            var userLeaderRole = await _userService.GetUserRoleBySystemName(SystemUserRoleNames.MeetingLeaders);
            var existLastLeader = _meetingRepositoryAsync.Table.FirstOrDefault(m => m.CurrentLeaderId == oldLeaderId && m.Id != meetingId);
            if (existLastLeader == null)
            {
                //remove role of last Current Leader  
                var lastUserLeader = _userService.GetUserByIdAsync(oldLeaderId);
                if (lastUserLeader != null)
                {
                    lastUserLeader.Result.UserRoles.Remove(userLeaderRole);
                    await _userService.UpdateAsync(lastUserLeader.Result);
                }
            }

            //not set current leader
            if (newLeaderId == 0)
                return;

            var existNewLeader = _meetingRepositoryAsync.Table.FirstOrDefault(m => m.CurrentLeaderId == newLeaderId && m.Id != meetingId);
            if (existNewLeader == null)
            {
                //add role
                var newCurrentLeader = _userService.GetUserByIdAsync(newLeaderId);
                newCurrentLeader.Result.UserRoles.Add(userLeaderRole);
                await _userService.UpdateAsync(newCurrentLeader.Result);
            }
        }

        /// <summary>
        /// check user is meeting leader or not by departmentId and userId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <returns>true or false</returns>
        public bool CheckUserIsMeetingLeaderByDepartmentIdAndUserId(int departmentId, int userId)
        {
            if (departmentId <= 0 || userId <= 0)
                return false;

            return _meetingRepositoryAsync.Table.FirstOrDefault(m => m.DepartmentId == departmentId).UserInMeetings.Where(u => u.IsLeader).Any(u => u.UserId == userId);
        }
    }
}
