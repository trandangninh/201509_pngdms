using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.QualityAlert
{
    public class NumberComplaintTrackingChartModel
    {
        public NumberComplaintTrackingChartModel()
        {
            Items = new List<ChartItemModel>();
        }
        public string SuplierName { get; set; }
        public string MonthList { get; set; }
        public List<ChartItemModel> Items { get; set; }
    }
}