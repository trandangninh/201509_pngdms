using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{

    public class MeasureSupplyChainMapping : EntityTypeConfiguration<MeasureSupplyChain>
    {

        public MeasureSupplyChainMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("MeasureSupplyChain");
            
        }
    }
}
