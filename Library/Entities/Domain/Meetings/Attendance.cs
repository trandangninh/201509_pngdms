using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Dds;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain.Meetings
{
    public class Attendance : BaseEntity
    {
        public int DdsMeetingId { get; set; }
        public virtual DdsMeeting DdsMeeting { get; set; }
        public bool IsAttendance { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
