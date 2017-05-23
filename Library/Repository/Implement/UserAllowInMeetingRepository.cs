using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;

namespace Repository.Implement
{   

    public static class UserAllowInMeetingRepository
    {
        public static Task<List<UserAllowInMeeting>> GetUserAllowByMeetingTypeAsync(this IRepositoryAsync<UserAllowInMeeting> repository, MeetingType type)
        {
            return repository
                .Table
                .Where(x => x.MeetingTypeId == (int)type).ToListAsync();

        }
    }
}
