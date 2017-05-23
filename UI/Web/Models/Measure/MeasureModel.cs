using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Measure
{
  
    public class MeasureModel
    {
        public int Id { get; set; }

        public string MeasureCode { get; set; }
        public string MeasureName { get; set; }

        public string Target { get; set; }

        public string Note { get; set; }

        public string Unit { get; set; }

        public List<string> ListUsername { get; set; }

        public MeasureModel()
        {
            ListUsername = new List<string>();
        }

        public DmsViewModel Dms { get; set; }
        public MeasureTypeViewModel MeasureType { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }

        public class DmsViewModel
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public DmsViewModel()
            {
                Id = 0;
                Code = "";
            }
        }
        public class MeasureTypeViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public MeasureTypeViewModel()
            {
                Id = 0;
                Name = "";
            }
            //public MeasureType(int id, string name)
            //{
            //    Id = id;
            //    Name = name;
            //}
        }
    }
   
}