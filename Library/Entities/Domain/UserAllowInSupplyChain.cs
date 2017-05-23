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
    public class UserAllowInSupplyChain : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int DmsTypeId { get; set; }
        public DmsType DmsType
        {
            get { return (DmsType) DmsTypeId; }
            set { DmsTypeId = (int) value; }
        }
    }
}
