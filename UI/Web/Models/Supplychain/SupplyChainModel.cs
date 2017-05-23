using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Web.Models.ProductPlanning;

namespace Web.Models.SupplyChain
{
    public class SupplyChainModel
    {
        public SupplyChainModel()
        {
            SupplyChainHSE = new List<SupplyChainHSEModel>();
            SupplyChainDDS = new List<SupplyChainDDSModel>();
            SupplyChainFPQ = new List<SupplyChainFPQModel>();
            SupplyChainService = new List<SupplyChainServiceModel>();
            SupplyChainProductionPlanning = new List<SupplyChainProductionPlanningModel>();
            ProductionPlanning = new List<ProductPlanningNewModel>();
            SupplyChainMPSA = new List<SupplyChainMPSAModel>();
        }

        public int DepartmentId { get; set; }
        public DateTime Date { get; set; }

        public List<IssueModel> ListIssueModel { get; set; }
        public AttendanceModel AttendanceModel { get; set; }
        public List<SupplyChainHSEModel> SupplyChainHSE { get; set; }
        public List<SupplyChainDDSModel> SupplyChainDDS { get; set; }
        public List<SupplyChainFPQModel> SupplyChainFPQ { get; set; }
        public List<SupplyChainMPSAModel> SupplyChainMPSA { get; set; }
        public List<SupplyChainServiceModel> SupplyChainService { get; set; }
        public List<ProductPlanningNewModel> ProductionPlanning { get; set; }

        public List<SupplyChainProductionPlanningModel> SupplyChainProductionPlanning { get; set; }
        public Boolean permissionIssue { get; set; }
        public Boolean permissionAttendance { get; set; }
        public bool IsSupplyChainDepartment { get; set; }
    }


    #region SupplyChainHSE

