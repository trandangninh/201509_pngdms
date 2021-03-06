using System.Collections.Generic;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public class PermissionRecord : BaseEntity
    {
        private ICollection<UserRole> _userRoles;
        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }

        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
    }
}
