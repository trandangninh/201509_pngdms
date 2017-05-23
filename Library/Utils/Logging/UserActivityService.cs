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
    public class UserActivityService: IUserActivityService
    {

        private readonly IRepositoryAsync<ActivityLog> _activityLogrepository;
        //private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public UserActivityService(IRepositoryAsync<ActivityLog> activityLogrepository)//, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _activityLogrepository = activityLogrepository;
            //_unitOfWorkAsync = unitOfWorkAsync;
        }


        public  Task InsertActivity(ActivityLogType activityLogType,
            string comment, User user)
        {
            var newActivity = new ActivityLog()
            {
                ActivityLogType = activityLogType,
                Comment = Helper.CommonHelper.ConvertEnum(activityLogType.ToString()) + "( " + comment + " )",
                //UserId = user.Id,
                CreatedOnUtc = DateTime.Now
            };

            _activityLogrepository.Insert(newActivity);
            //return _unitOfWorkAsync.SaveChangesAsync();
            return null;
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
