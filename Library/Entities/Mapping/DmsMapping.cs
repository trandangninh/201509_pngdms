using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class DmsMapping : EntityTypeConfiguration<Dms>
    {
        public DmsMapping()
        {
            this.ToTable("Dms");
            this.HasKey(d => d.Id);
            this.Ignore(d => d.DmsType);
            this.HasRequired(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DepartmentId);
        }
    }
}
