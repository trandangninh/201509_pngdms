using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ScMeasures
{
    public class MqsMeasure : BaseEntity
    {
        public int ScMeasureId { get; set; }
        public int SupplierId { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }
}
