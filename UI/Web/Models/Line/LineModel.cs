using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Common;
using Web.Models.Department;

namespace Web.Models.Line
{
    public class LineModel
    {
        public int Id { get; set; }
        public string LineCode { get; set; }

        public string LineName { get; set; }

        public string LineDesc { get; set; }

        public string Note { get; set; }

        public DepartmentViewModel Department { get; set; }

        public int Order { get; set; }


        public List<string> ListUsername { get; set; }
        public bool Active { get; set; }
        public LineModel()
        {
            ListUsername = new List<string>();
        }
    }
}