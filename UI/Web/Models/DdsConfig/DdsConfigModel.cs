using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsConfig
{
    public class DdsConfigModel
    {
        public int Id { get; set; }
        public string Dms { get; set; }
        public int DepartmentId { get; set; }
        public int MeasureId { get; set; }

        public string Measure { get; set; }

        public string MeasureType { get; set; }

        public string Target { get; set; }

        public string Unit { get; set; }

        public string Owner { get; set; }

        public List<LineResultOfDdsConfig> Lines { get; set; }

        public DdsConfigModel()
        {
            Lines = new List<LineResultOfDdsConfig>();
        }

    }

    public class LineResultOfDdsConfig
    {
        public int Id { get; set; }
        public int DdsConfigId { get; set; }
        public string LineCode { get; set; }

        //public string Result { get; set; }

        public string LineName { get; set; }

        public bool ReadOnly { get; set; }

        //attribute to span columns
        public string Colspan { get; set; }
        public bool IsHiddenForSpanColumns { get; set; }
        //

    }
}