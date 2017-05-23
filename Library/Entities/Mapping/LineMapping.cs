using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class LineMapping : EntityTypeConfiguration<Line>
    {
        public LineMapping()
        {
            this.ToTable("Line");
            this.HasKey(t => t.Id);
            

            this.HasMany(l => l.Users)
                .WithMany(u => u.Lines)
                .Map(m => m.ToTable("Line_User_Mapping"));

            this.HasRequired(l => l.Department)
                .WithMany(d => d.Lines)
                .HasForeignKey(l => l.DepartmentId);
        }
    }
}
