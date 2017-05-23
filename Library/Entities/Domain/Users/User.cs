using Entities.Domain.Activity;
using Entities.Domain.Departments;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Users
{
    /// <summary>
    /// User entity
    /// </summary>
    public class User : BaseEntity
    {
        private ICollection<UserRole> _userRoles;
        private ICollection<Department> _departments;
        private ICollection<Issue> _asignedIssues;
        private ICollection<Issue> _createdIssues;
        private ICollection<ShutdownRequest> _shutdownRequests;
        private ICollection<ActivityLog> _activityLogs;
        private ICollection<Line> _lines;
        private ICollection<Dms> _dmses;
        private ICollection<Measure> _measures;
        /// <summary>
        /// Gets or sets User guid
        /// </summary>
        public Guid UserGuid { get;set;}

        /// <summary>
        /// Gets or sets username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets password salt
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets first name
        /// </summary>
        public string FirstName { get;set;}

        /// <summary>
        /// Gets or sets last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets active
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Gets or sets deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the last login date
        /// </summary>
        public DateTime LastLoginDate { get; set; }
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
        /// <summary>
        /// Issueses are asigned
        /// </summary>
        public virtual ICollection<Issue> AsignedIssues
        {
            get { return _asignedIssues ?? (_asignedIssues = new List<Issue>()); }
            protected set { _asignedIssues = value; }
        }
        /// <summary>
        /// Issueses created
        /// </summary>
        public virtual ICollection<Issue> CreatedIssues
        {
            get { return _createdIssues ?? (_createdIssues = new List<Issue>()); }
            protected set { _createdIssues = value; }
        }
        public virtual ICollection<ShutdownRequest> ShutdownRequests
        {
            get { return _shutdownRequests ?? (_shutdownRequests = new List<ShutdownRequest>()); }
            protected set { _shutdownRequests = value; }
        }
        public virtual ICollection<ActivityLog> ActivityLogs
        {
            get { return _activityLogs ?? (_activityLogs = new List<ActivityLog>()); }
            protected set { _activityLogs = value; }
        }
        public virtual ICollection<Line> Lines
        {
            get { return _lines ?? (_lines = new List<Line>()); }
            protected set { _lines = value; }
        }
        public virtual ICollection<Dms> Dmses
        {
            get { return _dmses ?? (_dmses = new List<Dms>()); }
            protected set { _dmses = value; }
        }
        public virtual ICollection<Measure> Measures
        {
            get { return _measures ?? (_measures = new List<Measure>()); }
            protected set { _measures = value; }
        }

        public virtual ICollection<Department> Departments
        {
            get { return _departments ?? (_departments = new List<Department>()); }
            protected set { _departments = value; }
        }
    }
}
