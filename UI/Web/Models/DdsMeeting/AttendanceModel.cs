using Entities.Domain.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Packing;

namespace Web.Models.DdsMeeting
{
    public class AttendanceModel
    {
        public int DepartmentId { get; set; }
        public DateTime Date { get; set; }
        public List<UserAttendanceModel> UserAttendanceModels { get; set; }
        public bool CanWriteAttendance { get; set; }
        
        public class UserAttendanceModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsAttendance { get; set; }
        }
    }
}