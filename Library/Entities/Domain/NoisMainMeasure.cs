using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;

namespace Entities.Domain
{
    //public class NoisMainMeasure: BaseEntity
    //{
    //    public DateTime CreatedDateTime { get; set; }

    //    public DateTime UpdatedDateTime { get; set; }

      
    //    public string Result { get; set; }

    //    public DmsCode DmsHardCode { get; set; }

    //    public NoisMainMeasureType TypeHardCode { get; set; }

    //    public LineHardCodeType LineHardCode { get; set; }

    //    public int UserCreatedId { get; set; }

    //    public int UserUpdatedId { get; set; }

    //    public NoisMainMeasure()
    //    {
    //        CreatedDateTime = DateTime.Now;
    //        UpdatedDateTime = DateTime.Now;
    //    }
    //}


    public enum NoisMainMeasureType
    {
        SafetyTriggerCompliance = 1,
        SafetyNearMiss=2,
        QualityTriggerCompliance =3,
        PrimaryQFactorIncompliance =4,
        HoldReworkForDay =5,
        DefectOpen =6,
        TaskNotCompletedOnTime =7,
        CLNotCompliance =8,
        ScrapDueToCO = 9,
        ActualScrapOnLine=10,
        AmountOfBulkProduceDay=11,
        ScaptOnlineMSU=12,
        RCOGTGOutOfTarget =13,
        WorkOrderNotCompletionOnTime=14,
        TotalPO =15,
        POMissed=16,
        ProductionVolume=17,
        OfUnplannedStopsDayLineConstraint =18,
        PRImpactedToPackingLines = 19,
        EOCoachingCompletion=20,
        QualityAlert =21,
        FEBulk7Days =22,
        FEBulk3Days = 23,
        PCSAdditionSigmaIncompliance = 24,
        PCSCrTzCompliance = 25,
        PRYesterday = 26,
        PROfLPD1BatchProcess = 27,
        PROfLPD2ImpactedToPackingLine = 28,
        DDIError	 = 29,
        ContractorPlan = 30,
        DefectFoundByLineLeader = 31,
        DDI = 32,
        MPSA = 33,
        UnplannedStopsDayLineConstraint = 34,
        PR= 35,
        NumberOfContainerDeedmacMelting = 36,
        ContainerDeemacReadyToUnload=37,
        ContainerReturnPending = 38,
        DeedmacIsotankAtTemporatyStation=39,
        CDeedmacExpiredOn=40,
    }

    public enum LineHardCodeType
    { 
        FR=4,
        Cha=1,
        KWT=2,
        Pou =3,
       
        FEBatch=5,
        LPD3=6,
        LPD2=7,
        LPD1=8,
        Sac1=9,
        Sac2 = 10,
        Bottle=11,
        FRMK=12,
        FRPK = 13,
        DeedmacOperation = 14,

    }
    public enum DmsCode
    {
        IERP = 1,
        FPQ = 2,
        DH = 3,
        CIL = 4,
        Cl=5,
        RCO = 6,
        MP_S =7,
        MPS = 8,
        Production = 9,
        SoftPoints=10,
        WINForTheShift=11,
        DeedmacOperation = 12
    }
}
