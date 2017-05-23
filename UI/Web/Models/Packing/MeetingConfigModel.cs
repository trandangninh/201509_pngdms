using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Web.Models.Packing
{

    public class MeetingConfigModel
    {

        public string DmsCode { get; set; }

        public string DmsName { get; set; }

        public string MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string MeasureType { get; set; }

        public string Target { get; set; }

        public string Unit { get; set; }

        public string Owner { get; set; }

        public List<LineResult> ListResult { get; set; }

        public MeetingConfigModel()
        {
            ListResult = new List<LineResult>();
        }

    }
    

  

}