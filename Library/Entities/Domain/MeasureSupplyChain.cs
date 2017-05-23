using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    

    public class MeasureSupplyChain : BaseEntity
    {
        public string MeasureSupplyChainName { get; set; }

        public string MeasureSupplyChainCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string DmsCode { get; set; }

    }
}
