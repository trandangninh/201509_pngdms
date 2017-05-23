using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.DdsMeeting
{
    public class IssueListModel
    {
        public bool CanWriteIssue { get; set; }
        public int DepartmentId { get; set; }
        public string IssueStatusId { get; set; }
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public DateTime Date { get; set; }
        public bool ShowDepartmentCbx { get; set; }
        public class IssueModel
        {
            public int Id { get; set; }

            public int Index { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public string Content { get; set; }

            public string NextStep { get; set; }

            public string CreatedDate { get; set; }

            public string UpdatedDate { get; set; }

            public string UserCreated { get; set; }

            public int UserAssignedId { get; set; }

            public string UserAssigned { get; set; }

            public string Status { get; set; }
            public int StatusId { get; set; }

            public string Type { get; set; }

            public string TypeId { get; set; }

            public string date { get; set; }

            public string ActionPlan { get; set; }

            public string When { get; set; }

            public string Note { get; set; }

            public string IssuesPriority { get; set; }

            public string WhenDue { get; set; }

            public string SystemFixDMSLinkage { get; set; }
            public bool IsAllowEdit { get; set; }
        }
    }
}