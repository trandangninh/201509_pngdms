using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Activity;

using Entities.Domain.Users;

namespace Utils.Logging
{
    public interface IUserActivityService
    {

        #region CRUD


        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="activityLogType">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="user">The User</param>
        /// <returns>Activity log item</returns>
        Task InsertActivity(ActivityLogType activityLogType,
            string comment, User user);

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log</param>
        Task DeleteActivity(ActivityLog activityLog);

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all Users</param>
        /// <param name="createdOnTo">Log item creation to; null to load all Users</param>
        /// <param name="userId">User identifier; null to load all Users</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log collection</returns>
        IList<ActivityLog> GetAllActivities(DateTime? createdOnFrom,
            DateTime? createdOnTo, int? userId,
            int activityLogTypeId);

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        ActivityLog GetActivityById(int activityLogId);

        /// <summary>
        /// Clears activity log
        /// </summary>
        void ClearAllActivities();

        #endregion


        #region active for user

        #endregion
    }
}
