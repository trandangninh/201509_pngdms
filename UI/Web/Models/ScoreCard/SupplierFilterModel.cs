using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models.ScoreCard
{
    public class SupplierFilterModel
    {
        public int Year { get; set; }
        public int SupplierId { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        //public List<int> MonthIds { get; set; }
    }
}
