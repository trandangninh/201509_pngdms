using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain
{
    public class UserInSupplyChainFpq : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int SupplyChainFpqId { get; set; }
    }
}
