using Entities.Domain.Security;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Users
{
    /// <summary>
    /// Userrole entity
    /// </summary>
    public class UserRole : BaseEntity
    {
        private ICollection<PermissionRecord> _permissionRecords; 
        /// <summary>
        /// Gets or sets system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// get or set user role is system
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// get or set isActive or not
        /// </summary>
        public bool IsActive { get; set; }


        public virtual ICollection<PermissionRecord> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecord>()); }
            protected set { _permissionRecords = value; }
        }
    }
}
