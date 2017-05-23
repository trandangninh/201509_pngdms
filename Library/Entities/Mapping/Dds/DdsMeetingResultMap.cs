using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Dds
{
    public partial class DdsMeetingResultMap : EntityTypeConfiguration<DdsMeetingResult>
    {
        public DdsMeetingResultMap()
        {
            this.ToTable("DdsMeetingResult");

            this.HasKey(d => d.Id);

            this.HasRequired(d => d.DdsMeeting)
                .WithMany(m=>m.DdsMeetingResults)
                .HasForeignKey(d => d.DdsMeetingId);

            this.HasRequired(d => d.Measure)
                .WithMany()
                .HasForeignKey(d => d.MeasureId);

            this.HasRequired(d => d.Line)
                .WithMany()
                .HasForeignKey(d => d.LineId);
        }
    }
}
