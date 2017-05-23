using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;
using Utils;

namespace Service.Common
{
    public interface IShutdownRequestService : IBaseService<ShutdownRequest>
    {
        /// <summary>
        /// Search Shutdown request
        /// </summary>
        /// <param name="keyword">Keyword to search</param>
        /// <param name="userId">User identify</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns></returns>
        IPagedList<ShutdownRequest> GetShutdownRequest(string keyword = "", int userId = 0, int statusId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
