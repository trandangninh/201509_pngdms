using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entities.Domain;
using Microsoft.Ajax.Utilities;

using Service.Common;
using Service.Interface;
using Service.Messages;
using Service.SupplyChain;
using Service.Users;
using Web.Extend;
using Web.Models.SupplyChain;
using Service.Implement;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Configuration;
using Service.Departments;


namespace Web.Controllers
{
    public class SupplyChainReportController : Controller
    {
        #region Field

        private readonly ISupplyChainHSEService _supplyChainHSE;
        private readonly ISupplyChainDDSService _supplyChainDDS;
        private readonly ISupplyChainFPQService _supplyChainFPQ;
        private readonly ISupplyChainMPSAService _supplyChainMPSA;
        private readonly ISupplyChainProductionPlanningService _supplyChainProductionPlanning;
        private readonly ISupplyChainServiceService _supplyChainService;
        private readonly IUserService _userService;
        //private readonly IAttendanceService _attendanceService;
        //private readonly INoisMainMeasureService _noisMainMeasureService;
        private readonly IReportService _reportService;
        private readonly IMeasureSupplyChainService _measureSupplyChainService;
        private readonly ISendMailService _sendMailService;
        private readonly IWorkFlowMessageService _workFlowMessageService;

        #endregion field

        #region Contructor

        public SupplyChainReportController(ISupplyChainHSEService supplyChainHSE, IUserService userService,
            //IAttendanceService attendanceService, 
            ISupplyChainDDSService supplyChainDds, ISupplyChainFPQService supplyChainFpq, ISupplyChainServiceService supplyChainService, ISupplyChainProductionPlanningService supplyChainProductionPlanning, 
            //INoisMainMeasureService noisMainMeasureService, 
            ISupplyChainMPSAService supplyChainMpsa, IMeasureSupplyChainService measureSupplyChainService, IReportService reportService, ISendMailService sendMailService, IWorkFlowMessageService workFlowMessageService)
        {
            _supplyChainHSE = supplyChainHSE;
            _userService = userService;
            //_attendanceService = attendanceService;
            _supplyChainDDS = supplyChainDds;
            _supplyChainFPQ = supplyChainFpq;
            _supplyChainService = supplyChainService;
            _supplyChainProductionPlanning = supplyChainProductionPlanning;
            //_noisMainMeasureService = noisMainMeasureService;
            _supplyChainMPSA = supplyChainMpsa;
            _measureSupplyChainService = measureSupplyChainService;
            _reportService = reportService;
            _sendMailService = sendMailService;
            _workFlowMessageService = workFlowMessageService;
        }

        #endregion

        // GET: SupplyChain
        public async Task<ViewResult> Index(string fromDate, string toDate)
        {
            DateTime fromDateSearch;
            if (String.IsNullOrEmpty(fromDate)) fromDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(fromDate, culture, styles, out fromDateSearch))
                {
                    fromDateSearch = DateTime.Now;
                }
            }

