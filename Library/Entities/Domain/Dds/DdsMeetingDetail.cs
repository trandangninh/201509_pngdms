using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain.Dds
{
    public class DdsMeetingDetail : BaseEntity
    {
        public int DdsMeetingResultId { get; set; }
        public virtual DdsMeetingResult DdsMeetingResult { get; set; }
        public int PomMaking { get; set; }
        public string PomMakingRemark { get; set; }
        public int PomPacking { get; set; }
        public string PomPackingRemark { get; set; }
        public int PomPlaning { get; set; }
        public string PomPlaningRemark { get; set; }
    }
}
