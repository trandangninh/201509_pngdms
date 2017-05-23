using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Report
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            Lines = new List<ResultLineModel>();
            GroupLines = new List<GroupLine>();
        }

        public List<ResultLineModel> Lines { get; set; }
        public List<GroupLine> GroupLines { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public class ResultLineModel
        {
            public string Name { get; set; }
            public string Field { get; set; }
            public int LineId { get; set; }
            public string IsReadOnly { get; set; }
            public string Remark { get; set; }
            public int DayIndex { get; set; }
            public string Colspan { get; set; }
            public string IsHiddenForSpanColumns { get; set; }
            public string IsLastLineOfDay { get; set; }
        }

        public class GroupLine
        {
            public int DayIndex { get; set; }
            public string Date { get; set; }
        }
    }
}


//public class ReportViewModel
//{
//    public ReportViewModel()
//    {
//        LinesInDay = new List<LineInDayModel>();
//    }

//    public List<LineInDayModel> LinesInDay { get; set; }
//    public int DepartmentId { get; set; }
//    public string DepartmentName { get; set; }
//    public DateTime FromDate { get; set; }
//    public DateTime ToDate { get; set; }

//    public class ResultLineModel
//    {
//        public string Name { get; set; }
//        public string Field { get; set; }
//        public int LineId { get; set; }
//        public string IsReadOnly { get; set; }
//        public string Remark { get; set; }
//    }

//    public class LineInDayModel
//    {
//        public List<ResultLineModel> Lines { get; set; }
//        public LineInDayModel()
//        {
//            Lines = new List<ResultLineModel>();
//        }
//    }
//}