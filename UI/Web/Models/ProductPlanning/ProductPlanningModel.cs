using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Web.Models.ProductPlanning
{
  

 
    public class ProductPlanningNewModel
    {
        public string DateTime { get; set; }

        public string ShiftType { get; set; }

        public List<ProductLineResult>  ListProductLineResult { get; set; }

        public ProductPlanningNewModel()
        {
            ListProductLineResult = new List<ProductLineResult>();
        }


    }

    public class ProductLineResult
    {
        public string LineType { get; set; }
        public string LineResult { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
    }

}