using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Department
{
    public class DepartmentModel : BaseNoisEntityModel
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}