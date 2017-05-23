using Entities.Domain.Meetings;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Meetings
{
    public partial class UserInMeetingMap : EntityTypeConfiguration<UserInMeeting>
    {
        public UserInMeetingMap()
        {
            this.ToTable("UserInMeeting");
            this.HasKey(m => m.Id);
            //this.Property(m => m.MeetingId).IsRequired();
            //this.Property(m => m.UserId).IsRequired();

            this.HasRequired(um => um.Meeting)
                .WithMany(m => m.UserInMeetings)
                .HasForeignKey(um => um.MeetingId);

            this.HasRequired(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);
        }
    }
}
