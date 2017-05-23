using Entities.Domain.ScMeasures;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.ScMeasures
{
    public class ScMeasureMap : EntityTypeConfiguration<ScMeasure>
    {
        public ScMeasureMap()
        {
            this.ToTable("ScMeasure");
            this.HasKey(sm => sm.Id);
            this.Ignore(sm => sm.YtdCalculatedType);
        }
    }
}
