using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Supplychain
{
    public class MpsaModel
    {
        public MpsaModel()
        {
            LineNames = new List<string>();
            FrLineNames = new List<string>();
            Measures = new List<MpsaMeasureModel>();
        }
        public string Target { get; set; }
        public string Remark { get; set; }
        public List<string> LineNames { get; set; }
        public List<string> FrLineNames { get; set; }
        public List<MpsaMeasureModel> FrMeasures { get; set; }
        public List<MpsaMeasureModel> Measures { get; set; }
        public class MpsaMeasureModel
        {
            public MpsaMeasureModel()
            {
                Lines = new List<MpsaLineModel>();
                ListUsername = new List<string>();
            }

            public string Name { get; set; }
            public string Id { get; set; }
            public string Remark { get; set; }
            public List<string> ListUsername { get; set; }
            public List<MpsaLineModel> Lines { get; set; }
        }

        public class MpsaLineModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}