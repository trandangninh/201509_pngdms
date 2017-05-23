using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RepositoryPattern;

namespace Entities.Domain
{
    public class SupplyChainMPSA : BaseEntity
    {
        public int UserCreatedId { get; set; }

        public int UserUpdatedId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int MeasureCode { get; set; }

        public string Owner { get; set; }

        public int FR { get; set; }

        public string FRRemarks { get; set; }

        public int Bottle { get; set; }

        public string BottleRemarks { get; set; }

        public int Sachet1 { get; set; }

        public string Sachet1Remarks { get; set; }

        public int Sachet2 { get; set; }

        public string Sachet2Remarks { get; set; }

        public int Pouch { get; set; }

        public string PouchRemark { get; set; }

        public string Remark { get; set; }

        public string OutputRemark { get; set; }

        public string MPSAFR { get; set; }

        public string MPSAFE { get; set; }
    }

    public class SupplyChainMPSAUpdateTotalPo      
    {
         public int result { get; set; }
         public int linceCode { get; set; }
         public int MeasureCode { get; set; }
         public DateTime CreatedDate { get; set; }

    }


    public class UserInSupplyChainMPSA : BaseEntity
    {
        public int UserId { get; set; }

        public int SupplyChainMPSAId { get; set; }
    }
  
    public enum SupplyChainMPSAMeasure
    {
        Target = 1,
        TotalPO = 2,
        ReasonCodePOMissedduetoMaking = 3,
        ReasonCodePOMissedduetoPacking = 4,
        ReasonCodePOMissedduetoPlanning = 5,
        DailyMPSA=6,
        MTDMPSA=7

    }

    public class SupplyChainMPSADataPacking
    {
        public int result { get; set; }

        public string remarks { get; set; }
    }
}
