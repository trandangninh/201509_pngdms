using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class SupplyChainService : BaseEntity
    {
        public int UserCreatedId { get; set; }

        public int UserUpdatedId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Owner { get; set; }

        public int type { get; set; }

        public int MeasureCode { get; set; }

        public string CFR { get; set; }

        public string SAMBC { get; set; }

        public string PrioritySKU { get; set; }

        public string PriorityLine { get; set; }

        public string Shipment { get; set; }

    }

    public enum SupplyChainServiceType
    {
        DailyFE = 1,
        DailyFR = 2
    }
    public enum SupplyChainServiceMeasure
    {
        Target = 1,
        Daily = 2,
        MTD = 3
    }
}
