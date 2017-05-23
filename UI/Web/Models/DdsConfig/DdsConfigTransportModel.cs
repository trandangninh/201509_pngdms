using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsConfig
{
    public class DdsConfigTransportModel
    {
        public DdsConfigTransportModel()
        {
            Lines = new List<ResultLineModelForDdsConfig>();
        }

        public List<ResultLineModelForDdsConfig> Lines { get; set; }
        public int DepartmentId { get; set; }

        public class ResultLineModelForDdsConfig
        {
            public string Name { get; set; }
            public int LineId { get; set; }
            public string Field { get; set; }
            public string Template { get; set; }
            public string View { get; set; }

            //attribute to span columns
            public string Colspan { get; set; }
            public string IsHiddenForSpanColumns { get; set; }
            //
        }
    }
}