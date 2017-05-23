using Entities.Domain.Departments;
using Entities.Domain.Users;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Meetings
{
    public partial class Meeting : BaseEntity
    {
        private ICollection<UserInMeeting> _userInMeetings;

        /// <summary>
        /// Gets or sets department id
        /// </summary>
        public int DepartmentId{ get; set; }
        public virtual Department Department { get; set; }
        /// <summary>
        /// Current lead user identify
        /// </summary>
        public int CurrentLeaderId { get; set; }
        //public virtual User CurrentLeader { get; set; }
        public DateTime UpdateCurrentLeaderDate { get; set; }

        public virtual ICollection<UserInMeeting> UserInMeetings
        {
            get { return _userInMeetings ?? (_userInMeetings = new List<UserInMeeting>()); }
            set { _userInMeetings = value; }
        }
    }
}
