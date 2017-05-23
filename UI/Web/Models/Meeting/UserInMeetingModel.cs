using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Meeting
{
    public class UserInMeetingModel : BaseNoisEntityModel
    {
        public int MeetingId { get; set; }
        public UserForMeetingViewModel User { get; set; }
        public bool IsLeader { get; set; }
        public bool IsCurrentLeader { get; set; }
        public int Order { get; set; }
    }
}