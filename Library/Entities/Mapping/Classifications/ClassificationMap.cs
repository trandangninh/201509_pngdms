using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Entities.Domain.Classifications;
using RepositoryPattern;

namespace Entities.Mapping.Classifications
{
    public partial class ClassificationMap : EntityTypeConfiguration<Classification>
    {
        public ClassificationMap()
        {
            this.ToTable("Classification");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Code).IsRequired();
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}