    public class SupplyChainHSEModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }

        public string Owner { get; set; }

        public string OwnerId { get; set; }

        public string Making { get; set; }

        public string Packing { get; set; }

        public string CommonArea { get; set; }

        public string Remarks { get; set; }

        public string RemarkDisplay { get; set; }

        public DateTime Date { get; set; }
    }

    #endregion

    #region SupplyChainDDS

    public class SupplyChainDDSModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string Owner { get; set; }

        public int type { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }

        public string LPD1 { get; set; }

        public string LPD2 { get; set; }

        public string LPD3 { get; set; }

        public string Batch { get; set; }

        public string FRMK { get; set; }

        public string Bottle { get; set; }

        public string Sachet { get; set; }

        public string Pouch { get; set; }

        public string FRPK { get; set; }

        public string FE { get; set; }

        public string Remark { get; set; }



        public string RemarkDisplay { get; set; }

        public string BottleRemark { get; set; }

        public string SachetRemark { get; set; }

        public string PouchRemark { get; set; }

        public string FRPKRemark { get; set; }

        public DateTime Date { get; set; }
    }

    public class SupplyChainDDSUpdatePackingModel
    {
        public int Id { get; set; }

        public int type { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }


        public string Value { get; set; }

        public int LineCode { get; set; }

        public string Remark { get; set; }

        public DateTime Date { get; set; }

    }

    public class SupplyChainDDSUpdateMakingModel
    {
        public int Id { get; set; }

        public int type { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }

        public string Value { get; set; }

        public int LineCode { get; set; }

        public DateTime Date { get; set; }
    }


    public class SupplyChainFPQUpdatePackingModel
    {
        public int Id { get; set; }

        public int type { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }

        public string Value { get; set; }

        public string Remark { get; set; }

        public int LineCode { get; set; }

        public DateTime Date { get; set; }
    }

    public class SupplyChainDDSDataPackingModel
    {
        public string PRLastDay { get; set; }
        public string PRMTD { get; set; }
        public string PRLastDayRemark { get; set; }
        public string PRMTDRemark { get; set; }
    }

    public class SupplyChainFPQDataPackingModel
    {
        public string result { get; set; }
        public string remark { get; set; }
    }

    #endregion

    #region SupplyChainFPQ

    public class SupplyChainFPQModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public List<string> ListUsernameInSupplyChainFPQ { get; set; }
        public List<string> ListUserIdInSupplyChainFPQ { get; set; }

        public int type { get; set; }

        public string MeasureName { get; set; }

        public int MeasureCode { get; set; }

        public string LPD1 { get; set; }

        public string LPD2 { get; set; }

        public string LPD3 { get; set; }

        public string Batch { get; set; }

        public string FR { get; set; }

        public string FRMK { get; set; }

        public string Bottle { get; set; }

        public string Sachet { get; set; }

        public string Sachet1 { get; set; }
        public string Sachet2 { get; set; }

        public string Pouch { get; set; }

        public string Remark { get; set; }
        public string RemarkDisplay { get; set; }

        public string FRRemark { get; set; }

        public string BottleRemark { get; set; }

        public string SachetRemark { get; set; }

        public string PouchRemark { get; set; }

        public DateTime Date { get; set; }

        public SupplyChainFPQModel()
        {
            ListUsernameInSupplyChainFPQ = new List<string>();
            ListUserIdInSupplyChainFPQ = new List<string>();
        }
    }

    #endregion

    #region SupplyChainService

    public class SupplyChainServiceModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string UpdateUser { get; set; }



        public string Owner { get; set; }

        public int type { get; set; }

        public int MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string CFR { get; set; }

        public string SAMBC { get; set; }

        public string PrioritySKU { get; set; }

        public string PriorityLine { get; set; }

        public string Shipment { get; set; }

        public DateTime Date { get; set; }
    }

    #endregion

    #region SupplyChainMPSA

    public class SupplyChainMPSAModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string UpdateUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public int MeasureCode { get; set; }

        public List<string> ListUsernameInSupplyChainMPSA { get; set; }

        public List<string> ListUserIdInSupplyChainMPSA { get; set; }

        public string MeasureName { get; set; }

        public string Owner { get; set; }

        public int FR { get; set; }

        public int Bottle { get; set; }

        public int Sachet1 { get; set; }

        public int Sachet2 { get; set; }

        public int Pouch { get; set; }

        public string Remark { get; set; }

        public string RemarkDisplay { get; set; }

        public string OutputRemark { get; set; }

        public string MPSAFR { get; set; }

        public string MPSAFE { get; set; }

        public string FRRemarks { get; set; }

        public string BottleRemarks { get; set; }

        public string Sachet1Remarks { get; set; }

        public string Sachet2Remarks { get; set; }

        public string PouchRemark { get; set; }

        public DateTime Date { get; set; }

        public SupplyChainMPSAModel()
        {
            ListUsernameInSupplyChainMPSA = new List<string>();
            ListUserIdInSupplyChainMPSA = new List<string>();
        }
    }

    #endregion

    #region SupplyCHainProductionPlanning

    public class SupplyChainProductionPlanningModel
    {
        public int Id { get; set; }

        public string CreatedUser { get; set; }

        public string UpdatedUser { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string Owner { get; set; }

        public int type { get; set; }

        public int MeasureCode { get; set; }

        public string MeasureName { get; set; }

        public string Shift1 { get; set; }

        public string Shift2 { get; set; }

        public string Shift3 { get; set; }

        public double MonthTarget { get; set; }

        public double TodayPlan { get; set; }

        public double MTD { get; set; }

        public double Gap { get; set; }

        public string Remark { get; set; }

        public string RemarkDisplay { get; set; }

        public DateTime Date { get; set; }

        public string Shift1UIColorBg { get; set; }

        public string Shift2UIColorBg { get; set; }

        public string Shift3UIColorBg { get; set; }

        public SupplyChainProductionPlanningModel()
        {
            Shift1UIColorBg = "#FFF";
            Shift2UIColorBg = "#FFF";
            Shift3UIColorBg = "#FFF";
        }
    }

    #endregion

    public class IssueModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string NextStep { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string UserCreated { get; set; }

        public string UserAssignedId { get; set; }

        public string UserAssigned { get; set; }

        public string Status { get; set; }
        public int StatusId { get; set; }

        public string Type { get; set; }

        public string TypeId { get; set; }

        public string date { get; set; }

        public string ActionPlan { get; set; }

        public string When { get; set; }

        public string Note { get; set; }

        public string SystemFixDMSLinkage { get; set; }
    }


    public class AttendanceModel
    {
        public int Id { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UserCreated { get; set; }

        public string CreatedDateString { get; set; }

        public List<String> ListUsernameInAttendance { get; set; }
        public List<int> ListUserIdInAttendance { get; set; }
        public List<String> ListUsernameNotInAttendance { get; set; }
        public List<int> ListUserIdNotInAttendance { get; set; }
        public int TypeId { get; set; }

        public string Type { get; set; }

        public AttendanceModel()
        {
            ListUsernameInAttendance = new List<string>();
            ListUserIdInAttendance = new List<int>();
            ListUsernameNotInAttendance = new List<string>();
            ListUserIdNotInAttendance = new List<int>();
        }

    }


}