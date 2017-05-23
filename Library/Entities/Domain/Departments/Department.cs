using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Departments
{
    /// <summary>
    /// Department entity
    /// </summary>
    public partial class Department : BaseEntity
    {
        /// <summary>
        /// Gets or sets department name
        /// </summary>
        public string Name { get; set; }

        public bool Active { get; set; }

        public int Order { get; set; }

        public int DepartmentTypeId { get; set; }

        public DepartmentType DepartmentType
        {
            get
            {
                return (DepartmentType)DepartmentTypeId;
            }
            set { DepartmentTypeId = (int) value; }
        }

        private ICollection<Line> _lines;
        public virtual ICollection<Line> Lines
        {
            get { return _lines ?? (_lines = new List<Line>()); }
            protected set { _lines = value; }
        }
    }

    public enum DepartmentType
    {
        Basic = 1,
        SupplyChain = 2
    }
}
