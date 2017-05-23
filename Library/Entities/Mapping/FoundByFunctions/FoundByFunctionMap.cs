using Entities.Domain.FoundByFunction;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.FoundByFunctions
{    
    public partial class FoundByFunctionMap : EntityTypeConfiguration<FoundByFunction>
    {
        public FoundByFunctionMap()
        {
            this.ToTable("FoundByFunction");
            this.HasKey(pa => pa.Id);
            
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}
