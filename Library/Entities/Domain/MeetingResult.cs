using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain
{
    public class MeetingResult: BaseEntity
    {
        public string UserId { get; set; }

        public int MeasureId { get; set; }

        public int LineId { get; set; }

        public string Result { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
