using Entities.Domain.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Meetings
{
    public partial interface IUserInMeetingService : IBaseService<UserInMeeting>
    {
        List<UserInMeeting> GetAllUserInMeetingByDepartmentId(int departmentId);
    }
}
