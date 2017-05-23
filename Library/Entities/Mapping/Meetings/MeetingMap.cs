using Entities.Domain.Meetings;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Meetings
{
    public partial class MeetingMap : EntityTypeConfiguration<Meeting>
    {
        public MeetingMap()
        {
            this.ToTable("Meeting");
            this.HasKey(m => m.Id);
            //this.Property(m => m.DepartmentId).IsRequired();
            //this.Property(m => m.CurrentLeaderId).IsRequired();

            this.HasRequired(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DepartmentId);

            //this.HasRequired(m => m.CurrentLeader)
            //    .WithMany()
            //    .HasForeignKey(m => m.CurrentLeaderId);
        }
    }
}
