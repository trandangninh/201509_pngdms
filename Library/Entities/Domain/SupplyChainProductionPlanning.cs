using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class SupplyChainProductionPlanning : BaseEntity
    {
        public int UserCreatedId { get; set; }

        public int UserUpdatedId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        
        public string Owner { get; set; }
        
        public int type { get; set; }

        public int MeasureCode { get; set; }

        public string Shift1 { get; set; }

        public string Shift2 { get; set; }

        public string Shift3 { get; set; }

        public double MonthTarget { get; set; }

        public double TodayPlan { get; set; }

        public double MTD { get; set; }

        public double Gap { get; set; }
        
        public string Remark { get; set; }

    }

    public enum SupplyChainProductionPlanningType
    {
        Making = 1,
        Packing = 2
    }
    public enum SupplyChainProductionPlanningMeasure
    {
        LPD1=1,
        LPD2=2,
        LPD3=3,
        DSR=4,
        FRMK3=5,
        FRMK4 = 6,
        Sachet=7,
        Pouch=8,
        Bottle=9,
        FE=10,
        FR=11,
        Total=12
    }
}
