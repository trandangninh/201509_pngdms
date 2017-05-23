using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.SupplyChain;

namespace Entities.Mapping.SupplyChain
{
    //public class SupplyChainDetailMap : EntityTypeConfiguration<SupplyChainDetail>
    //{
    //    public SupplyChainDetailMap()
    //    {
    //        this.ToTable("SupplyChainDetail");
    //        this.HasKey(d => d.Id);
    //        this.Ignore(d => d.SupplyChainOptionType);

    //        this.HasRequired(d => d.DdsMeeting)
    //            .WithMany(m => m.SupplyChainDetails)
    //            .HasForeignKey(d => d.DdsMeetingId);
    //    }
    //}
}
