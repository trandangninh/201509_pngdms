using Entities.Domain.Frequencys;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Frequencys
{
    public class FrequencyMap : EntityTypeConfiguration<Frequency>
    {
        public FrequencyMap()
        {
            this.ToTable("Frequency");
            this.HasKey(f => f.Id);            
        }
    }
}
