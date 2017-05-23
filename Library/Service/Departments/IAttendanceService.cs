using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Departments
{
    public interface IAttendanceService : IBaseService<AttendancePerDay>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<AttendancePerDay>> GetAttendancesByTwoDate(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<AttendancePerDay> GetAttendancesByDateAndType(DateTime datetime, AttendanceType? type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceId"></param>
        /// <returns></returns>
        Task<List<UserInAttendance>> GetUserInAttendance(int attendanceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceId"></param>
        /// <returns></returns>
        List<string> GetUsernameInAttendance(int attendanceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceId"></param>
        /// <returns></returns>
        List<string> GetUsernameNotInAttendance(int attendanceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceId"></param>
        /// <returns></returns>
        List<int> GetUserIdInAttendance(int attendanceId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInAttendance"></param>
        /// <returns></returns>
        Task CreateUserInAttendanceAsync(UserInAttendance userInAttendance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInAttendance"></param>
        /// <returns></returns>
        Task DeleteUserInAttendanceAsync(UserInAttendance userInAttendance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceId"></param>
        /// <returns></returns>
        Task DeletaAllUserInAttendance(int attendanceId);
    }
}
