using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{

    public class ShutdownRequestMapping : EntityTypeConfiguration<ShutdownRequest>
    {

        public ShutdownRequestMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("ShutdownRequest");

            this.Ignore(s => s.ShutdownStatus);

            this.HasRequired(p => p.CreatedUser)
                .WithMany(o => o.ShutdownRequests)
                .HasForeignKey(p => p.UserCreatedId);

        }
    }
}
