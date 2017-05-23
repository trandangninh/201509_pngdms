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
    

    public class ShutdownRequest : BaseEntity
    {
        public int UserCreatedId { get; set; }
        public virtual User CreatedUser { get; set; }
        public string ShutdownRequestContent { get; set; }

        
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string MakingScope { get; set; }

        public string PackingScope { get; set; }

        public DateTime BasePlanDate { get; set; }

        public string Comment { get; set; }

        public string Remark { get; set; }

        public int ShutdownStatusId { get; set; }

        public ShutdownStatus ShutdownStatus
        {
            get { return (ShutdownStatus)ShutdownStatusId; }
            set { ShutdownStatusId = (int)value; }
        }
    }
    public enum ShutdownStatus
    {
        Open = 1,
        Closed = 2,
        Delayed = 3
    }
}
