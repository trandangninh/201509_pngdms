using Entities.Domain.Departments;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Mapping.Departments
{
    /// <summary>
    /// Deparment Mapping
    /// </summary>
    public partial class DepartmentMap : EntityTypeConfiguration<Department>
    {
        public DepartmentMap()
        {
            this.ToTable("Department");
            this.HasKey(pa => pa.Id);
            this.Ignore(d => d.DepartmentType);
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}
