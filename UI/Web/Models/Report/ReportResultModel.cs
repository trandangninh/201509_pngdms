using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Report
{
    public class ReportResultModel
    {
        public string Dms { get; set; }
        public int DmsId { get; set; }
        public string IPorOP { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public string Owner { get; set; }
        public string Target { get; set; }
        public string Unit { get; set; }
        public List<LineModelForReportView> Lines { get; set; }
        public ReportResultModel()
        {
            Lines = new List<LineModelForReportView>();
        }
        //public class LineInDayResultModel
        // {
        //    public List<LineModelForReportView> Lines { get; set; }
        //    public LineInDayResultModel()
        //    {
        //        Lines = new List<LineModelForReportView>();
        //    }
        // }
        public class LineModelForReportView
        {
            public DateTime Date { get; set; }
            public int LineId { get; set; }
            public string LineCode { get; set; }
            public string LineName { get; set; }
            public string Value { get; set; }
            public bool IsReadOnly { get; set; }
            public string Remark { get; set; }
            public string Colspan { get; set; }
            public bool IsHiddenForSpanColumns { get; set; }
            public bool IsLastLineOfDay { get; set; }
            public LineModelForReportView()
            {
                Value = "";
                IsReadOnly = true;
            }
        }
    }
}