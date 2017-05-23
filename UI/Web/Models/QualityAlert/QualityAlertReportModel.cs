using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.QualityAlert
{
    public class QualityAlertReportModel
    {
        public QualityAlertReportModel()
        {
            Items = new List<ChartItemModel>();
        }
        //public string MonthList { get; set; }
        public string SuplierList { get; set; }
        public List<ChartItemModel> Items { get; set; }
        public string MonthList { get; set; }
    }
}