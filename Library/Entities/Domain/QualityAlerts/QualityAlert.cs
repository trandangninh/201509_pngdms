using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.QualityAlerts
{
    public partial class QualityAlert : BaseEntity
    {
        public DateTime AlertDateTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int LineId { get; set; }
        public string Detail { get; set; }
        public string Action { get; set; }
        public string GCAS { get; set; }
        public string SAPLot { get; set; }
        public int? NumBlock { get; set; }
        public int OwnerId { get; set; }
        public string Machine { get; set; }
        public string FollowUpAction { get; set; }
        public DateTime When { get; set; }
        public string UserNameCreated { get; set; }
        public int QualityAlertStatusId { get; set; }
        public QualityAlertStatus QualityAlertStatus
        {
            get { return (QualityAlertStatus)QualityAlertStatusId;}
            set { QualityAlertStatusId = (int)value; }
        }
        public int ClassificationId { get; set; }

        public string SupplierLot { get; set; }
        public int SupplierId { get; set; }
        //public int MaterialId { get; set; }
        public string Material { get; set; }
        public string Unit { get; set; }
        public int CategoryId { get; set; }
        public int ComplaintTypeId { get; set; }
        public ComplaintType ComplaintType
        {
            get { return (ComplaintType)ComplaintTypeId; }
            set { ComplaintTypeId = (int)value; }
        }
        public int ClassificationDefectId { get; set; }
        public int DefectRepeatId { get; set; }
        public DefectRepeat DefectRepeat
        {
            get { return (DefectRepeat)DefectRepeatId; }
            set { DefectRepeatId = (int)value; }
        }
        public DateTime SupplierReplyDate { get; set; }
        //public double? CostImpacted { get; set; }
        public double? PRLossPercent { get; set; }
        public int? QuantityReturn { get; set; }
        public int? NumStop { get; set; }
        public string DownTime { get; set; }
        public int? DefectedQty { get; set; }
        public DateTime InformedToSupplierDate { get; set; }
        public int? PGerEffortLoss { get; set; }
        public int? ContractorEffortLoss { get; set; }
        public bool? QARelatedToMaterials { get; set; }
        public bool? QARelatedToFG { get; set; }
        public int? FoundByFunctionId { get; set; }  
        public int ClassificationRPN { get; set; }
    }

    public enum QualityAlertStatus
    {
        Open = 1,
        Closed = 2,
        Delayed = 3
    }

    public enum ComplaintType
    {
        Complaint = 1,
        Feedback = 2,
    }

    public enum DefectRepeat
    {
        Yes = 1,
        No = 2,
    }
}
