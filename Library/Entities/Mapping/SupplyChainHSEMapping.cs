using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    class SupplyChainHSEMapping: EntityTypeConfiguration<SupplyChainHSE>
    {

        public SupplyChainHSEMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("SupplyChainHSE");
            //this.HasRequired(p => p.CreatedUser).WithMany(o => o.SupplyChainHSEs).HasForeignKey(p => p.UserCreatedId);


        }
    }
}
