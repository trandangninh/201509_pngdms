using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Activity;

namespace Entities.Mapping.Activity
{

    public class ActivityLogMapping : EntityTypeConfiguration<ActivityLog>
    {

        public ActivityLogMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("ActivityLog");
            this.HasRequired(p => p.User).WithMany(o => o.ActivityLogs).HasForeignKey(p => p.UserId);
        }
    }
}
