using Entities.Domain;
using Entities.Domain.Users;

namespace Service.Messages
{
    /// <summary>
    /// create queue email, we can use stringbuilder or load template from external file
    /// </summary>
    public interface IWorkFlowMessageService
    {

        /// <summary>
        /// send mail when report result meeting
        /// </summary>
        /// <param name="user"></param>
        /// <param name="department"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        QueuedEmail SendReportToUser(User user, string department, string fromDate, string toDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issues"></param>
        /// <returns></returns>
        QueuedEmail SendIssusToUser(User user,Issue issues);

        /// <summary>
        /// send mail to user create
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issues"></param>
        /// <param name="userUpdate"></param>
        /// <returns></returns>
        QueuedEmail SendIssusToOwner(User user, Issue issues, string userUpdate);

        /// <summary>
        /// send mail ateendance to owner
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issues"></param>
        /// <param name="userUpdate"></param>
        /// <returns></returns>
        //QueuedEmail SendAttendToOwner(User user, AttendancePerDay attendancePerDay);

        QueuedEmail SendWarningMark(string email, int Id, string createDate, int valueMark, int severityValue);
    }
}
