using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class SupplyChainHSE : BaseEntity
    {
        public int UserCreatedId { get; set; }
        public int UserUpdatedId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int MeasureCode { get; set; }

        public string Owner { get; set; }

        public string Making { get; set; }

        public string Packing { get; set; }

        public string CommonArea { get; set; }

        public string Remarks { get; set; }

    }

    public enum SupplyChainHSEMeasure
    {
        Target = 1,
        BOSCompletion = 2,
        BOSTopUnsafeBehaviour = 3
    }
}
