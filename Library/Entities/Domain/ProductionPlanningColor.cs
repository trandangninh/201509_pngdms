using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain
{
    public class ProductionPlanningColor : BaseEntity
    {

        public string ProductionName { get; set; }
        public string Color { get; set; }
    }

  
}
