using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class SupplyChainFPQ : BaseEntity
    {
        public int UserCreatedId { get; set; }

        public int UserUpdatedId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Owner { get; set; }

        public int type { get; set; }

        public int MeasureCode { get; set; }

        public string LPD1 { get; set; }

        public string LPD2 { get; set; }

        public string LPD3 { get; set; }



        public string Batch { get; set; }

        public string FR { get; set; }

        public string FRRemark { get; set; }

        public string Bottle { get; set; }

        public string BottleRemark { get; set; }

        public string Sachet { get; set; }

        public string Sachet1 { get; set; }

        public string Sachet2 { get; set; }

        public string Sachet1Remark { get; set; }

        public string Sachet2Remark { get; set; }

        public string FRMK { get; set; }

        public string SachetRemark { get; set; }

        public string Pouch { get; set; }

        public string PouchRemark { get; set; }

        public string Remark { get; set; }

    }
    public enum SupplyChainFPQMeasure
    {
        Target = 1,
        QuanlityBOS = 2,
        UHEventPending = 3,
        UHEventLastDay = 4,
        UHEventMTD = 5,

    }
}
