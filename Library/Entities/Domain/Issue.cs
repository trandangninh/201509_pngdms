using System;
using Entities.Domain.Departments;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain
{
    public class Issue: BaseEntity
    {
        /// <summary>
        /// Created user identify
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Created user object
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Created user identify
        /// </summary>
        public int UserOwnerId { get; set; }
        /// <summary>
        /// Created user object
        /// </summary>
        public virtual User UserOwner { get; set; }
        public string Content { get; set; }

        public string NextStep { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int IssueStatusId { get; set; }

        public IssueStatus IssueStatus {
            get { return (IssueStatus) IssueStatusId; }
            set { IssueStatusId = (int)value; }
        }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public string ActionPlan { get; set; }

        public string When { get; set; }

        public string Note { get; set; }

        public string IssuesPriority { get; set; }

        public DateTime WhenDue { get; set; }

        public string SystemFixDmsLinkage { get; set; }
    }

    public enum IssueStatus
    {
        Open=1,
        Closed=2,
        Delayed=3
    }

    public enum IssueType
    {
        Making = 1,
        Packing = 2,
        SupplyChain = 3
    }
}
