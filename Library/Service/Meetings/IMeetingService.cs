using Entities.Domain.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Meetings
{
    public partial interface IMeetingService : IBaseService<Meeting>
    {
        Task<Meeting> GetMeetingByDepartmentId(int departmentId);
        /// <summary>
        /// Add meeting leader role to new leader and remove to old leader
        /// </summary>
        /// <param name="oldLeaderId">Old leader identify</param>
        /// <param name="newLeaderId">New leader identify</param>
        /// <returns></returns>
        Task UpdateMeetingLeader(int meetingId, int oldLeaderId, int newLeaderId);

        /// <summary>
        /// Get all meeting has department is active
        /// </summary>
        /// <param name="oldLeaderId">page index</param>
        /// <param name="newLeaderId">page size</param>
        /// <returns></returns>
        //IPagedList<Meeting> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check user is meeting leader or not by departmentId and userId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <returns>true or false</returns>
        bool CheckUserIsMeetingLeaderByDepartmentIdAndUserId(int departmentId, int userId);
    }
}
