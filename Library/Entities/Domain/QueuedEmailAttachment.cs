using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain
{
    public class QueuedEmailAttachment: BaseEntity
    {
        public string FilePath { get; set; }

        public QueuedEmail QueuedEmail { get; set; }

        public int QueuedEmailId { get; set; }
    }
}
