using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Users
{
    public static class SystemUserRoleNames
    {
        public static string Administrators { get { return "Administrators"; } }

        public static string Employees { get { return "Employees"; } }
        public static string MeetingLeaders { get { return "MeetingLeaders"; } }
        public static string Guest { get { return "Guest"; } }
    }
}
