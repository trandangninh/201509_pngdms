using Entities.Domain.Suppliers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Suppliers
{
    public class ScMeasureTargetMap : EntityTypeConfiguration<ScMeasureTarget>
    {
        public ScMeasureTargetMap()
        {
            this.ToTable("ScMeasureTarget");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Supplier)
                .WithMany(p => p.ScMeasureTargets)
                .HasForeignKey(x => x.SupplierId);

            this.HasRequired(x => x.ScMeasure)
                .WithMany(l => l.ScMeasureTargets)
                .HasForeignKey(d => d.ScMeasureId);
        }
    }
}
