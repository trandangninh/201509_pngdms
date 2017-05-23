using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.LineRemark
{
    public class LineRemarkModel
    {
        public int Id { get; set; }

        public string LineCode { get; set; }

        public string Remark { get; set; }

        public int TypeCode { get; set; }

        public int TypeName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUser { get; set; }

        public string CreateUser { get; set; }

    }
}