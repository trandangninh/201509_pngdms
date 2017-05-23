using Entities.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Mapping
{
    public class LineRemarkMapping : EntityTypeConfiguration<LineRemark>
    {
        public LineRemarkMapping()
        {
            this.ToTable("LineRemark");
            this.HasKey(l => l.Id);
            this.Ignore(lr => lr.LineRemarkType);
        }
    }
}