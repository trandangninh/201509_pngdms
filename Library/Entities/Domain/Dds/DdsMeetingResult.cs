using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Dds
{
    public partial class DdsMeetingResult : BaseEntity
    {
        public int DdsMeetingId { get; set; }
        public virtual DdsMeeting DdsMeeting { get; set; }
        public int MeasureId { get; set; }
        public virtual Measure Measure { get; set; }
        public int LineId { get; set; }
        public virtual Line Line { get; set; }
        public string Result { get; set; }
        public bool ReadOnly { get; set; }
        public string Remark { get; set; }
    }
}
