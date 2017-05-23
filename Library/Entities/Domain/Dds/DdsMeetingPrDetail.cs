using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain.Dds
{
    public class DdsMeetingPrDetail : BaseEntity
    {
        public int DdsMeetingResultId { get; set; }
        public virtual DdsMeetingResult DdsMeetingResult { get; set; }
        public decimal PrLastDay { get; set; }
        public string PrLastDayRemark { get; set; }
        public decimal PrMtd { get; set; }
        public string PrMtdRemark { get; set; }
    }
}
