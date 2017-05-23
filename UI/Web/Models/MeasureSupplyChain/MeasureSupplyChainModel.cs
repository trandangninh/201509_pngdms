using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.MeasureSupplyChain
{
  
    public class MeasureSupplyChainModel
    {
        public int Id { get; set; }

        public string MeasureSupplyChainName { get; set; }

        public string MeasureSupplyChainCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string DmsCode { get; set; }
    }
   
}