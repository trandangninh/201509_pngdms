using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Supplychain
{
    public class HseModel
    {
        public HseModel()
        {
            BosUnsafeDeps = new List<HseDepartmentModel>();
        }
        public string Target { get; set; }
        public string TargetRemark { get; set; }
        public string BosComplete { get; set; }
        public string BosCompleteRemark { get; set; }
        public int BosCompleteOwnerId { get; set; }
        public string BosToday { get; set; }
        public string BosDone { get; set; }
        public string BosUnsafe { get; set; }
        public string BosUnsafeCommon { get; set; }
        public int BosUnsafeOwnerId { get; set; }
        public string BosUnsafeRemark { get; set; }
        public List<HseDepartmentModel> BosUnsafeDeps { get; set; }

        public class HseDepartmentModel
        {
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public string Value { get; set; }
        }
    }
}