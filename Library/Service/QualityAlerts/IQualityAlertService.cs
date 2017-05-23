using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Entities.Domain.QualityAlerts;

namespace Service.QualityAlerts
{
    public partial interface IQualityAlertService : IBaseService<QualityAlert>
    {
        Task DeleteQualityAlertAsync(List<int> listId);
        Task<IPagedList<QualityAlert>> SearchQualityAlertAsync(int? lineId = null, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<IPagedList<QualityAlertFullObject>> SearchQualityAlertObjectAsync(int departmentId = 0,
            int userId = 0, 
                                                                                int lineId = 0, 
                                                                                DateTime? startDate = null, 
                                                                                DateTime? endDate = null, 
                                                                                List<int> supplierIds = null, 
                                                                                int categoryId = 0, 
                                                                                int complaintTypeId = 0,
                                                                                int classificationDefectId = 0, 
                                                                                int defectRepeatId = 0, 
                                                                                DateTime? supplierReplayDate = null, 
                                                                                int statusId = 0, int foundByFunctionId = 0,
                                                                                int pageIndex = 0, int pageSize = int.MaxValue);
        int CountAllQualityAlertByClassificationIdAsync(int classificationId);
        int CountQuanlityAllertByLineAndDate(int lineId, DateTime date);
        /// <summary>
        /// Get data by classification defect on month for complaint tracking chart
        /// Tuple: month-data
        /// </summary>
        /// <returns></returns>
        Task<List<Tuple<int,string>>> QualityAlertComplaintTracking();

        List<QualityAlert> QualityAlertByClassificationId(int classificationId, DateTime? lastDate);
    }
}
