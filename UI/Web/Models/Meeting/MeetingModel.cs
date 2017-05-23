using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Common;

namespace Web.Models.Meeting
{
    public class MeetingModel : BaseNoisEntityModel
    {
        public UserForMeetingViewModel CurrentLeader { get; set; }
        [AllowHtml]
        public string Leaders { get; set; }
        [AllowHtml]
        public string Members { get; set; }
        public DepartmentViewModel Department { get; set; }
    }

    public class UserForMeetingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserForMeetingViewModel()
        {
            Id = 0;
            Name = "";
        }
    }
}