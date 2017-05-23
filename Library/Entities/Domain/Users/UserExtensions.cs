using Entities.Domain.Meetings;
using RepositoryPattern.Infrastructure;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Users
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this User user)
        {
            return user.IsInUserRole(SystemUserRoleNames.Administrators);
        }

        public static bool IsEmployee(this User user)
        {
            return user.IsInUserRole(SystemUserRoleNames.Employees);
        }
        public static bool IsLeader(this User user)
        {
            var userInMeeting = EngineContext.Current.Resolve<IRepositoryAsync<UserInMeeting>>();

            foreach(var d in user.Departments)
            {
                //if (userInMeeting.Table.Where(u => u.Meeting.DepartmentId == d.Id).All(u => u.Meeting.CurrentLeaderId == 0) && userInMeeting.Table.Where(u => u.Meeting.DepartmentId == d.Id).Any(u=>u.UserId == user.Id))
                if (userInMeeting.Table.Where(u => u.Meeting.DepartmentId == d.Id).Any(u => u.UserId == user.Id && u.IsLeader))
                    return true;
            }
            return user.IsInUserRole(SystemUserRoleNames.MeetingLeaders);
        }
        public static bool IsLeader(this User user, int departmentId)
        {
            var userInMeeting = EngineContext.Current.Resolve<IRepositoryAsync<UserInMeeting>>();
            //if (userInMeeting.Table.Where(u => u.Meeting.DepartmentId == d.Id).All(u => u.Meeting.CurrentLeaderId == 0) && userInMeeting.Table.Where(u => u.Meeting.DepartmentId == d.Id).Any(u=>u.UserId == user.Id))
            return userInMeeting.Table.Where(u => u.Meeting.DepartmentId == departmentId).Any(u => u.UserId == user.Id && u.IsLeader);
        }
        public static bool IsInUserRole(this User user, string systemUserRoleName)
        {
            if (user == null)
                return false;
            if (string.IsNullOrEmpty(systemUserRoleName))
                return false;

            return user.UserRoles.FirstOrDefault(ur => ur.SystemName == systemUserRoleName) != null;
        }
    }
}
