using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Users;
using RepositoryPattern;
using Entities.Domain.Departments;

namespace Entities.Domain
{
    public class Line : BaseEntity
    {
        private ICollection<User> _users; 
        public string LineCode { get; set; }

        public string LineName { get; set; }

        public string LineDesc { get; set; }

        public string Note { get; set; }

        public int Index { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public bool Active { get; set; }
        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }
    }
}
