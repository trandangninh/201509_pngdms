using Entities.Domain.Suppliers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Suppliers
{
    public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            this.ToTable("Supplier");
            this.HasKey(s => s.Id);

            this.Ignore(s => s.LocationType);

            this.HasMany(s => s.ClassificationDefects)
            .WithMany(c => c.Suppliers)
            .Map(m => m.ToTable("ClassificationDefect_Supplier_Mapping"));
        }
    }
}
