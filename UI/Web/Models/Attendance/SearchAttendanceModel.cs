using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models.Attendance
{
    public class SearchAttendanceModel
    {
        public SearchAttendanceModel()
        {
            AvailableDepartments = new List<SelectListItem>();
        }
        public int DepartmentId { get; set; }
        public IList<SelectListItem> AvailableDepartments { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}