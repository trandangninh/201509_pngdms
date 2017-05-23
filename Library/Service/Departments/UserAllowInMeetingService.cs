using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

using RepositoryPattern.Repositories;
using Service.Interface;
using Entities.Domain.Users;

namespace Service.Implement
{
    public class UserAllowInMeetingService : IUserAllowInMeetingService
    {
        private readonly IRepositoryAsync<UserAllowInMeeting> _userAllowRepositoryAsync;

        public UserAllowInMeetingService(IRepositoryAsync<UserAllowInMeeting> repository)
        {
            _userAllowRepositoryAsync = repository;
        }

        public Task<List<UserAllowInMeeting>> GetByMeetingType(MeetingType type)
        {
            return _userAllowRepositoryAsync.GetUserAllowByMeetingTypeAsync(type);
        }

        public async Task<List<User>> GetUserInMeetingType(MeetingType type)
        {
            return await _userAllowRepositoryAsync.Table.Where(u=>u.MeetingTypeId == (int)type).Select(u=>u.User).ToListAsync();
        }

        public  Task DeleteAllOfType(MeetingType type)
        {
            var allRecords = _userAllowRepositoryAsync.Table.Where(p=>p.MeetingTypeId==(int)type);

            return _userAllowRepositoryAsync.DeleteAsync(allRecords);
        }

        public Task AddUserToType(MeetingType type, User user)
        {
            return _userAllowRepositoryAsync.InsertAsync(new UserAllowInMeeting()
            {
                MeetingType = type,
                UserId = user.Id
            });
        }
    }
}
