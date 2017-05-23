using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Dds;

namespace Entities.Mapping.Dds
{
    public class DdsMeetingPrDetailMap : EntityTypeConfiguration<DdsMeetingPrDetail>
    {
        public DdsMeetingPrDetailMap()
        {
            this.ToTable("DdsMeetingPrDetail");
            this.HasKey(d => d.Id);

            this.HasRequired(d => d.DdsMeetingResult)
                .WithMany()
                .HasForeignKey(d => d.DdsMeetingResultId);
        }
    }
}
