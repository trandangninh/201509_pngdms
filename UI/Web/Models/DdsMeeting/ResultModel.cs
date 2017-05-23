using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsMeeting
{
    public class ResultModel
    {
        public string Dms { get; set; }
        public string IPorOP { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public string Owner { get; set; }
        public string Target { get; set; }
        public string Unit { get; set; }
        public string MeasureSystemType { get; set; }
        public bool Readonly { get; set; }
        public bool IsGuest { get; set; } 
        public List<LineModelForDdsMeetingView> Lines { get; set; }
        public ResultModel()
        {
            Lines = new List<LineModelForDdsMeetingView>();
        }
        public class LineModelForDdsMeetingView
        {
            public int LineId { get; set; }
            public string LineCode { get; set; }
            public string LineName { get; set; }
            public string Value { get; set; }
            public bool IsReadOnly { get; set; }
            public string Remark { get; set; }

            //attribute to span columns
            public string Colspan { get; set; }
            public bool IsHiddenForSpanColumns { get; set; }
            //

            public LineModelForDdsMeetingView()
            {
                Value = "";
                IsReadOnly = true;
            }
        }
    }
}