            DateTime toDateSearch;
            if (String.IsNullOrEmpty(toDate)) toDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(toDate, culture, styles, out toDateSearch))
                {
                    toDateSearch = DateTime.Now;
                }
            }

            var listModel = new List<SupplyChainModel>();



            for (var i = fromDateSearch; i <= toDateSearch; i = i.AddDays(1))
            {
                DateTime dateSearch = i;

                var model = new SupplyChainModel();

                #region HSE

                var allUser = _userService.GetAllUsersAsync();
                var owner = allUser.LastOrDefault();
                var currentDate = DateTime.Now;

                var currentUser = await _userService.GetUserByUsernameAsync("Admin");
                var supplyChainHSE = new List<SupplyChainHSEModel>();


                supplyChainHSE = new List<SupplyChainHSEModel>()
                                     {
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int) SupplyChainHSEMeasure.Target,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty
                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.BOSCompletion
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure.BOSCompletion,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty

                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.BOSTopUnsafeBehaviour
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure
                                                 .BOSTopUnsafeBehaviour,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty
                                         }
                                     };
                var allsupplyChain =
                    _supplyChainHSE.GetAllSupplyChainHSEs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target).Where(p => p.CreatedDate <= dateSearch)
                        .ToList();

                if (allsupplyChain.Count == 0)
                {
                    var newModel = new SupplyChainHSE()
                    {
                        Owner = owner == null ? "" : owner.Username,
                        UserUpdatedId = currentUser.Id,
                        CreatedDate = dateSearch,
                        UpdatedDate = dateSearch,
                        UserCreatedId = currentUser.Id,
                        MeasureCode = (int)SupplyChainHSEMeasure.Target,
                        Making = "Daily 1 LT to share the BOS Observation"

                    };
                    await _supplyChainHSE.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChain.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new SupplyChainHSE()
                        {
                            Owner = owner == null ? "" : owner.Username,
                            CreatedDate = dateSearch,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            Making = lastDayTarget.Making,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainHSE.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainHSE)
                {


                    var supplychainResult =
                        _supplyChainHSE.GetSupplyChainHSEMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {

                        item.MeasureCode = supplychainResult.MeasureCode;
                        item.Making = supplychainResult.Making;
                        //item.MeasureName = ((SupplyChainHSEMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.Packing = supplychainResult.Packing;
                        item.RemarkDisplay = supplychainResult.Remarks == null
                            ? ""
                            : supplychainResult.Remarks.Replace("\n", "<br/>");
                        item.Remarks = supplychainResult.Remarks == null
                            ? ""
                            : supplychainResult.Remarks.Replace("<br/>", "\n");
                        //item.CreatedUser = supplychainResult.CreatedUser.Username;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.CommonArea = supplychainResult.CommonArea;
                    }
                    else
                    {
                        var yesterdayHSE =dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainHSEOwner =
                     _supplyChainHSE.GetAllSupplyChainHSEs()
                         .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayHSE)
                         .ToList();


                        if (allsupplyChainHSEOwner.Count == 0)
                        {

                            var newModel = new SupplyChainHSE()
                            {
                                Owner = owner == null ? "" : owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = dateSearch,
                                UpdatedDate = dateSearch,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,


                            };
                            await _supplyChainHSE.CreateAsync(newModel);
                            item.Owner = owner == null ? "" : owner.Username;
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainHSEOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var lastDayTarget = allsupplyChainHSEOwner.OrderBy(p => p.UpdatedDate).Last();
                                if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                {


                                    var newModel = new SupplyChainHSE()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = dateSearch,
                                        UpdatedDate = dateSearch,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                    };
                                    await _supplyChainHSE.CreateAsync(newModel);
                                    item.Owner = lastDayTarget.Owner;
                                }

                            }
                        }
                    }

                    //check owner


                }





                model.SupplyChainHSE = supplyChainHSE;
                #endregion

                #region DDS


                var supplyChainDDS = new List<SupplyChainDDSModel>()
                                         {
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             }

                                         };
                var allsupplyChainMaking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 1).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainMaking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        LPD1 = "90%",
                        LPD2 = "90%",
                        LPD3 = "90%",
                        Batch = "90%",
                        FRMK = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMaking.OrderBy(p => p.UpdatedDate).Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {
                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 1,
                            LPD1 = lastDayTarget.LPD1,
                            LPD2 = lastDayTarget.LPD2,
                            LPD3 = lastDayTarget.LPD3,
                            Batch = lastDayTarget.Batch,
                            FRMK = lastDayTarget.FRMK,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                var allsupplyChainPacking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 2).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainPacking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        Bottle = "90%",
                        Sachet = "90%",
                        Pouch = "90%",
                        FRPK = "90%",
                        FE = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainPacking.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            Owner = lastDayTarget.Owner,
                            CreatedDate = dateSearch,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 2,
                            Bottle = lastDayTarget.Bottle,
                            Sachet = lastDayTarget.Sachet,
                            Pouch = lastDayTarget.Pouch,
                            FRPK = lastDayTarget.FRPK,
                            FE = lastDayTarget.FE,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainDDS)
                {
                    var supplychainResult =
                        _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FRPK = supplychainResult.FRPK;
                        item.Pouch = supplychainResult.Pouch;
                        item.RemarkDisplay = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("\n", "<br/>");
                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.FRMK = supplychainResult.FRMK;
                        item.FE = supplychainResult.FE;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Batch = supplychainResult.Batch;

                        item.BottleRemark = supplychainResult.BottleRemark;
                        item.FRPKRemark = supplychainResult.FRPKRemark;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.SachetRemark = supplychainResult.SachetRemark;

                        await _supplyChainDDS.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        var yesterdayDDS = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainDDSOwner =
                        _supplyChainDDS.GetAllSupplyChainDDSs()
                            .Result.Where(p => p.MeasureCode == item.MeasureCode && p.type == item.type && p.CreatedDate <= yesterdayDDS).OrderBy(p => p.UpdatedDate)
                            .ToList();
                        if (allsupplyChainDDSOwner.Count == 0)
                        {

                            var newModel = new SupplyChainDDS()
                            {
                                Owner = owner == null ? "" : owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = dateSearch,
                                UpdatedDate = dateSearch,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,
                                type = item.type

                            };
                            await _supplyChainDDS.CreateAsync(newModel);
                            item.Owner = owner == null ? "" : owner.Username;
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainDDSOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var lastDayTarget = allsupplyChainDDSOwner.Last();
                                if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                                {

                                    var newModel = new SupplyChainDDS()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = dateSearch,
                                        UpdatedDate = dateSearch,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                        type = item.type
                                    };
                                    await _supplyChainDDS.CreateAsync(newModel);

                                    item.Owner = lastDayTarget.Owner;
                                }
                            }
                        }
                    }
                }


                model.SupplyChainDDS = supplyChainDDS;



                #endregion

                #region FPQ

                var SupplyChainFPQ = new List<SupplyChainFPQModel>()
                                         {
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.QuanlityBOS
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.QuanlityBOS,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventPending
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventPending,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             }

                                         };
                var allsupplyChainFPQ =
                    _supplyChainFPQ.GetAllSupplyChainFPQs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainFPQMeasure.Target).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainFPQ.Count == 0)
                {
                    var newModel = new SupplyChainFPQ()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainFPQMeasure.Target,
                        LPD1 = "Daily 1 LT to share the BOS Observation",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id

                    };
                    await _supplyChainFPQ.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFPQ.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {
                        var newModel = new Entities.Domain.SupplyChainFPQ()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            LPD1 = lastDayTarget.LPD1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainFPQ.CreateAsync(newModel);
                    }

                }
                foreach (var item in SupplyChainFPQ)
                {
                    var supplychainResult =
                        _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                           dateSearch);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainFPQMeasure) supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.ListUsernameInSupplyChainFPQ =
                            _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainResult.Id);

                        if (item.ListUsernameInSupplyChainFPQ.Count == 0)
                        {

                            var listStringUsernameFPQ = new List<string>();
                            listStringUsernameFPQ.Add(owner.Username);
                            item.ListUsernameInSupplyChainFPQ = listStringUsernameFPQ;

                        }

                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FR = supplychainResult.FR;
                        item.FRMK = supplychainResult.FRMK;
                        item.Pouch = supplychainResult.Pouch;

                        item.RemarkDisplay = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("\n", "<br/>");

                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Sachet1 = supplychainResult.Sachet1;
                        item.Sachet2 = supplychainResult.Sachet2;
                        item.Batch = supplychainResult.Batch;
                        item.FRRemark = supplychainResult.FRRemark;
                        item.BottleRemark = supplychainResult.BottleRemark;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.SachetRemark = supplychainResult.SachetRemark;


                        await _supplyChainFPQ.UpdateAsync(supplychainResult);

                        if (item.ListUsernameInSupplyChainFPQ.Count == 0)
                        {
                            await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                            {
                                SupplyChainFpqId =
                                    supplychainResult.Id,
                                UserId = owner.Id
                            });
                        }

                    }
                    else
                    {
                        var yesterdayFPQ = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainFPQOwner =
                         _supplyChainFPQ.GetAllSupplyChainFPQs()
                             .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayFPQ).OrderBy(p => p.UpdatedDate)
                             .ToList();

                        if (allsupplyChainFPQOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainFPQ()
                            {
                                CreatedDate = dateSearch,
                                Owner = owner == null ? "" : owner.Username,
                                UpdatedDate = dateSearch,
                                MeasureCode = item.MeasureCode,
                                type = item.type,
                            };
                            await _supplyChainFPQ.CreateAsync(newModel);

                            await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                            {
                                SupplyChainFpqId =
                                    newModel.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainFPQ =
                                _supplyChainFPQ.GetUserNameInSupplyChainFPQ(newModel.Id);
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainFPQOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var newModel = new Entities.Domain.SupplyChainFPQ()
                                {
                                    CreatedDate = dateSearch,
                                    Owner = allsupplyChainFPQOwner.Last().Owner,
                                    UpdatedDate = dateSearch,
                                    MeasureCode = allsupplyChainFPQOwner.Last().MeasureCode,
                                    type = allsupplyChainFPQOwner.Last().type,
                                };

                                var listOldOwner =
                                    _supplyChainFPQ.GetUserIdInSupplyChainFPQ(allsupplyChainFPQOwner.Last().Id);

                                await _supplyChainFPQ.CreateAsync(newModel);


                                if (listOldOwner != null)
                                {

                                    foreach (var itemOwner in listOldOwner)
                                    {
                                        await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                                        {
                                            SupplyChainFpqId =
                                                newModel.Id,
                                            UserId = itemOwner
                                        });
                                    }
                                }
                                else
                                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                                    {
                                        SupplyChainFpqId = newModel.Id,
                                        UserId = owner.Id
                                    });

                                item.ListUsernameInSupplyChainFPQ =
                                    _supplyChainFPQ.GetUserNameInSupplyChainFPQ(newModel.Id);
                            }
                        }
                    }
                }

                model.SupplyChainFPQ = SupplyChainFPQ;

                #endregion

                #region Service


                var supplyChainService = new List<SupplyChainServiceModel>()
                                             {
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.MTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.MTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 }

                                             };
                var allsupplyChainFE =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 1).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainFE.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFE.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {

                        var newModel = new Entities.Domain.SupplyChainService()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            CFR = lastDayTarget.CFR,
                            SAMBC = lastDayTarget.SAMBC,
                            PrioritySKU = lastDayTarget.PrioritySKU,
                            PriorityLine = lastDayTarget.PriorityLine,
                            Shipment = lastDayTarget.Shipment,
                            type = 1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainService.CreateAsync(newModel);
                    }

                }

                var yesterdayService = dateSearch.AddDays(-1).AddHours(23);
                var allsupplyChainService =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 2 && p.CreatedDate <= yesterdayService).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainService.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    if (dateSearch > allsupplyChainService.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                    {
                        var lastDayTarget = allsupplyChainService.Last();
                        if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                        {


                            var newModel = new Entities.Domain.SupplyChainService()
                            {
                                CreatedDate = dateSearch,
                                Owner = lastDayTarget.Owner,
                                UpdatedDate = dateSearch,
                                MeasureCode = lastDayTarget.MeasureCode,
                                CFR = lastDayTarget.CFR,
                                SAMBC = lastDayTarget.SAMBC,
                                PrioritySKU = lastDayTarget.PrioritySKU,
                                PriorityLine = lastDayTarget.PriorityLine,
                                Shipment = lastDayTarget.Shipment,
                                type = 2,
                                UserCreatedId = currentUser.Id,
                                UserUpdatedId = currentUser.Id

                            };
                            await _supplyChainService.CreateAsync(newModel);
                        }
                    }
                }
                foreach (var item in supplyChainService)
                {
                    var supplychainResult =
                        _supplyChainService.GetSupplyChainServiceMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                           dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.Owner = supplychainResult.Owner;
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainServiceMeasure)supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.CFR = supplychainResult.CFR;
                        item.PriorityLine = supplychainResult.PriorityLine;
                        item.PrioritySKU = supplychainResult.PrioritySKU;
                        item.SAMBC = supplychainResult.SAMBC;
                        item.Shipment = supplychainResult.Shipment;
                        await _supplyChainService.UpdateAsync(supplychainResult);

                    }
                }

                model.SupplyChainService = supplyChainService;



                #endregion

                #region ProductionPlanning


                var SupplyChainProductionPlanning = new List<SupplyChainProductionPlanningModel>()
                                                        {
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD1
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD1,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD2
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                    
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD2,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD3
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD3,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.DSR
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.DSR,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FRMK3
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FRMK3,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FRMK4
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FRMK4,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Sachet
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Sachet,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Pouch
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Pouch,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Bottle
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Bottle,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FE
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FE,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =
                                                                    SupplyChainProductionPlanningMeasure.FR.ToString(),
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FR,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            }
                                                            ,
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Total
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Total,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            }

                                                        };

                foreach (var item in SupplyChainProductionPlanning)
                {
                    var supplychainResult =
                        _supplyChainProductionPlanning.GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.Shift1 = supplychainResult.Shift1;
                        item.Shift2 = supplychainResult.Shift2;
                        item.Shift3 = supplychainResult.Shift3;
                        item.TodayPlan = supplychainResult.TodayPlan;
                        item.MTD = supplychainResult.MTD;
                        item.MonthTarget = supplychainResult.MonthTarget;
                        item.Gap = supplychainResult.Gap;
                        item.RemarkDisplay = String.IsNullOrEmpty(supplychainResult.Remark) ? string.Empty : supplychainResult.Remark;
                        item.Remark = String.IsNullOrEmpty(supplychainResult.Remark) ? string.Empty : supplychainResult.Remark.Replace("<br/>", "\n");
                        await _supplyChainProductionPlanning.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        if (item.type == 1)
                        {
                            var allsupplyChainProductionPlanningOwner =
                                _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                    .Result.Where(p => p.MeasureCode == item.MeasureCode).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                                    .ToList();

                            if (allsupplyChainProductionPlanningOwner.Count == 0)
                            {

                                var newModel = new SupplyChainProductionPlanning()
                                {
                                    type = 1,
                                    Owner = owner == null ? "" : owner.Username,
                                    UserUpdatedId = currentUser.Id,
                                    CreatedDate = dateSearch,
                                    UpdatedDate = dateSearch,
                                    UserCreatedId = currentUser.Id,
                                    MeasureCode = item.MeasureCode,
                                    Remark = String.IsNullOrEmpty(item.Remark) ? String.Empty : item.Remark
                                };
                                await _supplyChainProductionPlanning.CreateAsync(newModel);
                                item.Owner = owner == null ? "" : owner.Username;
                            }
                            else
                            {
                                if (dateSearch > allsupplyChainProductionPlanningOwner.LastOrDefault().CreatedDate &&
                                    dateSearch < currentDate.AddDays(1))
                                {
                                    var lastDayTarget = allsupplyChainProductionPlanningOwner.Last();
                                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                    {


                                        var newModel = new SupplyChainProductionPlanning()
                                        {
                                            type = 1,
                                            Owner = lastDayTarget.Owner,
                                            CreatedDate = dateSearch,
                                            UpdatedDate = dateSearch,
                                            MeasureCode = lastDayTarget.MeasureCode,
                                            Remark = String.IsNullOrEmpty(item.Remark) ? String.Empty : item.Remark
                                        };
                                        await _supplyChainProductionPlanning.CreateAsync(newModel);
                                        item.Owner = lastDayTarget.Owner;
                                    }

                                }
                            }
                        }
                        else
                        {
                            var allsupplyChainProductionPlanning =
                                _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                    .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                    .ToList();

                            if (allsupplyChainProductionPlanning.Count == 0)
                            {

                                var newModel = new SupplyChainProductionPlanning()
                                {
                                    type = 2,
                                    MonthTarget = 0,
                                    UserUpdatedId = currentUser.Id,
                                    CreatedDate = dateSearch,
                                    UpdatedDate = dateSearch,
                                    UserCreatedId = currentUser.Id,
                                    MeasureCode = item.MeasureCode,

                                };
                                await _supplyChainProductionPlanning.CreateAsync(newModel);
                                item.MonthTarget = newModel.MonthTarget;
                            }
                            else
                            {
                                if (dateSearch > allsupplyChainProductionPlanning.LastOrDefault().CreatedDate &&
                                    dateSearch < currentDate.AddDays(1))
                                {
                                    var lastDayTarget = allsupplyChainProductionPlanning.Last();
                                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                                    {


                                        var newModel = new SupplyChainProductionPlanning()
                                        {
                                            type = 2,
                                            MonthTarget = lastDayTarget.MonthTarget,
                                            CreatedDate = dateSearch,
                                            UpdatedDate = dateSearch,
                                            MeasureCode = lastDayTarget.MeasureCode,

                                        };
                                        await _supplyChainProductionPlanning.CreateAsync(newModel);
                                        item.MonthTarget = newModel.MonthTarget;
                                    }

                                }
                            }
                        }
                    }
                }

                model.SupplyChainProductionPlanning = SupplyChainProductionPlanning;


                #endregion

                #region MPSA

                var supplyChainMPSA = new List<SupplyChainMPSAModel>();

                supplyChainMPSA = new List<SupplyChainMPSAModel>()
                                          {
                                              new SupplyChainMPSAModel()
                                              {

                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.Target.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int) SupplyChainMPSAMeasure.Target,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty

                                              },
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.TotalPO.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.TotalPO,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                         Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                         Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                   Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.DailyMPSA.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.DailyMPSA,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                       Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.MTDMPSA.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.MTDMPSA,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                          };
                var allsupplyChainMPSA =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainMPSAMeasure.Target).Where(p => p.CreatedDate <= dateSearch).OrderBy(p => p.UpdatedDate)
                        .ToList();

                if (allsupplyChainMPSA.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainMPSA()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainMPSAMeasure.Target,
                        MPSAFR = "PO Missed < 90% of total",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainMPSA.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMPSA.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainMPSA()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            MPSAFR = lastDayTarget.MPSAFR,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainMPSA.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainMPSA)
                {
                    var supplychainResult =
                        _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {
                        //item.MeasureName = ((SupplyChainMPSAMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.FR = supplychainResult.FR;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet1 = supplychainResult.Sachet1;
                        item.Sachet2 = supplychainResult.Sachet2;
                        item.Pouch = supplychainResult.Pouch;

                        item.RemarkDisplay = supplychainResult.Remark == null
                             ? ""
                             : supplychainResult.Remark.Replace("\n", "<br/>");

                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.OutputRemark = supplychainResult.OutputRemark;
                        item.MPSAFE = supplychainResult.MPSAFE;
                        item.MPSAFR = supplychainResult.MPSAFR;
                        item.BottleRemarks = supplychainResult.BottleRemarks;
                        item.FRRemarks = supplychainResult.FRRemarks;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.Sachet1Remarks = supplychainResult.Sachet1Remarks;
                        item.Sachet2Remarks = supplychainResult.Sachet2Remarks;
                        item.ListUsernameInSupplyChainMPSA =
                            _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainResult.Id);

                        if (item.ListUsernameInSupplyChainMPSA.Count == 0)
                        {

                            var listStringUsernameMPSA = new List<string>();
                            listStringUsernameMPSA.Add(owner.Username);
                            item.ListUsernameInSupplyChainMPSA = listStringUsernameMPSA;

                        }
                        await _supplyChainMPSA.UpdateAsync(supplychainResult);

                        if (item.ListUsernameInSupplyChainMPSA.Count == 0)
                        {
                            await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                            {
                                SupplyChainMPSAId =
                                    supplychainResult.Id,
                                UserId = owner.Id
                            });
                        }
                    }
                    else
                    {
                        var yesterdayMPSA = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainMPSAOwner =
                            _supplyChainMPSA.GetAllSupplyChainMPSAs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayMPSA).OrderBy(p => p.UpdatedDate)
                                .ToList();

                        if (allsupplyChainMPSAOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainMPSA()
                            {
                                CreatedDate = dateSearch,
                                Owner = owner == null ? "" : owner.Username,
                                UpdatedDate = dateSearch,
                                MeasureCode = item.MeasureCode,
                            };
                            await _supplyChainMPSA.CreateAsync(newModel);
                            var supplychainNew =
                                _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                                    dateSearch);
                            await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                            {
                                SupplyChainMPSAId = supplychainNew.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainMPSA =
                                _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainNew.Id);
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainMPSAOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var newModel = new Entities.Domain.SupplyChainMPSA()
                                {
                                    CreatedDate = dateSearch,
                                    Owner = allsupplyChainMPSAOwner.Last().Owner,
                                    UpdatedDate = dateSearch,
                                    MeasureCode = allsupplyChainMPSAOwner.Last().MeasureCode,
                                };

                                var listOldOwner =
                                    _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);
                                await _supplyChainMPSA.CreateAsync(newModel);
                                if (listOldOwner != null)
                                    foreach (var itemOwner in listOldOwner)
                                    {
                                        await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                                        {
                                            SupplyChainMPSAId =
                                                newModel.Id,
                                            UserId = itemOwner
                                        });
                                    }

                                else
                                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                                    {
                                        SupplyChainMPSAId =
                                            newModel.Id,
                                        UserId = owner.Id
                                    });

                                item.ListUsernameInSupplyChainMPSA =
                                    _supplyChainMPSA.GetUserNameInSupplyChainMPSA(newModel.Id);
                            }
                        }
                    }
                }

                model.SupplyChainMPSA = supplyChainMPSA;


                #endregion

                listModel.Add(model);
            }


            return View("SupplyChainReport", listModel);
        }

        public async Task<FileContentResult> ExportToExcel(string fromDate, string toDate)
        {
            DateTime fromDateSearch;
            if (String.IsNullOrEmpty(fromDate)) fromDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(fromDate, culture, styles, out fromDateSearch))
                {
                    fromDateSearch = DateTime.Now;
                }
            }

            DateTime toDateSearch;
            if (String.IsNullOrEmpty(toDate)) toDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(toDate, culture, styles, out toDateSearch))
                {
                    toDateSearch = DateTime.Now;
                }
            }

            var listModel = new List<SupplyChainModel>();



            for (var i = fromDateSearch; i <= toDateSearch; i = i.AddDays(1))
            {
                DateTime dateSearch = i;

                var model = new SupplyChainModel();

                #region HSE

                var allUser = _userService.GetAllUsersAsync();
                var owner = allUser.LastOrDefault();


                var currentUser = await _userService.GetUserByUsernameAsync("Admin");
                var supplyChainHSE = new List<SupplyChainHSEModel>();


                supplyChainHSE = new List<SupplyChainHSEModel>()
                                     {
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int) SupplyChainHSEMeasure.Target,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty
                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.BOSCompletion
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure.BOSCompletion,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty

                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("HSE",SupplyChainHSEMeasure.BOSTopUnsafeBehaviour
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure
                                                 .BOSTopUnsafeBehaviour,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                                 RemarkDisplay = string.Empty
                                         }
                                     };
                var allsupplyChain =
                    _supplyChainHSE.GetAllSupplyChainHSEs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target)
                        .ToList();

                if (allsupplyChain.Count == 0)
                {
                    var newModel = new SupplyChainHSE()
                    {
                        Owner = owner == null ? "" : owner.Username,
                        UserUpdatedId = currentUser.Id,
                        CreatedDate = dateSearch,
                        UpdatedDate = dateSearch,
                        UserCreatedId = currentUser.Id,
                        MeasureCode = (int)SupplyChainHSEMeasure.Target,
                        Making = "Daily 1 LT to share the BOS Observation"

                    };
                    await _supplyChainHSE.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChain.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new SupplyChainHSE()
                        {
                            Owner = owner == null ? "" : owner.Username,
                            CreatedDate = dateSearch,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            Making = lastDayTarget.Making,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainHSE.CreateAsync(newModel);
                    }

                }
                var currentDate = DateTime.Now;
                foreach (var item in supplyChainHSE)
                {


                    var supplychainResult =
                        _supplyChainHSE.GetSupplyChainHSEMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {

                        item.MeasureCode = supplychainResult.MeasureCode;
                        item.Making = supplychainResult.Making;
                        //item.MeasureName = ((SupplyChainHSEMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.Packing = supplychainResult.Packing;
                        item.RemarkDisplay = supplychainResult.Remarks == null
                            ? ""
                            : supplychainResult.Remarks.Replace("\n", "<br/>");
                        item.Remarks = supplychainResult.Remarks == null
                            ? ""
                            : supplychainResult.Remarks.Replace("<br/>", "\n");
                        //item.CreatedUser = supplychainResult.CreatedUser.Username;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.CommonArea = supplychainResult.CommonArea;
                    }
                    else
                    {
                        var yesterdayHSE = DateTime.Now.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainHSEOwner =
                     _supplyChainHSE.GetAllSupplyChainHSEs()
                         .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayHSE)
                         .ToList();


                        if (allsupplyChainHSEOwner.Count == 0)
                        {

                            var newModel = new SupplyChainHSE()
                            {
                                Owner = owner == null ? "" : owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = dateSearch,
                                UpdatedDate = dateSearch,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,


                            };
                            await _supplyChainHSE.CreateAsync(newModel);
                            item.Owner = owner == null ? "" : owner.Username;
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainHSEOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var lastDayTarget = allsupplyChainHSEOwner.Last();
                                if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                {


                                    var newModel = new SupplyChainHSE()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = dateSearch,
                                        UpdatedDate = dateSearch,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                    };
                                    await _supplyChainHSE.CreateAsync(newModel);
                                    item.Owner = lastDayTarget.Owner;
                                }

                            }
                        }
                    }

                    //check owner


                }





                model.SupplyChainHSE = supplyChainHSE;
                #endregion

                #region DDS


                var supplyChainDDS = new List<SupplyChainDDSModel>()
                                         {
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             }

                                         };
                var allsupplyChainMaking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 1).OrderBy(p => p.Id)
                        .ToList();

                if (allsupplyChainMaking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        LPD1 = "90%",
                        LPD2 = "90%",
                        LPD3 = "90%",
                        Batch = "90%",
                        FRMK = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMaking.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {
                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 1,
                            LPD1 = lastDayTarget.LPD1,
                            LPD2 = lastDayTarget.LPD2,
                            LPD3 = lastDayTarget.LPD3,
                            Batch = lastDayTarget.Batch,
                            FRMK = lastDayTarget.FRMK,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                var allsupplyChainPacking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 2)
                        .ToList();

                if (allsupplyChainPacking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        Bottle = "90%",
                        Sachet = "90%",
                        Pouch = "90%",
                        FRPK = "90%",
                        FE = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainPacking.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            Owner = lastDayTarget.Owner,
                            CreatedDate = dateSearch,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 2,
                            Bottle = lastDayTarget.Bottle,
                            Sachet = lastDayTarget.Sachet,
                            Pouch = lastDayTarget.Pouch,
                            FRPK = lastDayTarget.FRPK,
                            FE = lastDayTarget.FE,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainDDS)
                {
                    var supplychainResult =
                        _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FRPK = supplychainResult.FRPK;
                        item.Pouch = supplychainResult.Pouch;
                        item.RemarkDisplay = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("\n", "<br/>");
                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.FRMK = supplychainResult.FRMK;
                        item.FE = supplychainResult.FE;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Batch = supplychainResult.Batch;

                        item.BottleRemark = supplychainResult.BottleRemark;
                        item.FRPKRemark = supplychainResult.FRPKRemark;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.SachetRemark = supplychainResult.SachetRemark;

                        await _supplyChainDDS.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        var yesterdayDDS = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainDDSOwner =
                        _supplyChainDDS.GetAllSupplyChainDDSs()
                            .Result.Where(p => p.MeasureCode == item.MeasureCode && p.type == item.type && p.CreatedDate <= yesterdayDDS)
                            .ToList();
                        if (allsupplyChainDDSOwner.Count == 0)
                        {

                            var newModel = new SupplyChainDDS()
                            {
                                Owner = owner == null ? "" : owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = dateSearch,
                                UpdatedDate = dateSearch,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,
                                type = item.type

                            };
                            await _supplyChainDDS.CreateAsync(newModel);
                            item.Owner = owner == null ? "" : owner.Username;
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainDDSOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var lastDayTarget = allsupplyChainDDSOwner.Last();
                                if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                                {


                                    var newModel = new SupplyChainDDS()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = dateSearch,
                                        UpdatedDate = dateSearch,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                        type = item.type
                                    };
                                    await _supplyChainDDS.CreateAsync(newModel);
                                    item.Owner = lastDayTarget.Owner;
                                }
                            }
                        }
                    }
                }


                model.SupplyChainDDS = supplyChainDDS;



                #endregion

                #region FPQ

                var SupplyChainFPQ = new List<SupplyChainFPQModel>()
                                         {
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.QuanlityBOS
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.QuanlityBOS,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventPending
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventPending,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2,
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                             }

                                         };
                var allsupplyChainFPQ =
                    _supplyChainFPQ.GetAllSupplyChainFPQs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainFPQMeasure.Target)
                        .ToList();

                if (allsupplyChainFPQ.Count == 0)
                {
                    var newModel = new SupplyChainFPQ()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainFPQMeasure.Target,
                        LPD1 = "Daily 1 LT to share the BOS Observation",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id

                    };
                    await _supplyChainFPQ.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFPQ.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainFPQ()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            LPD1 = lastDayTarget.LPD1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainFPQ.CreateAsync(newModel);
                    }

                }
                foreach (var item in SupplyChainFPQ)
                {
                    var supplychainResult =
                        _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                           dateSearch);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainFPQMeasure) supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.ListUsernameInSupplyChainFPQ =
                            _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainResult.Id);
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FR = supplychainResult.FR;
                        item.FRMK = supplychainResult.FRMK;
                        item.Pouch = supplychainResult.Pouch;

                        item.RemarkDisplay = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("\n", "<br/>");

                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Sachet1 = supplychainResult.Sachet1;
                        item.Sachet2 = supplychainResult.Sachet2;
                        item.Batch = supplychainResult.Batch;
                        item.FRRemark = supplychainResult.FRRemark;
                        item.BottleRemark = supplychainResult.BottleRemark;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.SachetRemark = supplychainResult.SachetRemark;


                        await _supplyChainFPQ.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        var yesterdayFPQ = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainFPQOwner =
                         _supplyChainFPQ.GetAllSupplyChainFPQs()
                             .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayFPQ)
                             .ToList();

                        if (allsupplyChainFPQOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainFPQ()
                            {
                                CreatedDate = dateSearch,
                                Owner = owner == null ? "" : owner.Username,
                                UpdatedDate = dateSearch,
                                MeasureCode = item.MeasureCode,
                                type = item.type,
                            };
                            await _supplyChainFPQ.CreateAsync(newModel);

                            await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                            {
                                SupplyChainFpqId = newModel.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainFPQ =
                                _supplyChainFPQ.GetUserNameInSupplyChainFPQ(newModel.Id);
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainFPQOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var newModel = new Entities.Domain.SupplyChainFPQ()
                                {
                                    CreatedDate = dateSearch,
                                    Owner = allsupplyChainFPQOwner.Last().Owner,
                                    UpdatedDate = dateSearch,
                                    MeasureCode = allsupplyChainFPQOwner.Last().MeasureCode,
                                    type = allsupplyChainFPQOwner.Last().type,
                                };

                                var listOldOwner =
                                    _supplyChainFPQ.GetUserIdInSupplyChainFPQ(allsupplyChainFPQOwner.Last().Id);
                                await _supplyChainFPQ.CreateAsync(newModel);

                                foreach (var itemOwner in listOldOwner)
                                {
                                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                                    {
                                        SupplyChainFpqId =
                                            newModel.Id,
                                        UserId = itemOwner
                                    });
                                }

                                item.ListUsernameInSupplyChainFPQ =
                                    _supplyChainFPQ.GetUserNameInSupplyChainFPQ(newModel.Id);
                            }
                        }
                    }
                }

                model.SupplyChainFPQ = SupplyChainFPQ;

                #endregion

                #region Service


                var supplyChainService = new List<SupplyChainServiceModel>()
                                             {
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.MTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = User.Identity.Name,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.MTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 }

                                             };
                var allsupplyChainFE =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 1)
                        .ToList();

                if (allsupplyChainFE.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFE.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {

                        var newModel = new Entities.Domain.SupplyChainService()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            CFR = lastDayTarget.CFR,
                            SAMBC = lastDayTarget.SAMBC,
                            PrioritySKU = lastDayTarget.PrioritySKU,
                            PriorityLine = lastDayTarget.PriorityLine,
                            Shipment = lastDayTarget.Shipment,
                            type = 1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainService.CreateAsync(newModel);
                    }

                }

                var yesterdayService = dateSearch.AddDays(-1).AddHours(23);
                var allsupplyChainService =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 2 && p.CreatedDate <= yesterdayService)
                        .ToList();

                if (allsupplyChainService.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    if (dateSearch > allsupplyChainService.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                    {
                        var lastDayTarget = allsupplyChainService.Last();
                        if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                        {


                            var newModel = new Entities.Domain.SupplyChainService()
                            {
                                CreatedDate = dateSearch,
                                Owner = lastDayTarget.Owner,
                                UpdatedDate = dateSearch,
                                MeasureCode = lastDayTarget.MeasureCode,
                                CFR = lastDayTarget.CFR,
                                SAMBC = lastDayTarget.SAMBC,
                                PrioritySKU = lastDayTarget.PrioritySKU,
                                PriorityLine = lastDayTarget.PriorityLine,
                                Shipment = lastDayTarget.Shipment,
                                type = 2,
                                UserCreatedId = currentUser.Id,
                                UserUpdatedId = currentUser.Id

                            };
                            await _supplyChainService.CreateAsync(newModel);
                        }
                    }
                }
                foreach (var item in supplyChainService)
                {
                    var supplychainResult =
                        _supplyChainService.GetSupplyChainServiceMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                           dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.Owner = supplychainResult.Owner;
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainServiceMeasure)supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.CFR = supplychainResult.CFR;
                        item.PriorityLine = supplychainResult.PriorityLine;
                        item.PrioritySKU = supplychainResult.PrioritySKU;
                        item.SAMBC = supplychainResult.SAMBC;
                        item.Shipment = supplychainResult.Shipment;
                        await _supplyChainService.UpdateAsync(supplychainResult);

                    }
                }

                model.SupplyChainService = supplyChainService;



                #endregion

                #region ProductionPlanning


                var SupplyChainProductionPlanning = new List<SupplyChainProductionPlanningModel>()
                                                        {
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD1
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD1,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD2
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                    
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD2,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.LPD3
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.LPD3,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.DSR
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.DSR,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FRMK3
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FRMK3,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FRMK4
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FRMK4,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 1,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Sachet
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Sachet,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Pouch
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Pouch,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Bottle
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Bottle,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.FE
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FE,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                RemarkDisplay = string.Empty
                                                            },
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =
                                                                    SupplyChainProductionPlanningMeasure.FR.ToString(),
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.FR,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            }
                                                            ,
                                                            new SupplyChainProductionPlanningModel()
                                                            {
                                                                CreatedUser = User.Identity.Name,
                                                                MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("ProductionPlanning",SupplyChainProductionPlanningMeasure.Total
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                                MeasureCode =
                                                                    (int) SupplyChainProductionPlanningMeasure.Total,
                                                                CreatedDate = dateSearch.ToShortDateString(),
                                                                UpdatedDate = dateSearch.ToShortDateString(),
                                                                type = 2,
                                                                Remark = string.Empty ,
                                                                RemarkDisplay = string.Empty
                                                            }

                                                        };

                foreach (var item in SupplyChainProductionPlanning)
                {
                    var supplychainResult =
                        _supplyChainProductionPlanning.GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.Shift1 = supplychainResult.Shift1;
                        item.Shift2 = supplychainResult.Shift2;
                        item.Shift3 = supplychainResult.Shift3;
                        item.TodayPlan = supplychainResult.TodayPlan;
                        item.MTD = supplychainResult.MTD;
                        item.MonthTarget = supplychainResult.MonthTarget;
                        item.Gap = supplychainResult.Gap;
                        item.RemarkDisplay = String.IsNullOrEmpty(supplychainResult.Remark) ? string.Empty : supplychainResult.Remark;
                        item.Remark = String.IsNullOrEmpty(supplychainResult.Remark) ? string.Empty : supplychainResult.Remark.Replace("<br/>", "\n");
                        await _supplyChainProductionPlanning.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        if (item.type == 1)
                        {
                            var allsupplyChainProductionPlanningOwner =
                                _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                    .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                    .ToList();

                            if (allsupplyChainProductionPlanningOwner.Count == 0)
                            {

                                var newModel = new SupplyChainProductionPlanning()
                                {
                                    type = 1,
                                    Owner = owner == null ? "" : owner.Username,
                                    UserUpdatedId = currentUser.Id,
                                    CreatedDate = dateSearch,
                                    UpdatedDate = dateSearch,
                                    UserCreatedId = currentUser.Id,
                                    MeasureCode = item.MeasureCode,
                                    Remark = String.IsNullOrEmpty(item.Remark) ? String.Empty : item.Remark
                                };
                                await _supplyChainProductionPlanning.CreateAsync(newModel);
                                item.Owner = owner == null ? "" : owner.Username;
                            }
                            else
                            {
                                if (dateSearch > allsupplyChainProductionPlanningOwner.LastOrDefault().CreatedDate &&
                                    dateSearch < currentDate.AddDays(1))
                                {
                                    var lastDayTarget = allsupplyChainProductionPlanningOwner.Last();
                                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                    {


                                        var newModel = new SupplyChainProductionPlanning()
                                        {
                                            type = 1,
                                            Owner = lastDayTarget.Owner,
                                            CreatedDate = dateSearch,
                                            UpdatedDate = dateSearch,
                                            MeasureCode = lastDayTarget.MeasureCode,
                                            Remark = String.IsNullOrEmpty(item.Remark) ? String.Empty : item.Remark
                                        };
                                        await _supplyChainProductionPlanning.CreateAsync(newModel);
                                        item.Owner = lastDayTarget.Owner;
                                    }

                                }
                            }
                        }
                        else
                        {
                            var allsupplyChainProductionPlanning =
                                _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                    .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                    .ToList();

                            if (allsupplyChainProductionPlanning.Count == 0)
                            {

                                var newModel = new SupplyChainProductionPlanning()
                                {
                                    type = 2,
                                    MonthTarget = 0,
                                    UserUpdatedId = currentUser.Id,
                                    CreatedDate = dateSearch,
                                    UpdatedDate = dateSearch,
                                    UserCreatedId = currentUser.Id,
                                    MeasureCode = item.MeasureCode,

                                };
                                await _supplyChainProductionPlanning.CreateAsync(newModel);
                                item.MonthTarget = newModel.MonthTarget;
                            }
                            else
                            {
                                if (dateSearch > allsupplyChainProductionPlanning.LastOrDefault().CreatedDate &&
                                    dateSearch < currentDate.AddDays(1))
                                {
                                    var lastDayTarget = allsupplyChainProductionPlanning.Last();
                                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                                    {


                                        var newModel = new SupplyChainProductionPlanning()
                                        {
                                            type = 2,
                                            MonthTarget = lastDayTarget.MonthTarget,
                                            CreatedDate = dateSearch,
                                            UpdatedDate = dateSearch,
                                            MeasureCode = lastDayTarget.MeasureCode,

                                        };
                                        await _supplyChainProductionPlanning.CreateAsync(newModel);
                                        item.MonthTarget = newModel.MonthTarget;
                                    }

                                }
                            }
                        }
                    }
                }

                model.SupplyChainProductionPlanning = SupplyChainProductionPlanning;


                #endregion

                #region MPSA

                var supplyChainMPSA = new List<SupplyChainMPSAModel>();

                supplyChainMPSA = new List<SupplyChainMPSAModel>()
                                          {
                                              new SupplyChainMPSAModel()
                                              {

                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.Target.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int) SupplyChainMPSAMeasure.Target,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty

                                              },
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.TotalPO.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.TotalPO,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                         Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                         Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                   Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.DailyMPSA.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.DailyMPSA,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                       Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                              ,
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = User.Identity.Name,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.MTDMPSA.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode =
                                                      (int)
                                                      SupplyChainMPSAMeasure.MTDMPSA,
                                                  CreatedDate =
                                                      dateSearch.ToShortDateString(),
                                                  UpdatedDate =
                                                      dateSearch.ToShortDateString(),
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              }
                                          };
                var allsupplyChainMPSA =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainMPSAMeasure.Target)
                        .ToList();

                if (allsupplyChainMPSA.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainMPSA()
                    {

                        CreatedDate = dateSearch,
                        Owner = owner == null ? "" : owner.Username,
                        UpdatedDate = dateSearch,
                        MeasureCode = (int)SupplyChainMPSAMeasure.Target,
                        MPSAFR = "PO Missed < 90% of total",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainMPSA.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMPSA.Last();
                    if (lastDayTarget.CreatedDate.Date != dateSearch.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainMPSA()
                        {
                            CreatedDate = dateSearch,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = dateSearch,
                            MeasureCode = lastDayTarget.MeasureCode,
                            MPSAFR = lastDayTarget.MPSAFR,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainMPSA.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainMPSA)
                {
                    var supplychainResult =
                        _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {
                        //item.MeasureName = ((SupplyChainMPSAMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.FR = supplychainResult.FR;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet1 = supplychainResult.Sachet1;
                        item.Sachet2 = supplychainResult.Sachet2;
                        item.Pouch = supplychainResult.Pouch;

                        item.RemarkDisplay = supplychainResult.Remark == null
                             ? ""
                             : supplychainResult.Remark.Replace("\n", "<br/>");

                        item.Remark = supplychainResult.Remark == null
                                ? ""
                                : supplychainResult.Remark.Replace("<br/>", "\n");
                        item.OutputRemark = supplychainResult.OutputRemark;
                        item.MPSAFE = supplychainResult.MPSAFE;
                        item.MPSAFR = supplychainResult.MPSAFR;
                        item.BottleRemarks = supplychainResult.BottleRemarks;
                        item.FRRemarks = supplychainResult.FRRemarks;
                        item.PouchRemark = supplychainResult.PouchRemark;
                        item.Sachet1Remarks = supplychainResult.Sachet1Remarks;
                        item.Sachet2Remarks = supplychainResult.Sachet2Remarks;
                        item.ListUsernameInSupplyChainMPSA =
                            _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainResult.Id);
                    }
                    else
                    {
                        var yesterdayMPSA = dateSearch.Date.AddDays(-1).AddHours(23);
                        var allsupplyChainMPSAOwner =
                            _supplyChainMPSA.GetAllSupplyChainMPSAs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode && p.CreatedDate <= yesterdayMPSA)
                                .ToList();

                        if (allsupplyChainMPSAOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainMPSA()
                            {
                                CreatedDate = dateSearch,
                                Owner = owner == null ? "" : owner.Username,
                                UpdatedDate = dateSearch,
                                MeasureCode = item.MeasureCode,
                            };
                            await _supplyChainMPSA.CreateAsync(newModel);
                            var supplychainNew =
                                _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                                    dateSearch);
                            await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                            {
                                SupplyChainMPSAId =
                                    supplychainNew.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainMPSA =
                                _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainNew.Id);
                        }
                        else
                        {
                            if (dateSearch > allsupplyChainMPSAOwner.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                            {
                                var newModel = new Entities.Domain.SupplyChainMPSA()
                                {
                                    CreatedDate = dateSearch,
                                    Owner = allsupplyChainMPSAOwner.Last().Owner,
                                    UpdatedDate = dateSearch,
                                    MeasureCode = allsupplyChainMPSAOwner.Last().MeasureCode,
                                };

                                var listOldOwner =
                                    _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);
                                await _supplyChainMPSA.CreateAsync(newModel);

                                foreach (var itemOwner in listOldOwner)
                                {
                                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                                    {
                                        SupplyChainMPSAId =
                                            newModel.Id,
                                        UserId = itemOwner
                                    });
                                }

                                item.ListUsernameInSupplyChainMPSA =
                                    _supplyChainMPSA.GetUserNameInSupplyChainMPSA(newModel.Id);
                            }
                        }
                    }
                }

                model.SupplyChainMPSA = supplyChainMPSA;

                #endregion

                listModel.Add(model);
            }



            #region export excell

            try
            {
                var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

                var r = new Random();
                int u = r.Next(10000);

                var filename = "report-export-" + u + "-" + currentTime + "-" + ".xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    ExportSupplyChainToXlsx(stream, listModel, "");
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception exc)
            {


            }

            #endregion

            return null;
        }
        
        public void ExportSupplyChainToXlsx(Stream stream, List<SupplyChainModel> listModel, string path)
        {

            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                #region HSE

                var worksheetHSE = xlPackage.Workbook.Worksheets.Add("HSE");
                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Merge = true;
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Value = listModel[i].SupplyChainHSE[0].CreatedDate;
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetHSE.Cells[1, i * 3 + 4, 1, i * 3 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header

                worksheetHSE.Cells["A1:A2"].Merge = true;
                worksheetHSE.Cells["A1:A2"].Value = "DMS";
                worksheetHSE.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["B1:B2"].Merge = true;
                worksheetHSE.Cells["B1:B2"].Value = "Measure";
                worksheetHSE.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["C1:C2"].Merge = true;
                worksheetHSE.Cells["C1:C2"].Value = "Owner";
                worksheetHSE.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["D2"].Value = "Making";
                worksheetHSE.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["E2"].Value = "Packing";
                worksheetHSE.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["F2"].Value = "Common Area";
                worksheetHSE.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["G2"].Value = "Remarks";
                worksheetHSE.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetHSE.Cells["G2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetHSE.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetHSE.Cells["A3:A6"].Merge = true;
                worksheetHSE.Cells["A3:A6"].Value = "HS&E";
                worksheetHSE.Cells["A3:A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["A3:A6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["A3:A6"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetHSE.Cells["A3:A6"].Style.Font.Color.SetColor(Color.White); // set color fon

                #endregion

                #region create content

                // target
                worksheetHSE.Cells["B3"].Value = "Target";
                worksheetHSE.Cells["B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["B3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color

                worksheetHSE.Cells["C3"].Value = "";
                worksheetHSE.Cells["C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["C3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color

                worksheetHSE.Cells["D3:F3"].Merge = true;
                worksheetHSE.Cells["D3:F3"].Value = listModel[0].SupplyChainHSE[0].Making;
                worksheetHSE.Cells["D3:F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["D3:F3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["D3:F3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color


                worksheetHSE.Cells["G3"].Value = listModel[0].SupplyChainHSE[0].Remarks;
                worksheetHSE.Cells["G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetHSE.Cells["G3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetHSE.Cells["G3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color



                // BOS-Completetion 
                worksheetHSE.Cells["B4:B5"].Merge = true;
                worksheetHSE.Cells["B4:B5"].Value = listModel[0].SupplyChainHSE[1].MeasureName;

                worksheetHSE.Cells["C4:C5"].Value = listModel[0].SupplyChainHSE[1].Owner;

                worksheetHSE.Cells["D4:F4"].Merge = true;
                worksheetHSE.Cells["D4:F4"].Value = "BOS today: " + listModel[0].SupplyChainHSE[1].Making;

                worksheetHSE.Cells["D5:F5"].Merge = true;
                worksheetHSE.Cells["D5:F5"].Value = "BOS done yesterday: " + listModel[0].SupplyChainHSE[1].Packing;

                worksheetHSE.Cells["G4:G5"].Value = listModel[0].SupplyChainHSE[1].Remarks;



                // BOS-Top Unsafe Behaviour 

                worksheetHSE.Cells["B6"].Value = listModel[0].SupplyChainHSE[2].MeasureName;

                worksheetHSE.Cells["C6"].Value = listModel[0].SupplyChainHSE[2].Owner;

                worksheetHSE.Cells["D6"].Value = listModel[0].SupplyChainHSE[2].Making;

                worksheetHSE.Cells["E6"].Value = listModel[0].SupplyChainHSE[2].Packing;

                worksheetHSE.Cells["F6"].Value = listModel[0].SupplyChainHSE[2].CommonArea;

                worksheetHSE.Cells["G6"].Value = listModel[0].SupplyChainHSE[2].Remarks;

                #endregion

                #region set width

                worksheetHSE.Cells["A2:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetHSE.Cells["B3:B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetHSE.Column(2).Width = 30;
                worksheetHSE.Column(3).Width = 20;
                worksheetHSE.Column(4).Width = 20;
                worksheetHSE.Column(5).Width = 20;
                worksheetHSE.Column(6).Width = 20;
                worksheetHSE.Column(7).Width = 20;
                #endregion

                for (int i = 1; i < listModel.Count; i++)
                {
                    #region create header

                    worksheetHSE.Cells[2, i * 3 + 4].Value = "Making";
                    worksheetHSE.Cells[2, i * 3 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetHSE.Cells[2, i * 3 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetHSE.Cells[2, i * 3 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetHSE.Cells[2, i * 3 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetHSE.Cells[2, i * 3 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetHSE.Cells[2, i * 3 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetHSE.Cells[2, i * 3 + 5].Value = "Packing";
                    worksheetHSE.Cells[2, i * 3 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetHSE.Cells[2, i * 3 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetHSE.Cells[2, i * 3 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetHSE.Cells[2, i * 3 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetHSE.Cells[2, i * 3 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetHSE.Cells[2, i * 3 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetHSE.Cells[2, i * 3 + 6].Value = "Common Area";
                    worksheetHSE.Cells[2, i * 3 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetHSE.Cells[2, i * 3 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetHSE.Cells[2, i * 3 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetHSE.Cells[2, i * 3 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetHSE.Cells[2, i * 3 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetHSE.Cells[2, i * 3 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    #endregion

                    #region create content


                    worksheetHSE.Cells[3, i * 3 + 4, 3, i * 3 + 6].Merge = true;
                    worksheetHSE.Cells[3, i * 3 + 4, 3, i * 3 + 6].Value = listModel[i].SupplyChainHSE[0].Making;
                    worksheetHSE.Cells[3, i * 3 + 4, 3, i * 3 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetHSE.Cells[3, i * 3 + 4, 3, i * 3 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetHSE.Cells[3, i * 3 + 4, 3, i * 3 + 6].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color



                    // BOS-Completetion 

                    worksheetHSE.Cells[4, i * 3 + 4, 4, i * 3 + 6].Merge = true;
                    worksheetHSE.Cells[4, i * 3 + 4, 4, i * 3 + 6].Value = "BOS today: " + listModel[i].SupplyChainHSE[1].Making;

                    worksheetHSE.Cells[5, i * 3 + 4, 5, i * 3 + 6].Merge = true;
                    worksheetHSE.Cells[5, i * 3 + 4, 5, i * 3 + 6].Value = "BOS done yesterday: " + listModel[i].SupplyChainHSE[1].Packing;



                    // BOS-Top Unsafe Behaviour 


                    worksheetHSE.Cells[6, i * 3 + 4].Value = listModel[i].SupplyChainHSE[2].Making;

                    worksheetHSE.Cells[6, i * 3 + 5].Value = listModel[i].SupplyChainHSE[2].Packing;

                    worksheetHSE.Cells[6, i * 3 + 6].Value = listModel[i].SupplyChainHSE[2].CommonArea;



                    #endregion

                    #region set width
                    worksheetHSE.Column(i * 3 + 4).Width = 20;
                    worksheetHSE.Column(i * 3 + 5).Width = 20;
                    worksheetHSE.Column(i * 3 + 6).Width = 20;
                    worksheetHSE.Cells[1, 1, 6, 3 * listModel.Count + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                    #endregion

                }

                #endregion

                #region DDS

                var worksheetDDS = xlPackage.Workbook.Worksheets.Add("DDS");

                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Merge = true;
                
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Value = listModel[i].SupplyChainDDS[0].CreatedDate;
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header

                worksheetDDS.Cells["A1:A2"].Merge = true;
                worksheetDDS.Cells["A1:A2"].Value = "DMS";
                worksheetDDS.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetDDS.Cells["B1:B2"].Merge = true;
                worksheetDDS.Cells["B1:B2"].Value = "Measure-Making";
                worksheetDDS.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["C1:C2"].Merge = true;
                worksheetDDS.Cells["C1:C2"].Value = "Owner";
                worksheetDDS.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["C1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["D2"].Value = "LPD1";
                worksheetDDS.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["E2"].Value = "LPD2";
                worksheetDDS.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["F2"].Value = "LPD3";
                worksheetDDS.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["G2"].Value = "Bat";
                worksheetDDS.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["G2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["H2"].Value = "FR (MK)";
                worksheetDDS.Cells["H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["H2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["H2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["I2"].Value = "Remark";
                worksheetDDS.Cells["I2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["I2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["I2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetDDS.Cells["A3:A9"].Merge = true;
                worksheetDDS.Cells["A3:A9"].Value = "DDS";
                worksheetDDS.Cells["A3:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Orange); // set border color
                worksheetDDS.Cells["A3:A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["A3:A9"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetDDS.Cells["A3:A9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["A3:A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Cells["A3:A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                #endregion


                #region create content


                for (int i = 0; i < 3; i++)
                {
                    int colum = i + 3;
                    worksheetDDS.Cells["B" + colum].Value = listModel[0].SupplyChainDDS[i].MeasureName;
                    worksheetDDS.Cells["C" + colum].Value = listModel[0].SupplyChainDDS[i].Owner;
                    worksheetDDS.Cells["D" + colum].Value = listModel[0].SupplyChainDDS[i].LPD1;
                    worksheetDDS.Cells["E" + colum].Value = listModel[0].SupplyChainDDS[i].LPD2;
                    worksheetDDS.Cells["F" + colum].Value = listModel[0].SupplyChainDDS[i].LPD3;
                    worksheetDDS.Cells["G" + colum].Value = listModel[0].SupplyChainDDS[i].Batch;
                    worksheetDDS.Cells["H" + colum].Value = listModel[0].SupplyChainDDS[i].FRMK;
                    worksheetDDS.Cells["I" + colum].Value = listModel[0].SupplyChainDDS[i].Remark;
                }
              

                worksheetDDS.Cells["B6"].Value = "Measure-Packing";
                worksheetDDS.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["B6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["B6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["B6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["C6"].Value = "Owner";
                worksheetDDS.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["C6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["C6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["C6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["D6"].Value = "Bottle";
                worksheetDDS.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["D6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["D6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["D6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["E6"].Value = "Sachet";
                worksheetDDS.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["E6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["E6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["E6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["F6"].Value = "Pouch";
                worksheetDDS.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["F6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["F6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["F6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["G6"].Value = "FR (PK)";
                worksheetDDS.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["G6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["G6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["G6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["H6"].Value = "FE";
                worksheetDDS.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["H6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["H6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["H6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["I6"].Value = "Remarks";
                worksheetDDS.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetDDS.Cells["I6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["I6"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetDDS.Cells["I6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetDDS.Cells["B3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetDDS.Cells["B3:I3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                for (int j = 3; j < 6; j++)
                {
                    int colum = j + 4;
                    worksheetDDS.Cells["B" + colum].Value = listModel[0].SupplyChainDDS[j].MeasureName;
                    worksheetDDS.Cells["C" + colum].Value = listModel[0].SupplyChainDDS[j].Owner;
                    worksheetDDS.Cells["D" + colum].Value = listModel[0].SupplyChainDDS[j].Bottle;
                    worksheetDDS.Cells["E" + colum].Value = listModel[0].SupplyChainDDS[j].Sachet;
                    worksheetDDS.Cells["F" + colum].Value = listModel[0].SupplyChainDDS[j].Pouch;
                    worksheetDDS.Cells["G" + colum].Value = listModel[0].SupplyChainDDS[j].FRPK;
                    worksheetDDS.Cells["H" + colum].Value = listModel[0].SupplyChainDDS[j].FE;
                    worksheetDDS.Cells["I" + colum].Value = listModel[0].SupplyChainDDS[j].Remark;
                }

                #endregion

                #region set width
                worksheetDDS.Cells["A3:A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetDDS.Column(2).Width = 25;
                worksheetDDS.Column(3).Width = 20;
                worksheetDDS.Column(2).Width = 30;
                worksheetDDS.Column(3).Width = 20;
                worksheetDDS.Column(4).Width = 20;
                worksheetDDS.Column(5).Width = 20;
                worksheetDDS.Column(6).Width = 20;
                worksheetDDS.Column(7).Width = 20;
                worksheetDDS.Column(8).Width = 20;
                worksheetDDS.Column(9).Width = 20;
                #endregion

                for (int c = 1; c < listModel.Count; c++)
                {
                    #region create header

                    worksheetDDS.Cells[2, c * 5 + 4].Value = "LPD1";
                    worksheetDDS.Cells[2, c * 5 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[2, c * 5 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[2, c * 5 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[2, c * 5 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[2, c * 5 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[2, c * 5 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetDDS.Cells[2, c * 5 + 5].Value = "LPD2";
                    worksheetDDS.Cells[2, c * 5 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[2, c * 5 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[2, c * 5 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[2, c * 5 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[2, c * 5 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[2, c * 5 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetDDS.Cells[2, c * 5 + 6].Value = "LPD3";
                    worksheetDDS.Cells[2, c * 5 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[2, c * 5 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[2, c * 5 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[2, c * 5 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[2, c * 5 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[2, c * 5 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetDDS.Cells[2, c * 5 + 7].Value = "Bat";
                    worksheetDDS.Cells[2, c * 5 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[2, c * 5 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[2, c * 5 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[2, c * 5 + 7].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[2, c * 5 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[2, c * 5 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetDDS.Cells[2, c * 5 + 8].Value = "FR (MK)";
                    worksheetDDS.Cells[2, c * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[2, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[2, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[2, c * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetDDS.Cells[2, c * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetDDS.Cells[2, c * 5 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion


                    #region create content


                    for (int i = 0; i < 3; i++)
                    {
                        int colum = i + 3;

                        worksheetDDS.Cells[colum, c * 5 + 4].Value = listModel[c].SupplyChainDDS[i].LPD1;
                        worksheetDDS.Cells[colum, c * 5 + 5].Value = listModel[c].SupplyChainDDS[i].LPD2;
                        worksheetDDS.Cells[colum, c * 5 + 6].Value = listModel[c].SupplyChainDDS[i].LPD3;
                        worksheetDDS.Cells[colum, c * 5 + 7].Value = listModel[c].SupplyChainDDS[i].Batch;
                        worksheetDDS.Cells[colum, c * 5 + 8].Value = listModel[c].SupplyChainDDS[i].FRMK;

                    }

                    worksheetDDS.Cells[6, c * 5 + 4].Value = "Bottle";
                    worksheetDDS.Cells[6, c * 5 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[6, c * 5 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[6, c * 5 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[6, c * 5 + 4].Style.Font.Color.SetColor(Color.White); // set color font

                    worksheetDDS.Cells[6, c * 5 + 5].Value = "Sachet";

                    worksheetDDS.Cells[6, c * 5 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[6, c * 5 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[6, c * 5 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[6, c * 5 + 5].Style.Font.Color.SetColor(Color.White); // set color font

                    worksheetDDS.Cells[6, c * 5 + 6].Value = "Pouch";

                    worksheetDDS.Cells[6, c * 5 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[6, c * 5 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[6, c * 5 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[6, c * 5 + 6].Style.Font.Color.SetColor(Color.White); // set color font

                    worksheetDDS.Cells[6, c * 5 + 7].Value = "FR (PK)";

                    worksheetDDS.Cells[6, c * 5 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[6, c * 5 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[6, c * 5 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[6, c * 5 + 7].Style.Font.Color.SetColor(Color.White); // set color font

                    worksheetDDS.Cells[6, c * 5 + 8].Value = "FE";

                    worksheetDDS.Cells[6, c * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetDDS.Cells[6, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[6, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetDDS.Cells[6, c * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font

                    worksheetDDS.Cells["B7:I7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells["B7:I7"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    for (int j = 3; j < 6; j++)
                    {
                        int colum = j + 4;

                        worksheetDDS.Cells[colum, c * 5 + 4].Value = listModel[c].SupplyChainDDS[j].Bottle;
                        worksheetDDS.Cells[colum, c * 5 + 5].Value = listModel[c].SupplyChainDDS[j].Sachet;
                        worksheetDDS.Cells[colum, c * 5 + 6].Value = listModel[c].SupplyChainDDS[j].Pouch;
                        worksheetDDS.Cells[colum, c * 5 + 7].Value = listModel[c].SupplyChainDDS[j].FRPK;
                        worksheetDDS.Cells[colum, c * 5 + 8].Value = listModel[c].SupplyChainDDS[j].FE;

                    }


                    #endregion
                }

                #region set width
                worksheetDDS.Cells["A3:A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                for (int c = 1; c < listModel.Count; c++)
                {
                    worksheetDDS.Column(c * 5 + 4).Width = 20;
                    worksheetDDS.Column(c * 5 + 5).Width = 20;
                    worksheetDDS.Column(c * 5 + 6).Width = 20;
                    worksheetDDS.Column(c * 5 + 7).Width = 20;
                    worksheetDDS.Column(c * 5 + 8).Width = 20;

                    worksheetDDS.Cells[3, c * 5 + 4, 3, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[3, c * 5 + 4, 3, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    worksheetDDS.Cells[7, c * 5 + 4, 7, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDDS.Cells[7, c * 5 + 4, 7, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    worksheetDDS.Cells[1, 1, 9, 5 * listModel.Count + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 
                }

                #endregion


                #endregion

                #region FPQ

                var worksheetFPQ = xlPackage.Workbook.Worksheets.Add("FPQ");

                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Merge = true;
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Value = listModel[i].SupplyChainFPQ[0].CreatedDate;
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[1, i * 9 + 4, 1, i * 9 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header

                worksheetFPQ.Cells["A1:A2"].Merge = true;
                worksheetFPQ.Cells["A1:A2"].Value = "DMS";
                worksheetFPQ.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetFPQ.Cells["B1:B2"].Merge = true;
                worksheetFPQ.Cells["B1:B2"].Value = "Measure";
                worksheetFPQ.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetFPQ.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["C1:C2"].Merge = true;
                worksheetFPQ.Cells["C1:C2"].Value = "Owner";
                worksheetFPQ.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetFPQ.Cells["C1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetFPQ.Cells["D2"].Value = "FR(MK)";
                worksheetFPQ.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["E2"].Value = "LPD1";
                worksheetFPQ.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["F2"].Value = "LPD2";
                worksheetFPQ.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["G2"].Value = "LPD3";
                worksheetFPQ.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["G2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["H2"].Value = "Batch";
                worksheetFPQ.Cells["H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["H2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["H2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["I2"].Value = "FR(Pk)";
                worksheetFPQ.Cells["I2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["I2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["I2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["J2"].Value = "Bottle";
                worksheetFPQ.Cells["J2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["J2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["J2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["J2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["K2"].Value = "Sachet";
                worksheetFPQ.Cells["K2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["K2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["K2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["K2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["K2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["L2"].Value = "Pouch";
                worksheetFPQ.Cells["L2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["L2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["L2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["L2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["L2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["M2"].Value = "Remark";
                worksheetFPQ.Cells["M2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetFPQ.Cells["M2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["M2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetFPQ.Cells["M2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetFPQ.Cells["A3:A7"].Merge = true;
                worksheetFPQ.Cells["A3:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["A3:A7"].Value = "FPQ";
                worksheetFPQ.Cells["A3:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Orange); // set border color
                worksheetFPQ.Cells["A3:A7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["A3:A7"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetFPQ.Cells["A3:A7"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetFPQ.Cells["A3:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetFPQ.Cells["A3:A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                #endregion


                #region create content

                worksheetFPQ.Cells["B3"].Value = listModel[0].SupplyChainFPQ[0].MeasureName;
                worksheetFPQ.Cells["D3:k3"].Merge = true;
                worksheetFPQ.Cells["D3:k3"].Value = listModel[0].SupplyChainFPQ[0].LPD1;
                worksheetFPQ.Cells["B3:k3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetFPQ.Cells["B3:k3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                for (int i = 1; i < 5; i++)
                {
                    var listUser = "";
                    listUser = String.Join(",", listModel[0].SupplyChainFPQ[i].ListUsernameInSupplyChainFPQ);
                    int colum = i + 3;
                    worksheetFPQ.Cells["B" + colum].Value = listModel[0].SupplyChainFPQ[i].MeasureName;
                    worksheetFPQ.Cells["C" + colum].Value = listUser;
                    worksheetFPQ.Cells["D" + colum].Value = listModel[0].SupplyChainFPQ[i].FRMK;
                    worksheetFPQ.Cells["E" + colum].Value = listModel[0].SupplyChainFPQ[i].LPD1;
                    worksheetFPQ.Cells["F" + colum].Value = listModel[0].SupplyChainFPQ[i].LPD2;
                    worksheetFPQ.Cells["G" + colum].Value = listModel[0].SupplyChainFPQ[i].LPD3;
                    worksheetFPQ.Cells["H" + colum].Value = listModel[0].SupplyChainFPQ[i].Batch;
                    worksheetFPQ.Cells["I" + colum].Value = listModel[0].SupplyChainFPQ[i].FR;
                    worksheetFPQ.Cells["J" + colum].Value = listModel[0].SupplyChainFPQ[i].Bottle;
                    worksheetFPQ.Cells["K" + colum].Value = listModel[0].SupplyChainFPQ[i].Sachet;
                    worksheetFPQ.Cells["L" + colum].Value = listModel[0].SupplyChainFPQ[i].Pouch;
                    worksheetFPQ.Cells["M" + colum].Value = listModel[0].SupplyChainFPQ[i].Remark;
                }


                #endregion


                #region set width

                worksheetFPQ.Column(2).Width = 30;
                worksheetFPQ.Column(3).Width = 20;
                worksheetFPQ.Column(4).Width = 20;
                worksheetFPQ.Column(5).Width = 20;
                worksheetFPQ.Column(6).Width = 20;
                worksheetFPQ.Column(7).Width = 20;
                worksheetFPQ.Column(8).Width = 20;
                worksheetFPQ.Column(9).Width = 20;
                worksheetFPQ.Column(10).Width = 20;
                worksheetFPQ.Column(11).Width = 20;
                worksheetFPQ.Column(12).Width = 20;
                worksheetFPQ.Column(13).Width = 20;
                #endregion


                for (int c = 1; c < listModel.Count; c++)
                {
                    #region create header

                    worksheetFPQ.Cells[2, c * 9 + 4].Value = "FR ( MK )";
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetFPQ.Cells[2, c * 9 + 5].Value = "LPD1";
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetFPQ.Cells[2, c * 9 + 6].Value = "LPD2";
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetFPQ.Cells[2, c * 9 + 7].Value = "LPD3";
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetFPQ.Cells[2, c * 9 + 8].Value = "Batch";
                    worksheetFPQ.Cells[2, c * 9 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9+ 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetFPQ.Cells[2, c * 9 + 9].Value = "FR ( PK )";
                    worksheetFPQ.Cells[2, c * 9 + 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 9].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 9].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetHSE.Cells[2, c * 9 + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetHSE.Cells[2, c * 9 + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetFPQ.Cells[2, c * 9 + 10].Value = "Bottle";
                    worksheetFPQ.Cells[2, c * 9 + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9+ 10].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 10].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetFPQ.Cells[2, c * 9 + 11].Value = "Sachet";
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetFPQ.Cells[2, c * 9 + 12].Value = "Pouch";
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetFPQ.Cells[2, c * 9 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion


                    #region create content


                    worksheetFPQ.Cells[3, c * 9 + 4, 3, c * 9 + 12].Merge = true;
                    worksheetFPQ.Cells[3, c * 9 + 4, 3, c * 9 + 12].Value = listModel[c].SupplyChainFPQ[0].LPD1;
                    worksheetFPQ.Cells[3, c * 9+ 4, 3, c * 9 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetFPQ.Cells[3, c * 9 + 4, 3, c * 9 + 12].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                    for (int i = 1; i < 5; i++)
                    {
                      
                        int colum = i + 3;
                        worksheetFPQ.Cells[colum, c * 9 + 4].Value = listModel[c].SupplyChainFPQ[i].FRMK;
                        worksheetFPQ.Cells[colum, c * 9 + 5].Value = listModel[c].SupplyChainFPQ[i].LPD1;
                        worksheetFPQ.Cells[colum, c * 9 + 6].Value = listModel[c].SupplyChainFPQ[i].LPD2;
                        worksheetFPQ.Cells[colum, c * 9 + 7].Value = listModel[c].SupplyChainFPQ[i].LPD3;
                        worksheetFPQ.Cells[colum, c * 9 + 8].Value = listModel[c].SupplyChainFPQ[i].Batch;
                        worksheetFPQ.Cells[colum, c * 9 + 9].Value = listModel[c].SupplyChainFPQ[i].FR;
                        worksheetFPQ.Cells[colum, c * 9 + 10].Value = listModel[c].SupplyChainFPQ[i].Bottle;
                        worksheetFPQ.Cells[colum, c * 9 + 11].Value = listModel[c].SupplyChainFPQ[i].Sachet;
                        worksheetFPQ.Cells[colum, c * 9 + 12].Value = listModel[c].SupplyChainFPQ[i].Pouch;

                    }


                    #endregion


                    #region set width

                    worksheetFPQ.Column(c * 9 + 4).Width = 20;
                    worksheetFPQ.Column(c * 9 + 5).Width = 20;
                    worksheetFPQ.Column(c * 9 + 6).Width = 20;
                    worksheetFPQ.Column(c * 9 + 7).Width = 20;
                    worksheetFPQ.Column(c * 9 + 8).Width = 20;
                    worksheetFPQ.Column(c * 9+ 9).Width = 20;
                    worksheetFPQ.Column(c * 9 + 10).Width = 20;
                    worksheetFPQ.Column(c * 9 + 11).Width = 20;
                    worksheetFPQ.Column(c * 9 + 12).Width = 20;
                    worksheetFPQ.Cells[1, 1, 7, 9 * listModel.Count + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 

                    #endregion
                }
                #endregion

                #region MPSA

                var worksheetMPSA = xlPackage.Workbook.Worksheets.Add("MPSA");


                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Merge = true;
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Value = listModel[i].SupplyChainFPQ[0].CreatedDate;
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[1, i * 5 + 4, 1, i * 5 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header

                worksheetMPSA.Cells["A1:A2"].Merge = true;

                worksheetMPSA.Cells["A1:A2"].Value = "DMS";
                worksheetMPSA.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetMPSA.Cells["A3:A10"].Merge = true;
                worksheetMPSA.Cells["A3:A10"].Value = "MPSA";
                worksheetMPSA.Cells["A3:A10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["A3:A10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["A3:A10"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetMPSA.Cells["A3:A10"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["A3:A10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["A3:A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["B1:B2"].Merge = true;
                worksheetMPSA.Cells["B1:B2"].Value = "Measure-Making";
                worksheetMPSA.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetMPSA.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetMPSA.Cells["B8"].Value = "Output Measure";
                worksheetMPSA.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["B8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["B8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["B8"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["C1:C2"].Merge = true;
                worksheetMPSA.Cells["C1:C2"].Value = "Owner";
                worksheetMPSA.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetMPSA.Cells["C1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetMPSA.Cells["C8"].Value = "Owner";
                worksheetMPSA.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["C8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["C8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["C8"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["D2"].Value = "FR";
                worksheetMPSA.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["D8"].Value = "MPSA-FR";
                worksheetMPSA.Cells["D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["D8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["D8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["D8"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["E8:H8"].Merge = true;
                worksheetMPSA.Cells["E8:H8"].Value = "MPSA-FE";
                worksheetMPSA.Cells["E8:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["E8:H8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["E8:H8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["E8:H8"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["E8:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["E8:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["E2"].Value = "Bottle";
                worksheetMPSA.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["F2"].Value = "Sachet2";
                worksheetMPSA.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["G2"].Value = "Sachet2";
                worksheetMPSA.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["G2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMPSA.Cells["H2"].Value = "Pouch";
                worksheetMPSA.Cells["H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetMPSA.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["H2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetMPSA.Cells["H2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetMPSA.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

              
                #endregion


                #region create content

                worksheetMPSA.Cells["B3"].Value = listModel[0].SupplyChainMPSA[0].MeasureName;
                worksheetMPSA.Cells["D3:H3"].Merge = true;
                worksheetMPSA.Cells["D3:H3"].Value = listModel[0].SupplyChainMPSA[0].MPSAFR;
                worksheetMPSA.Cells["B3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetMPSA.Cells["B3:I3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                worksheetMPSA.Cells["I3"].Value = listModel[0].SupplyChainMPSA[0].Remark;

                for (int i = 1; i < 5; i++)
                {
                    int colum = i + 3;
                    var listUser = "";
                    listUser = String.Join(",", listModel[0].SupplyChainMPSA[i].ListUsernameInSupplyChainMPSA);
                    worksheetMPSA.Cells["B" + colum].Value = listModel[0].SupplyChainMPSA[i].MeasureName;
                    worksheetMPSA.Cells["C" + colum].Value = listUser;
                    worksheetMPSA.Cells["D" + colum].Value = listModel[0].SupplyChainMPSA[i].FR;
                    worksheetMPSA.Cells["E" + colum].Value = listModel[0].SupplyChainMPSA[i].Bottle;
                    worksheetMPSA.Cells["F" + colum].Value = listModel[0].SupplyChainMPSA[i].Sachet1;
                    worksheetMPSA.Cells["G" + colum].Value = listModel[0].SupplyChainMPSA[i].Sachet2;
                    worksheetMPSA.Cells["H" + colum].Value = listModel[0].SupplyChainMPSA[i].Pouch;


                }
                for (int i = 5; i < 7; i++)
                {
                    int colum = i + 4;
                    var listUser = "";
                    listUser = String.Join(",", listModel[0].SupplyChainMPSA[i].ListUsernameInSupplyChainMPSA);
                    worksheetMPSA.Cells["B" + colum].Value = listModel[0].SupplyChainMPSA[i].MeasureName;
                    worksheetMPSA.Cells["C" + colum].Value = listUser;
                    worksheetMPSA.Cells["D" + colum].Value = listModel[0].SupplyChainMPSA[i].MPSAFR;
                    worksheetMPSA.Cells["E" + colum + ":" + "H" + colum].Merge = true;
                    worksheetMPSA.Cells["E" + colum + ":" + "H" + colum].Value = listModel[0].SupplyChainMPSA[i].MPSAFE;

                }




                #endregion

                #region set width

                worksheetMPSA.Cells["A2:A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetMPSA.Column(2).Width = 40;
                worksheetMPSA.Column(3).Width = 15;
                worksheetMPSA.Column(4).Width = 20;
                worksheetMPSA.Column(5).Width = 20;
                worksheetMPSA.Column(6).Width = 20;
                worksheetMPSA.Column(7).Width = 20;
                worksheetMPSA.Column(8).Width = 20;
                worksheetMPSA.Column(9).Width = 20;
                #endregion

                for (int c = 1; c < listModel.Count; c++)
                {
                    #region create header

             
                    worksheetMPSA.Cells[2,c*5+4].Value = "FR";
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[8, c * 5 + 4].Value = "MPSA-FR";
                    worksheetMPSA.Cells[8, c * 5 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[8, c * 5 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[8, c * 5 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[8, c * 5 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Merge = true;
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Value = "MPSA-FE";
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[8, c * 5 + 5, 8, c * 5 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[2, c * 5 + 5].Value = "Bottle";
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[2, c * 5 + 6].Value = "Sachet2";
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[2, c * 5 + 7].Value = "Sachet2";
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetMPSA.Cells[2, c * 5 + 8].Value = "Pouch";
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetMPSA.Cells[2, c * 5 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    #endregion


                    #region create content


                    worksheetMPSA.Cells[3, c * 5 + 4, 3, c * 5 + 8].Merge = true;
                    worksheetMPSA.Cells[3, c * 5 + 4, 3, c * 5 + 8].Value = listModel[0].SupplyChainMPSA[0].MPSAFR;
                    worksheetMPSA.Cells[3, c * 5 + 4, 3, c * 5 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetMPSA.Cells[3, c * 5 + 4, 3, c * 5 + 8].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                  

                    for (int i = 1; i < 5; i++)
                    {
                        int colum = i + 3;

                        worksheetMPSA.Cells[colum, c * 5 + 4].Value = listModel[0].SupplyChainMPSA[i].FR;
                        worksheetMPSA.Cells[colum, c * 5 + 5].Value = listModel[0].SupplyChainMPSA[i].Bottle;
                        worksheetMPSA.Cells[colum, c * 5 + 6].Value = listModel[0].SupplyChainMPSA[i].Sachet1;
                        worksheetMPSA.Cells[colum, c * 5 + 7].Value = listModel[0].SupplyChainMPSA[i].Sachet2;
                        worksheetMPSA.Cells[colum, c * 5 + 8].Value = listModel[0].SupplyChainMPSA[i].Pouch;


                    }
                    for (int i = 5; i < 7; i++)
                    {
                        int colum = i + 4;

                        worksheetMPSA.Cells[colum, c * 5 + 4].Value = listModel[0].SupplyChainMPSA[i].MPSAFR;
                        worksheetMPSA.Cells[colum, c * 5 + 5, colum, c * 5 + 8].Merge = true;
                        worksheetMPSA.Cells[colum, c * 5 + 5, colum, c * 5 + 8].Value = listModel[0].SupplyChainMPSA[i].MPSAFE;

                    }




                    #endregion

                    #region set width

                    worksheetMPSA.Column(c * 5 + 4).Width = 20;
                    worksheetMPSA.Column(c * 5 + 5).Width = 20;
                    worksheetMPSA.Column(c * 5 + 6).Width = 20;
                    worksheetMPSA.Column(c * 5 + 7).Width = 20;
                    worksheetMPSA.Column(c * 5 + 8).Width = 20;
                    worksheetMPSA.Cells[1, 1, 10, 5 * listModel.Count + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 

          
                    #endregion
                }

                #endregion

                #region ProductionPlanning

                var worksheetProductionPlanning = xlPackage.Workbook.Worksheets.Add("ProductionPlanning");

                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Merge = true;
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Value = listModel[i].SupplyChainFPQ[0].CreatedDate;
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[1, i * 4 + 3, 1, i * 4 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header
                worksheetProductionPlanning.Cells["A1:A2"].Merge = true;
                worksheetProductionPlanning.Cells["A1:A2"].Value = "DMS";
                worksheetProductionPlanning.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetProductionPlanning.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["A3:A15"].Merge = true;
                worksheetProductionPlanning.Cells["A3:A15"].Value = "ProductionPlanning";
                worksheetProductionPlanning.Cells["A3:A15"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["A3:A15"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["A3:A15"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetProductionPlanning.Cells["A3:A15"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["A3:A15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["A3:A15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["B1:B2"].Merge = true;
                worksheetProductionPlanning.Cells["B1:B2"].Value = "MProduction Unit-Making";
                worksheetProductionPlanning.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetProductionPlanning.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetProductionPlanning.Cells["B9"].Value = "Production Unit Pkg";
                worksheetProductionPlanning.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["B9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["B9"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["B9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["C2"].Value = "Owner";
                worksheetProductionPlanning.Cells["C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetProductionPlanning.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                worksheetProductionPlanning.Cells["C9"].Value = "Month Target";
                worksheetProductionPlanning.Cells["C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["C9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["C9"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["C9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["C9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["D2"].Value = "Shift 2";
                worksheetProductionPlanning.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["D9"].Value = "Today 's Plan";
                worksheetProductionPlanning.Cells["D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["D9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["D9"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["D9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["D9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["E2"].Value = "Shift 3";
                worksheetProductionPlanning.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["E9"].Value = "MTD";
                worksheetProductionPlanning.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["E9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["E9"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["E9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["E9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetProductionPlanning.Cells["F2"].Value = "Shift 1";
                worksheetProductionPlanning.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetProductionPlanning.Cells["F9"].Value = "Gap";
                worksheetProductionPlanning.Cells["F9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetProductionPlanning.Cells["F9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetProductionPlanning.Cells["F9"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetProductionPlanning.Cells["F9"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetProductionPlanning.Cells["F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Cells["F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                #endregion


                #region create content

                for (int i = 0; i < 6; i++)
                {
                    int colum = i + 3;
                    worksheetProductionPlanning.Cells["B" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].MeasureName;
                    worksheetProductionPlanning.Cells["C" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].Owner;
                    worksheetProductionPlanning.Cells["D" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].Shift2;
                    worksheetProductionPlanning.Cells["E" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].Shift3;
                    worksheetProductionPlanning.Cells["F" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].Shift1;
                   

                }
                for (int i = 6; i < 12; i++)
                {
                    int colum = i + 4;
                    worksheetProductionPlanning.Cells["B" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].MeasureName;
                    worksheetProductionPlanning.Cells["C" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].MonthTarget;
                    worksheetProductionPlanning.Cells["D" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].TodayPlan;
                    worksheetProductionPlanning.Cells["E" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].MTD;
                    worksheetProductionPlanning.Cells["F" + colum].Value = listModel[0].SupplyChainProductionPlanning[i].Gap;
                   
                }




                #endregion

                #region set width

                worksheetProductionPlanning.Cells["A3:A15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetProductionPlanning.Column(2).Width = 30;
                worksheetProductionPlanning.Column(3).Width = 20;
                worksheetProductionPlanning.Column(4).Width = 20;
                worksheetProductionPlanning.Column(5).Width = 20;
                worksheetProductionPlanning.Column(6).Width = 20;
                worksheetProductionPlanning.Column(7).Width = 20;

                #endregion

                for (int c = 1; c < listModel.Count; c++)
                {
                    #region create header
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Value = "Shift 2";
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[2, c * 4 + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Value = "Month Target";
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[9, c * 4 + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Value = "Shift 3";
                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[2, c * 4 +4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[2, c * 4 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Value = "Today 's Plan";
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[9, c * 4 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Value = "Shift 1";
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[2, c * 4 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Value = "MTD";
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[9, c * 4 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Value = "Remark";
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[2, c * 4 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Value = "Gap";
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetProductionPlanning.Cells[9, c * 4 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion


                    #region create content

                    for (int i = 0; i < 6; i++)
                    {
                        int colum = i + 3;
                       
                        worksheetProductionPlanning.Cells[colum,c*4+3].Value = listModel[c].SupplyChainProductionPlanning[i].Shift2;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 4].Value = listModel[c].SupplyChainProductionPlanning[i].Shift3;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 5].Value = listModel[c].SupplyChainProductionPlanning[i].Shift1;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 6].Value = listModel[c].SupplyChainProductionPlanning[i].Remark;

                    }
                    for (int i = 6; i < 12; i++)
                    {
                        int colum = i + 4;

                        worksheetProductionPlanning.Cells[colum, c * 4 + 3].Value = listModel[c].SupplyChainProductionPlanning[i].MonthTarget;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 4].Value = listModel[c].SupplyChainProductionPlanning[i].TodayPlan;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 5].Value = listModel[c].SupplyChainProductionPlanning[i].MTD;
                        worksheetProductionPlanning.Cells[colum, c * 4 + 6].Value = listModel[c].SupplyChainProductionPlanning[i].Gap;
                      
                    }




                    #endregion

                    #region set width


                    worksheetProductionPlanning.Column(c * 4 + 3).Width = 20;
                    worksheetProductionPlanning.Column(c * 4 + 4).Width = 20;
                    worksheetProductionPlanning.Column(c * 4 + 5).Width = 20;
                    worksheetProductionPlanning.Column(c * 4 + 6).Width = 20;
                    worksheetProductionPlanning.Column(c * 4 + 7).Width = 20;
                    worksheetProductionPlanning.Cells[1, 1, 15, 4 * listModel.Count + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 
                    #endregion
                }

                #endregion

                #region Service

                var worksheetService = xlPackage.Workbook.Worksheets.Add("Service");


                #region create date
                for (int i = 0; i < listModel.Count; i++)
                {
                    worksheetService.Cells[1, i * 10 + 3,1, i * 10 + 12].Merge = true;
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Value = listModel[i].SupplyChainFPQ[0].CreatedDate;
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[1, i * 10 + 3, 1, i * 10 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                #endregion

                #region create header
                  worksheetService.Cells["A1:A3"].Merge = true;
                worksheetService.Cells["A1:A3"].Value = "DMS";
                worksheetService.Cells["A1:A3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["A1:A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["A1:A3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["A1:A3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["A1:A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["A1:A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["A4:A6"].Merge = true;
                worksheetService.Cells["A4:A6"].Value = "Service";
                worksheetService.Cells["A4:A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["A4:A6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["A4:A6"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                worksheetService.Cells["A4:A6"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["A4:A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["A4:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheetService.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["B1:B2"].Merge = true;
                worksheetService.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["B3"].Value = listModel[0].SupplyChainService[0].Owner;
                worksheetService.Cells["B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["B3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["B3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["C2:G2"].Merge = true;
                worksheetService.Cells["C2:G2"].Value = "Daily-FE";
                worksheetService.Cells["C2:G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["C2:G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["C2:G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["C2:G2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["C2:G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["C2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["H2:L2"].Merge = true;
                worksheetService.Cells["H2:L2"].Value = "Daily-FE";
                worksheetService.Cells["H2:L2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["H2:L2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["H2:L2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["H2:L2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["H2:L2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["H2:L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["C3"].Value = "CFR(%)";
                worksheetService.Cells["C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["C3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["C3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["C3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["D3"].Value = "SAMBC(%)";
                worksheetService.Cells["D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["D3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["D3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["E3"].Merge = true;
                worksheetService.Cells["E3"].Value = "Priority Line";
                worksheetService.Cells["E3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["E3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["E3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["E3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["F3"].Value = "Priority SKU";
                worksheetService.Cells["F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["F3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["F3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["F3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["G3"].Value = "Shipment";
                worksheetService.Cells["G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["G3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["G3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["G3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["H3"].Value = "CFR(%)";
                worksheetService.Cells["H3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["H3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["H3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["H3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["I3"].Value = "SAMBC(%)";
                worksheetService.Cells["I3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["I3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["I3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["J3"].Merge = true;
                worksheetService.Cells["J3"].Value = "Priority Line";
                worksheetService.Cells["J3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["J3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["J3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["J3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["K3"].Value = "Priority SKU";
                worksheetService.Cells["K3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["K3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["K3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["K3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["L3"].Value = "Shipment";
                worksheetService.Cells["L3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheetService.Cells["L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["L3"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheetService.Cells["L3"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheetService.Cells["L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetService.Cells["B4:L4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetService.Cells["B4:L4"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                worksheetService.Cells["B4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["B4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                #endregion


                #region create content


                for (int i = 0; i < 3; i++)
                {
                    int colum = i + 4;
                    worksheetService.Cells["B" + colum].Value = listModel[0].SupplyChainService[i].MeasureName;
                    worksheetService.Cells["C" + colum].Value = listModel[0].SupplyChainService[i].CFR;
                    worksheetService.Cells["D" + colum].Value = listModel[0].SupplyChainService[i].SAMBC;
                    worksheetService.Cells["E" + colum].Value = listModel[0].SupplyChainService[i].PriorityLine;
                    worksheetService.Cells["F" + colum].Value = listModel[0].SupplyChainService[i].PrioritySKU;
                    worksheetService.Cells["G" + colum].Value = listModel[0].SupplyChainService[i].Shipment;
                }
                for (int i = 3; i < 6; i++)
                {
                    int colum = i+1;

                    worksheetService.Cells["H" + colum].Value = listModel[0].SupplyChainService[i].CFR;
                    worksheetService.Cells["I" + colum].Value = listModel[0].SupplyChainService[i].SAMBC;
                    worksheetService.Cells["J" + colum].Value = listModel[0].SupplyChainService[i].PriorityLine;
                    worksheetService.Cells["K" + colum].Value = listModel[0].SupplyChainService[i].PrioritySKU;
                    worksheetService.Cells["L" + colum].Value = listModel[0].SupplyChainService[i].Shipment;
                }




                #endregion


                #region set width

                worksheetService.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Cells["A3:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheetService.Column(1).Width = 20;
                worksheetService.Column(2).Width = 20;
                worksheetService.Column(3).Width = 20;
                worksheetService.Column(4).Width = 20;
                worksheetService.Column(5).Width = 20;
                worksheetService.Column(6).Width = 20;
                worksheetService.Column(7).Width = 20;
                worksheetService.Column(8).Width = 20;
                worksheetService.Column(9).Width = 20;
                worksheetService.Column(10).Width = 20;
                worksheetService.Column(11).Width = 20;
                worksheetService.Column(12).Width = 20;

                #endregion

                for (int c = 1; c < listModel.Count; c++)
                {
                    #region create header
               

                    worksheetService.Cells[2,c*10+3,2,c*10+7].Merge = true;
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Value = "Daily-FE";
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[2, c * 10 + 3, 2, c * 10 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Merge = true;
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Value = "Daily-FR";
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[2, c * 10 + 8, 2, c * 10 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 3].Value = "CFR(%)";
                    worksheetService.Cells[3, c * 10 + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 3].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 3].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 4].Value = "SAMBC(%)";
                    worksheetService.Cells[3, c * 10 + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 4].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 4].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 5].Merge = true;
                    worksheetService.Cells[3, c * 10 + 5].Value = "Priority Line";
                    worksheetService.Cells[3, c * 10 + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 5].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 5].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 6].Value = "Priority SKU";
                    worksheetService.Cells[3, c * 10 + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 6].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 6].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 7].Value = "Shipment";
                    worksheetService.Cells[3, c * 10 + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 7].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 7].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 77].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 8].Value = "CFR(%)";
                    worksheetService.Cells[3, c * 10 + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 8].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 8].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 9].Value = "SAMBC(%)";
                    worksheetService.Cells[3, c * 10 + 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 9].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 9].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 10].Merge = true;
                    worksheetService.Cells[3, c * 10 + 10].Value = "Priority Line";
                    worksheetService.Cells[3, c * 10 + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 10].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 10].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 11].Value = "Priority SKU";
                    worksheetService.Cells[3, c * 10 + 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 11].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 11].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[3, c * 10 + 12].Value = "Shipment";
                    worksheetService.Cells[3, c * 10 + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                    worksheetService.Cells[3, c * 10 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[3, c * 10 + 12].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                    worksheetService.Cells[3, c * 10 + 12].Style.Font.Color.SetColor(Color.White); // set color font
                    worksheetService.Cells[3, c * 10 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[3, c * 10 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheetService.Cells[4, c * 10 + 3,4, c * 10 + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetService.Cells[4, c * 10 + 3, 4, c * 10 + 12].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                    worksheetService.Cells[4, c * 10 + 3, 4, c * 10 + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheetService.Cells[4, c * 10 + 3, 4, c * 10 + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion


                    #region create content


                    for (int i = 0; i < 3; i++)
                    {
                        int colum = i + 4;

                        worksheetService.Cells[colum, c * 10 + 3].Value = listModel[0].SupplyChainService[i].CFR;
                        worksheetService.Cells[colum, c * 10 + 4].Value = listModel[0].SupplyChainService[i].SAMBC;
                        worksheetService.Cells[colum, c * 10 + 5].Value = listModel[0].SupplyChainService[i].PriorityLine;
                        worksheetService.Cells[colum, c * 10 + 6].Value = listModel[0].SupplyChainService[i].PrioritySKU;
                        worksheetService.Cells[colum, c * 10 + 7].Value = listModel[0].SupplyChainService[i].Shipment;
                    }
                    for (int i = 3; i < 6; i++)
                    {
                        int colum = i + 1;

                        worksheetService.Cells[colum, c * 10 + 8].Value = listModel[0].SupplyChainService[i].CFR;
                        worksheetService.Cells[colum, c * 10 + 9].Value = listModel[0].SupplyChainService[i].SAMBC;
                        worksheetService.Cells[colum, c * 10 + 10].Value = listModel[0].SupplyChainService[i].PriorityLine;
                        worksheetService.Cells[colum, c * 10 + 11].Value = listModel[0].SupplyChainService[i].PrioritySKU;
                        worksheetService.Cells[colum, c * 10 + 12].Value = listModel[0].SupplyChainService[i].Shipment;
                    }




                    #endregion


                    #region set width


                    worksheetService.Column(c * 10 + 3).Width = 20;
                    worksheetService.Column(c * 10 + 4).Width = 20;
                    worksheetService.Column(c * 10 + 5).Width = 20;
                    worksheetService.Column(c * 10 + 6).Width = 20;
                    worksheetService.Column(c * 10 + 7).Width = 20;
                    worksheetService.Column(c * 10 + 8).Width = 20;
                    worksheetService.Column(c * 10 + 9).Width = 20;
                    worksheetService.Column(c * 10 + 10).Width = 20;
                    worksheetService.Column(c * 10 + 11).Width = 20;
                    worksheetService.Column(c * 10 + 12).Width = 20;
                    worksheetService.Cells[1, 1, 6, 10 * listModel.Count + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 
                    #endregion
                }

                #endregion

                if (String.IsNullOrEmpty(path))
                {
                    xlPackage.Save(); // save to excell
                }
                else
                {
                    xlPackage.SaveAs(new FileInfo(path));

                    stream.Close();
                }
            }

        }

        [HttpPost]
        public async Task<JsonResult> SendMailWithAttachment(string fromDate, string toDate, string emails)
        {

            DateTime fromDateSearch;
            if (String.IsNullOrEmpty(fromDate)) fromDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(fromDate, culture, styles, out fromDateSearch))
                {
                    fromDateSearch = DateTime.Now;
                }
            }

            DateTime toDateSearch;
            if (String.IsNullOrEmpty(toDate)) toDateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(toDate, culture, styles, out toDateSearch))
                {
                    toDateSearch = DateTime.Now;
                }
            }

            var listModel = new List<SupplyChainModel>();



            for (var i = fromDateSearch; i <= toDateSearch; i = i.AddDays(1))
            {
                DateTime dateSearch = i;

                var model = new SupplyChainModel();

                #region HSE

                var allUser = _userService.GetAllUsersAsync();
                var owner = allUser.LastOrDefault();
                var currentUser = await _userService.GetUserByUsernameAsync("Admin");
                var supplyChainHSE = new List<SupplyChainHSEModel>();


                supplyChainHSE = new List<SupplyChainHSEModel>()
                                     {
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("HSE", SupplyChainHSEMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int) SupplyChainHSEMeasure.Target,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("HSE", SupplyChainHSEMeasure.BOSCompletion
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure.BOSCompletion,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),

                                         },
                                         new SupplyChainHSEModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("HSE", SupplyChainHSEMeasure.BOSTopUnsafeBehaviour
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode =
                                                 (int)
                                                 SupplyChainHSEMeasure
                                                 .BOSTopUnsafeBehaviour,
                                             CreatedDate =
                                                 dateSearch.ToShortDateString(),
                                             UpdatedDate =
                                                 dateSearch.ToShortDateString(),
                                         }
                                     };
                var allsupplyChain =
                    _supplyChainHSE.GetAllSupplyChainHSEs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target)
                        .ToList();

                if (allsupplyChain.Count == 0)
                {

                    var newModel = new SupplyChainHSE()
                    {
                        Owner = owner.Username,
                        UserUpdatedId = currentUser.Id,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        UserCreatedId = currentUser.Id,
                        MeasureCode = (int)SupplyChainHSEMeasure.Target,
                        Making = "Daily 1 LT to share the BOS Observation"

                    };
                    await _supplyChainHSE.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChain.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new SupplyChainHSE()
                        {
                            Owner = lastDayTarget.Owner,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            Making = lastDayTarget.Making,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainHSE.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainHSE)
                {


                    var supplychainResult =
                        _supplyChainHSE.GetSupplyChainHSEMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {

                        item.MeasureCode = supplychainResult.MeasureCode;
                        item.Making = supplychainResult.Making;
                        //item.MeasureName = ((SupplyChainHSEMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.Packing = supplychainResult.Packing;
                        item.Remarks = supplychainResult.Remarks;
                        //item.CreatedUser = supplychainResult.CreatedUser.Username;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.CommonArea = supplychainResult.CommonArea;
                    }
                    else
                    {
                        var allsupplyChainHSEOwner =
                            _supplyChainHSE.GetAllSupplyChainHSEs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                .ToList();


                        if (allsupplyChainHSEOwner.Count == 0)
                        {

                            var newModel = new SupplyChainHSE()
                            {
                                Owner = owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,


                            };
                            await _supplyChainHSE.CreateAsync(newModel);
                            item.Owner = owner.Username;
                        }
                        else
                        {
                            if (allsupplyChainHSEOwner.FirstOrDefault().CreatedDate >= dateSearch &&
                                allsupplyChainHSEOwner.LastOrDefault().CreatedDate >= dateSearch)
                            {
                                var lastDayTarget = allsupplyChainHSEOwner.Last();
                                if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                {


                                    var newModel = new SupplyChainHSE()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                    };
                                    await _supplyChainHSE.CreateAsync(newModel);
                                    item.Owner = lastDayTarget.Owner;
                                }

                            }
                        }
                    }


                    model.SupplyChainHSE = supplyChainHSE;



                }

                #endregion

                #region DDS


                var supplyChainDDS = new List<SupplyChainDDSModel>()
                                     {
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 2
                                         },
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 2
                                         },
                                         new SupplyChainDDSModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("DDS", SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 2
                                         }

                                     };
                var allsupplyChainMaking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.type == 1)
                        .ToList();

                if (allsupplyChainMaking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        LPD1 = "90%",
                        LPD2 = "90%",
                        LPD3 = "90%",
                        Batch = "90%",
                        FRMK = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMaking.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            CreatedDate = DateTime.Now,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 1,
                            LPD1 = "90%",
                            LPD2 = "90%",
                            LPD3 = "90%",
                            Batch = "90%",
                            FRMK = "90%",
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                var allsupplyChainPacking =
                    _supplyChainDDS.GetAllSupplyChainDDSs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.type == 2)
                        .ToList();

                if (allsupplyChainPacking.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainDDS()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainDDSMeasure.Target,
                        Bottle = "90%",
                        Sachet = "90%",
                        Pouch = "90%",
                        FRPK = "90%",
                        FE = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainDDS.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMaking.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainDDS()
                        {
                            Owner = lastDayTarget.Owner,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            type = 2,
                            Bottle = "90%",
                            Sachet = "90%",
                            Pouch = "90%",
                            FRPK = "90%",
                            FE = "90%",
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainDDS.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainDDS)
                {
                    var supplychainResult =
                        _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FRPK = supplychainResult.FRPK;
                        item.Pouch = supplychainResult.Pouch;
                        item.Remark = supplychainResult.Remark;
                        item.FRMK = supplychainResult.FRMK;
                        item.FE = supplychainResult.FE;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Batch = supplychainResult.Batch;
                        await _supplyChainDDS.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        var allsupplyChainDDSOwner =
                            _supplyChainDDS.GetAllSupplyChainDDSs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode && p.type == item.type)
                                .ToList();
                        if (allsupplyChainDDSOwner.Count == 0)
                        {

                            var newModel = new SupplyChainDDS()
                            {
                                Owner = owner.Username,
                                UserUpdatedId = currentUser.Id,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                UserCreatedId = currentUser.Id,
                                MeasureCode = item.MeasureCode,
                                type = item.type

                            };
                            await _supplyChainDDS.CreateAsync(newModel);
                            item.Owner = owner.Username;
                        }
                        else
                        {
                            if (allsupplyChainDDSOwner.FirstOrDefault().CreatedDate >= dateSearch &&
                                allsupplyChainDDSOwner.LastOrDefault().CreatedDate >= dateSearch)
                            {
                                var lastDayTarget = allsupplyChainDDSOwner.Last();
                                if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                {


                                    var newModel = new SupplyChainDDS()
                                    {
                                        Owner = lastDayTarget.Owner,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                        MeasureCode = lastDayTarget.MeasureCode,
                                        type = item.type
                                    };
                                    await _supplyChainDDS.CreateAsync(newModel);
                                    item.Owner = lastDayTarget.Owner;
                                }
                            }
                        }
                    }
                }


                model.SupplyChainDDS = supplyChainDDS;



                #endregion

                #region FPQ

                var SupplyChainFPQ = new List<SupplyChainFPQModel>()
                                     {
                                         new SupplyChainFPQModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("FPQ", SupplyChainFPQMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainFPQMeasure.Target,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainFPQModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("FPQ", SupplyChainFPQMeasure.QuanlityBOS
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainFPQMeasure.QuanlityBOS,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainFPQModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("FPQ", SupplyChainFPQMeasure.UHEventPending
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainFPQMeasure.UHEventPending,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 1
                                         },
                                         new SupplyChainFPQModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("FPQ", SupplyChainFPQMeasure.UHEventLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainFPQMeasure.UHEventLastDay,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 2
                                         },
                                         new SupplyChainFPQModel()
                                         {
                                             CreatedUser = User.Identity.Name,
                                             MeasureName =
                                                 _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode
                                                 ("FPQ", SupplyChainFPQMeasure.UHEventMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                             MeasureCode = (int) SupplyChainFPQMeasure.UHEventMTD,
                                             CreatedDate = dateSearch.ToShortDateString(),
                                             UpdatedDate = dateSearch.ToShortDateString(),
                                             type = 2
                                         }

                                     };
                var allsupplyChainFPQ =
                    _supplyChainFPQ.GetAllSupplyChainFPQs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainFPQMeasure.Target)
                        .ToList();

                if (allsupplyChainFPQ.Count == 0)
                {
                    var newModel = new SupplyChainFPQ()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainFPQMeasure.Target,
                        LPD1 = "Daily 1 LT to share the BOS Observation",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id

                    };
                    await _supplyChainFPQ.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFPQ.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainFPQ()
                        {
                            CreatedDate = DateTime.Now,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            LPD1 = lastDayTarget.LPD1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainFPQ.CreateAsync(newModel);
                    }

                }
                foreach (var item in SupplyChainFPQ)
                {
                    var supplychainResult =
                        _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainFPQMeasure) supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.ListUsernameInSupplyChainFPQ =
                            _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainResult.Id);
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.LPD1 = supplychainResult.LPD1;
                        item.LPD2 = supplychainResult.LPD2;
                        item.LPD3 = supplychainResult.LPD3;
                        item.FR = supplychainResult.FR;
                        item.Pouch = supplychainResult.Pouch;
                        item.Remark = supplychainResult.Remark;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet = supplychainResult.Sachet;
                        item.Batch = supplychainResult.Batch;
                        await _supplyChainFPQ.UpdateAsync(supplychainResult);

                    }
                    else
                    {

                        var allsupplyChainFPQOwner =
                            _supplyChainFPQ.GetAllSupplyChainFPQs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                .ToList();

                        if (allsupplyChainFPQOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainFPQ()
                            {
                                CreatedDate = DateTime.Now,
                                Owner = owner.Username,
                                UpdatedDate = DateTime.Now,
                                MeasureCode = item.MeasureCode,
                                type = item.type,
                            };
                            await _supplyChainFPQ.CreateAsync(newModel);
                            var supplychainNew =
                                _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                                    dateSearch);
                            await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                            {
                                SupplyChainFpqId =
                                    supplychainNew.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainFPQ =
                                _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainNew.Id);
                        }
                        else
                        {
                            if (allsupplyChainFPQOwner.FirstOrDefault().CreatedDate <= dateSearch &&
                                allsupplyChainFPQOwner.LastOrDefault().CreatedDate >= dateSearch)
                            {
                                var newModel = new Entities.Domain.SupplyChainFPQ()
                                {
                                    CreatedDate = DateTime.Now,
                                    Owner = allsupplyChainFPQOwner.Last().Owner,
                                    UpdatedDate = DateTime.Now,
                                    MeasureCode = allsupplyChainFPQOwner.Last().MeasureCode,
                                    type = allsupplyChainFPQOwner.Last().type,
                                };

                                var listOldOwner =
                                    _supplyChainFPQ.GetUserIdInSupplyChainFPQ(allsupplyChainFPQOwner.Last().Id);
                                await _supplyChainFPQ.CreateAsync(newModel);
                                var supplychainNew =
                                    _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                                        dateSearch);
                                foreach (var itemOwner in listOldOwner)
                                {
                                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                                    {
                                        SupplyChainFpqId =
                                            supplychainNew.Id,
                                        UserId = itemOwner
                                    });
                                }
                                if (supplychainNew != null)
                                    item.ListUsernameInSupplyChainFPQ =
                                        _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainNew.Id);
                            }
                        }
                    }
                }

                model.SupplyChainFPQ = SupplyChainFPQ;

                #endregion

                #region Service


                var supplyChainService = new List<SupplyChainServiceModel>()
                                         {
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.Target
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.Daily
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.MTD
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.Target
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             },
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.Daily
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             },
                                             new SupplyChainServiceModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =
                                                     _measureSupplyChainService
                                                     .GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",
                                                         SupplyChainServiceMeasure.MTD
                                                     .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             }

                                         };
                var allsupplyChainFE =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.type == 1)
                        .ToList();

                if (allsupplyChainFE.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 1,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainFE.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {

                        var newModel = new Entities.Domain.SupplyChainService()
                        {
                            CreatedDate = DateTime.Now,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            CFR = "90%",
                            SAMBC = "90%",
                            PrioritySKU = "90%",
                            PriorityLine = "90%",
                            Shipment = "90%",
                            type = 1,
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id

                        };
                        await _supplyChainService.CreateAsync(newModel);
                    }

                }
                var allsupplyChainService =
                    _supplyChainService.GetAllSupplyChainServices()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.type == 2)
                        .ToList();

                if (allsupplyChainService.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainService()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainServiceMeasure.Target,
                        CFR = "90%",
                        SAMBC = "90%",
                        PrioritySKU = "90%",
                        PriorityLine = "90%",
                        Shipment = "90%",
                        type = 2,
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainService.CreateAsync(newModel);
                }
                else
                {
                    if (allsupplyChainService.FirstOrDefault().CreatedDate <= dateSearch &&
                        allsupplyChainService.LastOrDefault().CreatedDate >= dateSearch)
                    {
                        var lastDayTarget = allsupplyChainService.Last();
                        if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                        {


                            var newModel = new Entities.Domain.SupplyChainService()
                            {
                                CreatedDate = DateTime.Now,
                                Owner = lastDayTarget.Owner,
                                UpdatedDate = DateTime.Now,
                                MeasureCode = lastDayTarget.MeasureCode,
                                CFR = "90%",
                                SAMBC = "90%",
                                PrioritySKU = "90%",
                                PriorityLine = "90%",
                                Shipment = "90%",
                                type = 2,
                                UserCreatedId = currentUser.Id,
                                UserUpdatedId = currentUser.Id

                            };
                            await _supplyChainService.CreateAsync(newModel);
                        }
                    }
                }
                foreach (var item in supplyChainService)
                {
                    var supplychainResult =
                        _supplyChainService.GetSupplyChainServiceMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {
                        item.Owner = supplychainResult.Owner;
                        item.MeasureCode = supplychainResult.MeasureCode;
                        //item.MeasureName = ((SupplyChainServiceMeasure)supplychainResult.MeasureCode).ToString();
                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.CFR = supplychainResult.CFR;
                        item.PriorityLine = supplychainResult.PriorityLine;
                        item.PrioritySKU = supplychainResult.PrioritySKU;
                        item.SAMBC = supplychainResult.SAMBC;
                        item.Shipment = supplychainResult.Shipment;
                        await _supplyChainService.UpdateAsync(supplychainResult);

                    }
                }

                model.SupplyChainService = supplyChainService;



                #endregion

                #region ProductionPlanning


                var SupplyChainProductionPlanning = new List<SupplyChainProductionPlanningModel>()
                                                    {
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.LPD1
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.LPD1,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.LPD2
                                                                .ToString()).Result.MeasureSupplyChainName,

                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.LPD2,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.LPD3
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.LPD3,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.DSR
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.DSR,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.FRMK3
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.FRMK3,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.FRMK4
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.FRMK4,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 1
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.Sachet
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.Sachet,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.Pouch
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.Pouch,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.Bottle
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.Bottle,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.FE
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.FE,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        },
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                SupplyChainProductionPlanningMeasure.FR.ToString(),
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.FR,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        }
                                                        ,
                                                        new SupplyChainProductionPlanningModel()
                                                        {
                                                            CreatedUser = User.Identity.Name,
                                                            MeasureName =
                                                                _measureSupplyChainService
                                                                .GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                                    "ProductionPlanning",
                                                                    SupplyChainProductionPlanningMeasure.Total
                                                                .ToString()).Result.MeasureSupplyChainName,
                                                            MeasureCode =
                                                                (int) SupplyChainProductionPlanningMeasure.Total,
                                                            CreatedDate = dateSearch.ToShortDateString(),
                                                            UpdatedDate = dateSearch.ToShortDateString(),
                                                            type = 2
                                                        }

                                                    };

                foreach (var item in SupplyChainProductionPlanning)
                {
                    var supplychainResult =
                        _supplyChainProductionPlanning.GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(
                            item.MeasureCode.ToString(),
                            dateSearch, item.type);
                    if (supplychainResult != null)
                    {

                        item.CreatedDate = supplychainResult.CreatedDate.ToShortDateString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.type = supplychainResult.type;
                        item.Shift1 = supplychainResult.Shift1;
                        item.Shift2 = supplychainResult.Shift2;
                        item.Shift3 = supplychainResult.Shift3;
                        item.TodayPlan = supplychainResult.TodayPlan;
                        item.MTD = supplychainResult.MTD;
                        item.MonthTarget = supplychainResult.MonthTarget;
                        item.Gap = supplychainResult.Gap;
                        item.Remark = supplychainResult.Remark;
                        await _supplyChainProductionPlanning.UpdateAsync(supplychainResult);

                    }
                    else
                    {
                        if (item.type == 1)
                        {
                            var allsupplyChainProductionPlanningOwner =
                                _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                    .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                    .ToList();

                            if (allsupplyChainProductionPlanningOwner.Count == 0)
                            {

                                var newModel = new SupplyChainProductionPlanning()
                                {
                                    Owner = owner.Username,
                                    UserUpdatedId = currentUser.Id,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                    UserCreatedId = currentUser.Id,
                                    MeasureCode = item.MeasureCode,


                                };
                                await _supplyChainProductionPlanning.CreateAsync(newModel);
                                item.Owner = owner.Username;
                            }
                            else
                            {
                                if (allsupplyChainProductionPlanningOwner.FirstOrDefault().CreatedDate >= dateSearch &&
                                    allsupplyChainProductionPlanningOwner.LastOrDefault().CreatedDate >= dateSearch)
                                {
                                    var lastDayTarget = allsupplyChainProductionPlanningOwner.Last();
                                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                                    {


                                        var newModel = new SupplyChainProductionPlanning()
                                        {
                                            Owner = lastDayTarget.Owner,
                                            CreatedDate = DateTime.Now,
                                            UpdatedDate = DateTime.Now,
                                            MeasureCode = lastDayTarget.MeasureCode,
                                        };
                                        await _supplyChainProductionPlanning.CreateAsync(newModel);
                                        item.Owner = lastDayTarget.Owner;
                                    }

                                }
                            }
                        }
                    }
                }

                model.SupplyChainProductionPlanning = SupplyChainProductionPlanning;


                #endregion

                #region MPSA


                //var meetingResult = _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime.Now,
                //        "0", "TotalPO");
                // var meetingResult = _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime.Now,
                // line.ToString(), "16"");
                var supplyChainMPSA = new List<SupplyChainMPSAModel>();

                supplyChainMPSA = new List<SupplyChainMPSAModel>()
                                  {
                                      new SupplyChainMPSAModel()
                                      {

                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA", SupplyChainMPSAMeasure.Target.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int) SupplyChainMPSAMeasure.Target,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      },
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA", SupplyChainMPSAMeasure.TotalPO.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.TotalPO,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),



                                      }
                                      ,
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA",
                                                  SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      }
                                      ,
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA",
                                                  SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      }
                                      ,
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA",
                                                  SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      }
                                      ,
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA", SupplyChainMPSAMeasure.DailyMPSA.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.DailyMPSA,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      }
                                      ,
                                      new SupplyChainMPSAModel()
                                      {
                                          CreatedUser = User.Identity.Name,
                                          MeasureName =
                                              _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode(
                                                  "MPSA", SupplyChainMPSAMeasure.MTDMPSA.ToString())
                                              .Result.MeasureSupplyChainName,
                                          MeasureCode =
                                              (int)
                                              SupplyChainMPSAMeasure.MTDMPSA,
                                          CreatedDate =
                                              dateSearch.ToShortDateString(),
                                          UpdatedDate =
                                              dateSearch.ToShortDateString(),

                                      }
                                  };
                var allsupplyChainMPSA =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target)
                        .ToList();

                if (allsupplyChainMPSA.Count == 0)
                {
                    var newModel = new Entities.Domain.SupplyChainMPSA()
                    {

                        CreatedDate = DateTime.Now,
                        Owner = owner.Username,
                        UpdatedDate = DateTime.Now,
                        MeasureCode = (int)SupplyChainMPSAMeasure.Target,
                        MPSAFR = "PO Missed < 90% of total",
                        UserCreatedId = currentUser.Id,
                        UserUpdatedId = currentUser.Id
                    };
                    await _supplyChainMPSA.CreateAsync(newModel);
                }
                else
                {
                    var lastDayTarget = allsupplyChainMPSA.Last();
                    if (lastDayTarget.CreatedDate.Date != DateTime.Now.Date)
                    {


                        var newModel = new Entities.Domain.SupplyChainMPSA()
                        {
                            CreatedDate = DateTime.Now,
                            Owner = lastDayTarget.Owner,
                            UpdatedDate = DateTime.Now,
                            MeasureCode = lastDayTarget.MeasureCode,
                            MPSAFR = "PO Missed < 90% of total",
                            UserCreatedId = currentUser.Id,
                            UserUpdatedId = currentUser.Id
                        };
                        await _supplyChainMPSA.CreateAsync(newModel);
                    }

                }
                foreach (var item in supplyChainMPSA)
                {
                    var supplychainResult =
                        _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                            dateSearch);
                    if (supplychainResult != null)
                    {
                        //item.MeasureName = ((SupplyChainMPSAMeasure) supplychainResult.MeasureCode).ToString();
                        item.Owner = supplychainResult.Owner;
                        item.UpdatedDate = supplychainResult.UpdatedDate.ToShortDateString();
                        item.FR = supplychainResult.FR;
                        item.Bottle = supplychainResult.Bottle;
                        item.Sachet1 = supplychainResult.Sachet1;
                        item.Sachet2 = supplychainResult.Sachet2;
                        item.Pouch = supplychainResult.Pouch;
                        item.Remark = supplychainResult.Remark;
                        item.OutputRemark = supplychainResult.OutputRemark;
                        item.MPSAFE = supplychainResult.MPSAFE;
                        item.MPSAFR = supplychainResult.MPSAFR;
                        item.ListUsernameInSupplyChainMPSA =
                            _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainResult.Id);
                    }
                    else
                    {
                        var allsupplyChainMPSAOwner =
                            _supplyChainMPSA.GetAllSupplyChainMPSAs()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode)
                                .ToList();

                        if (allsupplyChainMPSAOwner.Count == 0)
                        {
                            var newModel = new Entities.Domain.SupplyChainMPSA()
                            {
                                CreatedDate = DateTime.Now,
                                Owner = owner.Username,
                                UpdatedDate = DateTime.Now,
                                MeasureCode = item.MeasureCode,
                            };
                            await _supplyChainMPSA.CreateAsync(newModel);
                            var supplychainNew =
                                _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                                    dateSearch);
                            await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                            {
                                SupplyChainMPSAId = supplychainNew.Id,
                                UserId = owner.Id
                            });
                            item.ListUsernameInSupplyChainMPSA =
                                _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainNew.Id);
                        }
                        else
                        {
                            if (allsupplyChainMPSAOwner.FirstOrDefault().CreatedDate <= dateSearch &&
                                allsupplyChainMPSAOwner.LastOrDefault().CreatedDate >= dateSearch)
                            {
                                var newModel = new Entities.Domain.SupplyChainMPSA()
                                {
                                    CreatedDate = DateTime.Now,
                                    Owner = allsupplyChainMPSAOwner.Last().Owner,
                                    UpdatedDate = DateTime.Now,
                                    MeasureCode = allsupplyChainMPSAOwner.Last().MeasureCode,
                                };

                                var listOldOwner =
                                    _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);
                                await _supplyChainMPSA.CreateAsync(newModel);
                                var supplychainNew =
                                    _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(item.MeasureCode.ToString(),
                                        dateSearch);
                                foreach (var itemOwner in listOldOwner)
                                {
                                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                                    {
                                        SupplyChainMPSAId =
                                            supplychainNew.Id,
                                        UserId = itemOwner
                                    });
                                }
                                if (supplychainNew != null)
                                    item.ListUsernameInSupplyChainMPSA =
                                        _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainNew.Id);
                            }
                        }
                    }
                }

                model.SupplyChainMPSA = supplyChainMPSA;


                #endregion

                listModel.Add(model);
            }

            #region save temp excel under folder of IIS

            var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

            var r = new Random();
            int u = r.Next(10000);

            var filename = "report-export-" + u + "-" + currentTime + "-" + ".xlsx";

            var folderForSaveFile = ConfigurationManager.AppSettings["excelforderpath"];
            var folderpath = AppDomain.CurrentDomain.BaseDirectory + folderForSaveFile;
            var filePath = Path.Combine(folderpath, filename);

            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    ExportSupplyChainToXlsx(stream, listModel, filePath);
                    //bytes = stream.ToArray();

                    var listEmail = emails.Split(';');
                    foreach (var itemEmail in listEmail)
                    {
                        if (!String.IsNullOrEmpty(itemEmail))
                        {
                            var userNeedToSendMail = await _userService.GetUserByEmailAsync(itemEmail);
                            if (userNeedToSendMail != null)
                            {
                                var listAttachment = new List<string>() { filename };
                                var queueEmail = _workFlowMessageService.SendReportToUser(userNeedToSendMail, "Supply Chain", fromDate, toDate);
                                _sendMailService.Sendmail(queueEmail, listAttachment);
                            }
                        }
                    }



                }

                return Json(new { status = "success" });

            }
            catch (Exception exc)
            {
                return Json(new { error = exc });

            }

            #endregion


        }


    }

}