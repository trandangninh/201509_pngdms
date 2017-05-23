using Entities.Domain.Meetings;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;

namespace Service.Meetings
{
    public partial class UserInMeetingService : BaseService<UserInMeeting>, IUserInMeetingService
    {
        protected override string PatternKey
        {
            get { return "PG.UserInMeeting."; }
        }
        #region constant for cache

        #endregion

        private readonly IRepositoryAsync<UserInMeeting> _userInMeetingRepositoryAsync;
        private readonly ICacheManager _cacheManager;

        public UserInMeetingService(IRepositoryAsync<UserInMeeting> userInMeetingRepositoryAsync,
            ICacheManager cacheManager)
            : base(userInMeetingRepositoryAsync, cacheManager)
        {
            this._userInMeetingRepositoryAsync = userInMeetingRepositoryAsync;
            this._cacheManager = cacheManager;
        }
        public override Task<IPagedList<UserInMeeting>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PatternKey + "all");

            return _cacheManager.Get(key, () =>
            {
                var query = _userInMeetingRepositoryAsync.Table.OrderBy(x => x.Order);
                return Task.FromResult(new PagedList<UserInMeeting>(query, pageIndex, pageSize) as IPagedList<UserInMeeting>);
            }
            );
        }

        public List<UserInMeeting> GetAllUserInMeetingByDepartmentId(int departmentId)
        {
            return _userInMeetingRepositoryAsync.Table.Where(u => u.Meeting.DepartmentId == departmentId).ToList();
        }
    }
}
