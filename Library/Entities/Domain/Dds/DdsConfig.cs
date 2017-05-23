using Entities.Domain.Departments;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Dds
{
    public partial class DdsConfig : BaseEntity
    {
        //public int DepartmentId { get; set; }
        //public virtual Department Department { get; set; }
        //public int DmsId { get; set; }
        //public virtual Dms Dms { get; set; }
        public int MeasureId { get; set; }
        public virtual Measure Measure { get; set; }
        public int LineId { get; set; }
        public virtual Line Line { get; set; }
        public DateTime CreatedDateTime { get; set; }
        //public DateTime UpdatedDateTime { get; set; }
        public int UserCreatedId { get; set; }
        //public int UserUpdatedId { get; set; }
    }
}
