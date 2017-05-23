using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Common
{
    public interface ITrackingService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Tracking> GetTrackingById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Tracking>> GetAllTrackings();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<Tracking>> GetTrackingByDateAndLine(string lineId,DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<Tracking>> GetTrackingByTwoDateAndLine(string lineId, DateTime fromDate, DateTime toDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        Task<List<Tracking>> GetTrackingOfLine(string lineId); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task CreateAsync(Tracking tracking);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task UpdateAsync(Tracking tracking);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task DeleteAsync(Tracking tracking);
    }
}
