using System;
using System.Collections.Generic;
using Entities.Domain.Users;
using RepositoryPattern;
using Entities.Domain.Departments;

namespace Entities.Domain
{
    public class Dms : BaseEntity
    {
        private ICollection<Measure> _measures;
        private ICollection<User> _users;
        public string DmsCode { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public int DmsTypeId { get; set; }

        public DmsType DmsType
        {
            get { return (DmsType) DmsTypeId; }
            set { DmsTypeId = (int) value; }
        }

        public virtual ICollection<Measure> Measures
        {
            get { return _measures ?? (_measures = new List<Measure>()); }
            protected set { _measures = value; }
        }
        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }
    }
    public enum DmsType
    {
        HSE = 1,
        DDS = 2,
        FPQ = 3,
        MPSA = 4,
        Productionlanning = 5,
        Service = 6
    }
}
