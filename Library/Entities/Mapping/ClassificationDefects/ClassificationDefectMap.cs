using Entities.Domain.ClassificationDefects;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.ClassificationDefects
{
    public class ClassificationDefectMap : EntityTypeConfiguration<ClassificationDefect>
    {
        public ClassificationDefectMap()
        {
            this.ToTable("ClassificationDefect");
            this.HasKey(cd => cd.Id);

            this.HasMany(c => c.Suppliers)
                .WithMany(s => s.ClassificationDefects)
                .Map(m => m.ToTable("ClassificationDefect_Supplier_Mapping"));
        }
    }
}
