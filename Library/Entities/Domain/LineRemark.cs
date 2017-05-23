using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain
{
    public class LineRemark : BaseEntity
    {
        public string LineCode { get; set; }

        public string Remark { get; set; }

        public int LineRemarkTypeId { get; set; }

        public LineRemarkType LineRemarkType
        {
            get { return (LineRemarkType) LineRemarkTypeId; }
            set { LineRemarkTypeId = (int) value; }
        }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateUserId { get; set; }

        public int CreateUserId { get; set; }

    }


    public enum LineRemarkType
    {
        Making= 1,
        Packing = 2,
    }
}
