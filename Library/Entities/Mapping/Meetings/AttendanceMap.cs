using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Meetings;

namespace Entities.Mapping.Meetings
{
    public class AttendanceMap : EntityTypeConfiguration<Attendance>
    {
        public AttendanceMap()
        {
            this.ToTable("Attendance");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.DdsMeeting)
                .WithMany(m => m.Attendances)
                .HasForeignKey(a => a.DdsMeetingId);

            this.HasRequired(a=>a.User)
                .WithMany()
                .HasForeignKey(a=>a.UserId)
                .WillCascadeOnDelete();
        }
    }
}
