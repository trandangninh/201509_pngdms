using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    class UserAllowInSupplyChainMapping: EntityTypeConfiguration<UserAllowInSupplyChain>
    {
        public UserAllowInSupplyChainMapping()
        {
            this.ToTable("UserAllowInSupplyChain");
            this.HasKey(t => t.Id);
            this.Ignore(us => us.DmsType);

            this.HasRequired(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId);
        }
    }
}
