using Entities.Domain.ScMeasures;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.ScMeasures
{
    public class MqsMeasureMap : EntityTypeConfiguration<MqsMeasure>
    {
        public MqsMeasureMap()
        {
            this.ToTable("MqsMeasure");
            this.HasKey(mm => mm.Id);
        }
    }
}
