using Entities.Domain.QualityAlerts;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.QualityAlerts
{
    public partial class QualityAlertMap : EntityTypeConfiguration<QualityAlert>
    {
        public QualityAlertMap()
        {
            this.ToTable("QualityAlert");
            this.HasKey(pa => pa.Id);
            this.Ignore(qa => qa.QualityAlertStatus);

            //this.Property(pa => pa.LineId).IsRequired();
            this.Property(pa => pa.AlertDateTime).IsRequired();
            this.Property(pa => pa.Detail).IsRequired();
            //this.Property(pa => pa.Action).IsRequired();
            this.Property(pa => pa.GCAS);
            //this.Property(pa => pa.SAPLot).IsRequired();
            //this.Property(pa => pa.SupplierLot).IsRequired();
            //this.Property(pa => pa.NumBlock).IsRequired();
            this.Property(pa => pa.DefectedQty);

            this.Ignore(qa => qa.ComplaintType);
            this.Ignore(qa => qa.DefectRepeat);
        }
    }
}
