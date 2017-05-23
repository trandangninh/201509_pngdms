using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Common;

namespace Web.Models.Dms
{
    public class DmsModel
    {
        public int Id { get; set; }

        public string DmsCode { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public DepartmentViewModel Department { get; set; }

        public List<string> ListUsername { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public DmsModel()
        {
            ListUsername = new List<string>();
        }
    }
    public class UserAvailableModel
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}