using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.QualityAlert
{
    public class QualityAlertFillterModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int LineId { get; set; }
        public bool IsAdminLogin { get; set; }
        public List<int> SupplierIds { get; set; }
        public int CategoryId { get; set; }
        public int ComplaintTypeId { get; set; }
        public int ClassificationDefectId { get; set; }
        public int DefectRepeatId { get; set; }
        public string SupplierReplyDate { get; set; }
        public int DepartmentId { get; set; }
        public int StatusId { get; set; }
        public QualityAlertFillterModel()
        {
            SupplierIds = new List<int>();
        }
        public int FoundByFunctionId { get; set; }
    }
}