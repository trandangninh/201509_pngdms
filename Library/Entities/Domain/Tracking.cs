using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain
{
    public class Tracking : BaseEntity
    {
        public User CreatedUser { get; set; }

        public int UserCreated{ get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string LineCode { get; set; }

        public string FGCode { get; set; }

        public string Variant { get; set; }

        public string Size { get; set; }

        public string Lot { get; set; }

        public string Cause { get; set; }

        public string Where { get; set; }

        public int Quantity { get; set; }

    }

  
   
}
