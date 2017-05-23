using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Dds
{
    public partial class DdsConfigMap : EntityTypeConfiguration<DdsConfig>
    {
        public DdsConfigMap()
        {
            this.ToTable("DdsConfig");
            this.HasKey(d => d.Id);


            //this.HasRequired(d => d.Department)
            //    .WithMany()
            //    .HasForeignKey(d => d.DepartmentId);

            //this.HasRequired(d => d.Dms)
            //    .WithMany()
            //    .HasForeignKey(d => d.DmsId);

            this.HasRequired(d => d.Measure)
                .WithMany()
                .HasForeignKey(d => d.MeasureId);

            this.HasRequired(d => d.Line)
                .WithMany()
                .HasForeignKey(d => d.LineId);
        }
    }
}
