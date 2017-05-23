using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using RepositoryPattern;

namespace Entities.Domain
{
    public class ProductPlanning : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public PlanLineHardCodeType Line { get; set; }

        public PlanShiftHardCodeType Shift { get; set; }

        public string ProductName { get; set; }

        public string Result { get; set; }
    }

    public enum PlanLineHardCodeType
    {
        [Description("Shift 1")]
        Line1 = 1,

        [Description("Shift 2")]
        Line2 = 2,

        [Description("Shift 3")]
        Line3 = 3,

        [Description("Gabbana -- DSR(MSU)")]
        Gabbana = 4,

        [Description("FR (MK3)")]
        FRMK3 = 5,

        [Description("Shift 3")]
        FRMK4 = 6

 
    }

    public enum PlanShiftHardCodeType
    {
        [Description("Shift 1")]
        Shift1 = 1,

        [Description("Shift 2")]
        Shift2 = 2,

        [Description("Shift 3")]
        Shift3 = 3
    }
}
