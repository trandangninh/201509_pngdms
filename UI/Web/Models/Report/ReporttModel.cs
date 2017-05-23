using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Web.Models.Report
{

    public class MeetingReportModel
    {

        public string DmsCode { get; set; }

        public string MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string MeasureType { get; set; }

        public string Target { get; set; }

        public string Unit { get; set; }

        public string Owner { get; set; }

        public List<LineResultReportModel> ListResult { get; set; }

        public MeetingReportModel()
        {
            ListResult = new List<LineResultReportModel>();
        }

    }

    public class LineResultReportModel
    {
        public DateTime DateTimeCreate { get; set; }

        public string LineCode { get; set; }

        public string Result { get; set; }

        public string LineName { get; set; }

    }


}