using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.QualityAlerts
{
    public class SubColumnFormula : BaseEntity
    {
        public string Name { get; set; }
        public string Formula { get; set; }
    }
}
