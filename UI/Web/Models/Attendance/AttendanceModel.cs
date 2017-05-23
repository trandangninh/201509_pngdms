using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Attendance
{
    public class AttendanceModel
    {
        public int Id { get; set; }

        public string DateString { get; set; }

        public String UsersInAttendance { get; set; }

        public String UsersNotInAttendance { get; set; }

        public string Department { get; set; }
    }
}