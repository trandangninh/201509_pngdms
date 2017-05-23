using System;
using System.Collections.Generic;
using Web.Models.Department;

namespace Web.Models.Common
{
    public class LeftNavigationModel
    {
        public bool LoginAsAdmin { get; set; }
        public bool IsLogin { get; set; }

        public List<DepartmentViewModel> Departments { get; set; }
        
        public LeftNavigationModel()
        {
            Departments = new List<DepartmentViewModel>();
        }
    }
    
}
