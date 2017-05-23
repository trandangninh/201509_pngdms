using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.QualityAlert
{
    public class ComplaintTrackingChartModel
    {
        public ComplaintTrackingChartModel()
        {
            Items = new List<ChartItemModel>();
        }
        public string SuplierName { get; set; }
        public string ClassificationDefectNameList { get; set; }
        public List<ChartItemModel> Items { get; set; }
    }
    public class ChartItemModel
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}