using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class UserInSupplyChainFpqMap : EntityTypeConfiguration<UserInSupplyChainFpq>
    {
        public UserInSupplyChainFpqMap()
        {
            this.ToTable("UserInSupplyChainFpq");
            this.HasKey(us => us.Id);

            this.HasRequired(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId);
        }
    }
}
