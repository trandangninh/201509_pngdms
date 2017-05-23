using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Web.Models.Packing
{

    public class MeetingResultModel
    {

        public string DmsCode { get; set; }

        public string DmsName { get; set; }

        public string MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string MeasureType { get; set; }

        public string Target { get; set; }

        public string Unit { get; set; }

        public string Owner { get; set; }

        public List<LineResult> ListResult { get; set; }

        public MeetingResultModel()
        {
            ListResult = new List<LineResult>();
        }

    }

    public class LineResult
    {
        public string LineCode { get; set; }

        public string Result { get; set; }

        public string LineName { get; set; }

        public string Remark { get; set; }

        public bool ReadOnly { get; set; }


        public string ColorClassCss { get; set; }

        public string ReadOnlyClassCss { get; set; }

    }



    public class IssueModel
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public string Content { get; set; }

        public string NextStep { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string UserCreated { get; set; }

        public int UserAssignedId { get; set; }

        public string UserAssigned { get; set; }

        public string Status { get; set; }
        public int StatusId { get; set; }

        public string Type { get; set; }

        public string TypeId { get; set; }

        public string date { get; set; }

        public string ActionPlan { get; set; }

        public string When { get; set; }

        public string Note { get; set; }

        public string IssuesPriority { get; set; }

        public string WhenDue { get; set; }

        public string SystemFixDMSLinkage { get; set; }
    }



    public class MeetModel
    {
        public List<MeetingResultModel> ListMeetingResultModel { get; set; }

        public List<IssueModel> ListIssueModel { get; set; }

        public AttendanceModel AttendanceModel { get; set; }

        public Boolean permissionIssue { get; set; }

        public Boolean permissionAttendance { get; set; }

        public Boolean permissionTracking { get; set; }

        public DateTime date { get; set; }
    }


    public class AttendanceModel
    {
        public int Id { get; set; }

        public string DateString { get; set; }

        public String UsersInAttendance { get; set; }

        public String UsersNotInAttendance { get; set; }

        public string Department { get; set; }
    }

    public class SearchAttendedModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Type { get; set; }

    }
    public class TrackingModel
    {
        public int Id { get; set; }
      
        public int UserCreated { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string LineCode { get; set; }

        public string FGCode { get; set; }

        public string Variant { get; set; }

        public string Size { get; set; }

        public string Lot { get; set; }

        public string Cause { get; set; }

        public string Where { get; set; }

        public int Quantity { get; set; }

        public int TotalQuantity { get; set; }
    }

}