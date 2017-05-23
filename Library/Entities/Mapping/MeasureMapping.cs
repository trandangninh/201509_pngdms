using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class MeasureMapping : EntityTypeConfiguration<Measure>
    {
        public MeasureMapping()
        {
            this.ToTable("Measure");
            this.HasKey(m => m.Id);
            this.Ignore(m => m.MeasureType);
            this.Ignore(m => m.MeasureSystemType);
            this.HasRequired(m => m.Dms)
                .WithMany(d => d.Measures)
                .HasForeignKey(p => p.DmsId);
        }
    }
}
