using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Supplychain
{
    public class FpqModel
    {
        public FpqModel()
        {
            LineNames = new List<string>();
            Measures = new List<FpqMeasureModel>();
        }
        public string Target { get; set; }
        public List<string> LineNames { get; set; }
        public List<FpqMeasureModel> Measures { get; set; }

        public class FpqMeasureModel
        {
            public FpqMeasureModel()
            {
                Lines = new List<FpqLineModel>();
                ListUsername = new List<string>();
            }

            public string Name { get; set; }
            public string Id { get; set; }
            public string Remark { get; set; }
            public List<string> ListUsername { get; set; }
            public List<FpqLineModel> Lines { get; set; }
        }

        public class FpqLineModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}