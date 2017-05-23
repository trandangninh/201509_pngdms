using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;


namespace Service.Interface
{
    public interface IMeetingResultService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MeetingResult> GetMeetingResultById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<MeetingResult>> GetAllMeetingResults();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<MeetingResult>> SearchMeetingResults(DateTime? searchDate, int? lineId, string userId = null);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingResult"></param>
        /// <returns></returns>
        Task CreateAsync(MeetingResult meetingResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingResult"></param>
        /// <returns></returns>
        Task UpdateAsync(MeetingResult meetingResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingResult"></param>
        /// <returns></returns>
        Task DeleteAsync(MeetingResult meetingResult);


        
    }
}
