using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Activity;

using RepositoryPattern.Repositories;
using Entities.Domain.Users;

namespace Utils.Logging
{
    public class NullActivityService: IUserActivityService
    {
        public NullActivityService(IRepositoryAsync<ActivityLog> repository)
        {
        }


        public Task InsertActivity(ActivityLogType activityLogType,
            string comment, User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteActivity(ActivityLog activityLog)
        {
            throw new NotImplementedException();
        }

        public IList<ActivityLog> GetAllActivities(DateTime? createdOnFrom, DateTime? createdOnTo, int? userId, int activityLogTypeId)
        {
            throw new NotImplementedException();
        }

        public ActivityLog GetActivityById(int activityLogId)
        {
            throw new NotImplementedException();
        }

        public void ClearAllActivities()
        {
            throw new NotImplementedException();
        }
    }
}
