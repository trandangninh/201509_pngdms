using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class UserInSupplyChainMPSAMap : EntityTypeConfiguration<UserInSupplyChainMPSA>
    {
        public UserInSupplyChainMPSAMap()
        {
            this.ToTable("UserInSupplyChainMPSA");
            this.HasKey(m => m.Id);
        }
    }
}
