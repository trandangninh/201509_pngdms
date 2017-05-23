using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsMeeting
{
    public class DdsMeetingModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Date { get; set; }
        public int LineId { get; set; }
    }
}