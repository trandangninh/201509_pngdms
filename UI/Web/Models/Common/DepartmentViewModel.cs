using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Common
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DepartmentViewModel()
        {
            Id = 0;
            Name = "";
        }
    }
}