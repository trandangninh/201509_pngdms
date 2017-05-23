using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

using Entities.Domain.Users;

namespace Service.Interface
{
    public interface IUserAllowInMeetingService
    {
        Task<List<UserAllowInMeeting>> GetByMeetingType(MeetingType type);
        Task<List<User>> GetUserInMeetingType(MeetingType type);
        Task DeleteAllOfType(MeetingType type);

        Task AddUserToType(MeetingType type, User user);


    }
}
