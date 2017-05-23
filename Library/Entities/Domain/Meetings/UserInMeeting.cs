using Entities.Domain.Users;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Meetings
{
    public partial class UserInMeeting : BaseEntity
    {
        /// <summary>
        /// Gets or sets department id
        /// </summary>
        public int MeetingId { get; set; }
        public virtual Meeting Meeting { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsLeader { get; set; }
        public int Order { get; set; }
    }
}
