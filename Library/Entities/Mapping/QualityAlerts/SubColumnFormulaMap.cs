using Entities.Domain.QualityAlerts;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.QualityAlerts
{
    public class SubColumnFormulaMap : EntityTypeConfiguration<SubColumnFormula>
    {
        public SubColumnFormulaMap()
        {
            this.ToTable("SubColumnFormula");
            this.HasKey(x => x.Id);
        }
    }
}
