using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Dds
{
    public partial class DdsMeetingMap : EntityTypeConfiguration<DdsMeeting>
    {
        public DdsMeetingMap()
        {
            this.ToTable("DdsMeeting");

            this.HasKey(d => d.Id);

            this.HasRequired(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DepartmentId);
        }
    }
}
