using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class MeetingReport
    {

        public string DmsDecription { get; set; }
        public int DmsId { get; set; }
        public string MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string MeasureType { get; set; }

        public string Target { get; set; }

        public string Unit { get; set; }

        public string Owner { get; set; }

        public List<LineResultReport> ListResult { get; set; }

        public MeetingReport()
        {
            ListResult = new List<LineResultReport>();
        }

    }

    public class LineResultReport
    {
        public DateTime DateTimeCreate { get; set; }

        public string LineCode { get; set; }

        public string Result { get; set; }

        public string LineName { get; set; }
        public bool IsLastLineOfDay { get; set; }

    }

    public class QualityAlertReport
    {
        public string AlertDateTime { get; set; }
        public string UserName { get; set; }
        public string LineName { get; set; }
        public string Machine { get; set; }
        public string Detail { get; set; }
        public string Action { get; set; }
        public string GCAS { get; set; }
        public string SAPLot { get; set; }
        public int NumBlock { get; set; }
        public string Owner { get; set; }
        public string FollowUpAction { get; set; }
        public string When { get; set; }
        public string Status { get; set; }
        public string Classification { get; set; }

        public string SupplierLot { get; set; }
        public string Supplier { get; set; }
        public string Material { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; }
        public string ComplaintType { get; set; }
        public string ClassificationDefect { get; set; }
        public string DefectRepeat { get; set; }
        public string SupplierReplyDate { get; set; }
        public double CostImpacted { get; set; }
        public double PRLossPercent { get; set; }
        public int Quantity { get; set; }
        public int? NumStop { get; set; }
        public string DownTime { get; set; }
    }
}
