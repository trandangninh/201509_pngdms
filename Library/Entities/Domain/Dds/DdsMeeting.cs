using Entities.Domain.Departments;
using Entities.Domain.Meetings;
using Entities.Domain.SupplyChain;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Dds
{
    public partial class DdsMeeting : BaseEntity
    {
        private ICollection<DdsMeetingResult> _ddsMeetingResults;
        private ICollection<Attendance> _attendances;
        //private ICollection<SupplyChainDetail> _supplyChainDetails;
        public DateTime CreatedDateTime { get; set; }
        //public int UserCreatedId { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public string LineRemark { get; set; }

        /// <summary>
        /// DdsMeetingResult
        /// </summary>
        public virtual ICollection<DdsMeetingResult> DdsMeetingResults
        {
            get { return _ddsMeetingResults ?? (_ddsMeetingResults = new List<DdsMeetingResult>()); }
            protected set { _ddsMeetingResults = value; }
        }
        public virtual ICollection<Attendance> Attendances
        {
            get { return _attendances ?? (_attendances = new List<Attendance>()); }
            set { _attendances = value; }
        }
        //public virtual ICollection<SupplyChainDetail> SupplyChainDetails
        //{
        //    get { return _supplyChainDetails ?? (_supplyChainDetails = new List<SupplyChainDetail>()); }
        //    set { _supplyChainDetails = value; }
        //}
    }
}
