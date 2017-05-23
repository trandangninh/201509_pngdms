using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    class SupplyChainMPSAMapping: EntityTypeConfiguration<SupplyChainMPSA>
    {

        public SupplyChainMPSAMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("SupplyChainMPSA");

        }
    }
}
