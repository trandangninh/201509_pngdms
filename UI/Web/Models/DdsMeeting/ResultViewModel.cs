using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsMeeting
{
    public class ResultViewModel
    {
        public ResultViewModel()
        {
            Lines = new List<ResultLineModel>();
        }

        public List<ResultLineModel> Lines { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Date { get; set; }
        public int LineId { get; set; }

        public class ResultLineModel
        {
            public string Name { get; set; }
            public string Field { get; set; }
            public int LineId { get; set; }
            public string IsReadOnly { get; set; }
            public string Remark { get; set; }

            //remark of column header marking
            public string LineRemark { get; set; }

            //attribute to span columns
            public string Colspan { get; set; }
            public string IsHiddenForSpanColumns { get; set; }
            //
        }
    }
}