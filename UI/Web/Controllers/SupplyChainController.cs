using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Common;
using Service.Dds;
using Service.Interface;
using Service.Security;
using Service.SupplyChain;
using Service.Users;
using Utils;
using Web.Models.SupplyChain;
using Service.Departments;
using Web.Models.Supplychain;
using Entities.Domain.Users;
using Web.Models.ProductPlanning;
using WebGrease.Css.Extensions;
using Nois.Web.Framework.Kendoui;


namespace Web.Controllers
{
    public class SupplyChainController : Controller
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
        private readonly IPermissionService _permissionService;
        private readonly IMeasureSupplyChainService _measureSupplyChainService;
        private readonly IXmlService _xmlService;
        private readonly IProductPlanningService _productPlanningService;
        private readonly IWorkContext _workContext;
        List<ProductionPlanningColor> _listProductionPlanningColor = new List<ProductionPlanningColor>();


        private readonly IDdsMeetingService _ddsMeetingService;
        private readonly IDmsService _dmsService;
        private readonly IDepartmentService _departmentService;

        #endregion field

        #region Contructor

        public SupplyChainController(ISupplyChainHSEService supplyChainHSE, IUserService userService,
            //IAttendanceService attendanceService, 
            ISupplyChainDDSService supplyChainDds, 
            ISupplyChainFPQService supplyChainFpq, ISupplyChainServiceService supplyChainService, 
            ISupplyChainProductionPlanningService supplyChainProductionPlanning, 
            //INoisMainMeasureService noisMainMeasureService, 
            ISupplyChainMPSAService supplyChainMpsa, IMeasureSupplyChainService measureSupplyChainService, 
            IPermissionService permissionService, IProductPlanningService productPlanningService, IXmlService xmlService,
            IWorkContext workContext, IDdsMeetingService ddsMeetingService, IDmsService dmsService, 
            IDepartmentService departmentService)
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
            _permissionService = permissionService;
            _productPlanningService = productPlanningService;
            _xmlService = xmlService;
            this._workContext = workContext;
            _ddsMeetingService = ddsMeetingService;
            _dmsService = dmsService;
            _departmentService = departmentService;
            _listProductionPlanningColor = _xmlService.GetAllProductionPlanningColors();
        }

        #endregion

        private int SupplyChainDepartmentId
        {
            get { return 7; }
        }

        // GET: SupplyChain
        public async Task<ViewResult> Index(string date, string supplyChain)
        {     
            var currentDate = DateTime.Now.Date;
            DateTime dateSearch = Extension.Parse(date, DateTime.Today);
            
            var model = new SupplyChainModel();

            var supplyChainDepartment = await _departmentService.GetSupplyChainDepartment();

            model.IsSupplyChainDepartment = supplyChainDepartment.DepartmentType == Entities.Domain.Departments.DepartmentType.SupplyChain;

            model.DepartmentId = supplyChainDepartment.Id;
            model.Date = dateSearch;

            #region attendance

            //var attendanceThisDay = _attendanceService.GetAttendancesByDateAndType(dateSearch.Date,
            //    AttendanceType.SupplyChain).FirstOrDefault();
            //AttendanceModel attendanceModel = null;

            //if (attendanceThisDay != null)
            //{
            //    attendanceModel = new AttendanceModel
            //                      {
            //                          UserCreated = attendanceThisDay.CreatedUserId,
            //                          CreatedDate = attendanceThisDay.CreatedDate,
            //                          Id = attendanceThisDay.Id,
            //                          Note = attendanceThisDay.Note,
            //                          Type = attendanceThisDay.Type.ToString(),
            //                          TypeId = (int)attendanceThisDay.Type,
            //                          ListUsernameInAttendance =
            //                              _attendanceService.GetUsernameInAttendance(attendanceThisDay.Id),
            //                          ListUserIdInAttendance =
            //                              _attendanceService.GetUserIdInAttendance(attendanceThisDay.Id),
            //                          ListUsernameNotInAttendance = _attendanceService.GetUsernameNotInAttendance(attendanceThisDay.Id),

            //                      };
            //    if (_workContext.CurrentUser.IsAdmin())
            //    {
            //        attendanceModel.ListUsernameInAttendance.Remove(_workContext.CurrentUser.Username);
            //    }
            //}



            #endregion

            #region production planning



            var listLineType = new List<PlanLineHardCodeType>()
                                   {
                                       PlanLineHardCodeType.Line1,
                                       PlanLineHardCodeType.Line2,
                                       PlanLineHardCodeType.Line3,
                                       PlanLineHardCodeType.Gabbana,
                                       PlanLineHardCodeType.FRMK3,
                                       PlanLineHardCodeType.FRMK4,
                                   };


            var listShiftType = new List<PlanShiftHardCodeType>()
                                    {
                                        PlanShiftHardCodeType.Shift1,
                                        PlanShiftHardCodeType.Shift2,
                                        PlanShiftHardCodeType.Shift3

                                    };

            for (int i = 0; i < 7; i++)
            {
                foreach (var shift in listShiftType)
                {
                    var submodel = new ProductPlanningNewModel()
                    {
                        DateTime = dateSearch.AddDays(i).ToShortDateString(),
                        ShiftType = shift.ToString(),
                    };

                    foreach (var line in listLineType)
                    {
                        {
                            var productPlanning = await
                                _productPlanningService.GetProductPlanningByDateAndShiftAndLine(
                                    dateSearch.AddDays(i),
                                    shift, line);

                            var productPlanningResult = productPlanning != null ? productPlanning.Result : "";
                            var productName = productPlanning != null ? productPlanning.ProductName : "#ffff";

                            var newProductLineResult = new ProductLineResult
                            {
                                LineResult = productPlanningResult,
                                LineType = line.ToString(),
                                ProductName = productName,
                                Color = GetColorByProductName(productName)
                            };
                            submodel.ListProductLineResult.Add(newProductLineResult);
                        }

                    }
                    model.ProductionPlanning.Add(submodel);
                }

            }

            #endregion

            //model.AttendanceModel = attendanceModel;

            #region HSE

            //Get all user
            var allUser = _userService.GetAllUsersAsync();

            // Get last user
            var owner = allUser.LastOrDefault();

            // Get user admin
            var currentUser = await _userService.GetUserByUsernameAsync("Admin");

            var supplyChainHSE = new List<SupplyChainHSEModel>();


            supplyChainHSE = new List<SupplyChainHSEModel>()
                                     {
                                         new SupplyChainHSEModel()
                                         {
                                             // Todo: check again
                                             CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                             // Todo: check again
                                             CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                             // Todo: check again
                                             CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
            // Get supply chain have measure code = target
            var allsupplyChain =
                _supplyChainHSE.GetAllSupplyChainHSEs()
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.CreatedDate<= dateSearch).OrderBy(p => p.CreatedDate)
                    .ToList();

            if (allsupplyChain.Count == 0)
            {
                // Create new model default
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
                // Get supply chain last
                var lastDayTarget = allsupplyChain.Last();
                if (lastDayTarget.CreatedDate.Date != dateSearch)
                {

                    // Create model last day to current day
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
                // Get supply chain in list
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
                    var yesterdayHSE = dateSearch.AddDays(-1).AddHours(23);
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
                            var lastDayTarget = allsupplyChainHSEOwner.OrderBy(p => p.CreatedDate).Last();
                            if (lastDayTarget.CreatedDate.Date != dateSearch)
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 // Todo: check again
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 1 && p.CreatedDate<=dateSearch).OrderBy(p=>p.Id)
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
                var lastDayTarget = allsupplyChainMaking.OrderBy(p => p.CreatedDate).Last();
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
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainDDSMeasure.Target && p.type == 2 && p.CreatedDate<=dateSearch)
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
                var lastDayTarget = allsupplyChainPacking.OrderBy(p => p.UpdatedDate).Last();
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
                            var lastDayTarget = allsupplyChainDDSOwner.OrderBy(p => p.UpdatedDate).Last();
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
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                 CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainFPQMeasure.Target && p.CreatedDate<= dateSearch)
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
                var lastDayTarget = allsupplyChainFPQ.OrderBy(p => p.UpdatedDate).Last();
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

                    if (item.ListUsernameInSupplyChainFPQ.Count==0)
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
                            SupplyChainFpqId = supplychainResult.Id,
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
                                                                                  SupplyChainFpqId =newModel.Id,
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
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                     MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.MTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.MTD,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 1
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Target,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                     MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("Service",SupplyChainServiceMeasure.Daily
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                     MeasureCode = (int) SupplyChainServiceMeasure.Daily,
                                                     CreatedDate = dateSearch.ToShortDateString(),
                                                     UpdatedDate = dateSearch.ToShortDateString(),
                                                     type = 2
                                                 },
                                                 new SupplyChainServiceModel()
                                                 {
                                                     CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 1 && p.CreatedDate <= dateSearch)
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
                var lastDayTarget = allsupplyChainFE.OrderBy(p => p.CreatedDate).Last();
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
          

            var allsupplyChainFR =
              _supplyChainService.GetAllSupplyChainServices()
                  .Result.Where(p => p.MeasureCode == (int)SupplyChainServiceMeasure.Target && p.type == 2 && p.CreatedDate <= dateSearch).OrderBy(p => p.CreatedDate) 
                  .ToList();

            if (allsupplyChainFR.Count == 0)
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
                if (dateSearch > allsupplyChainFR.LastOrDefault().CreatedDate && dateSearch < currentDate.AddDays(1))
                {
                    var lastDayTarget = allsupplyChainFR.OrderBy(p => p.CreatedDate).Last();
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                                CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                    item.Shift1UIColorBg = GetColorByProductResult(supplychainResult.Shift1);
                    item.Shift2UIColorBg = GetColorByProductResult(supplychainResult.Shift2);
                    item.Shift3UIColorBg = GetColorByProductResult(supplychainResult.Shift3);
                    await _supplyChainProductionPlanning.UpdateAsync(supplychainResult);

                }
                else
                {
                    if (item.type == 1)
                    {
                        var allsupplyChainProductionPlanningOwner =
                            _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode).Where(p=>p.CreatedDate<=dateSearch)
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
                                var lastDayTarget = allsupplyChainProductionPlanningOwner.OrderBy(p => p.CreatedDate).Last();
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
                        var allsupplyChainProductionPlanning=
                            _supplyChainProductionPlanning.GetAllSupplyChainProductionPlannings()
                                .Result.Where(p => p.MeasureCode == item.MeasureCode).Where(p=>p.CreatedDate<=dateSearch)
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
                                var lastDayTarget = allsupplyChainProductionPlanning.OrderBy(p => p.CreatedDate).Last();
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
                                                  MeasureName =_measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("MPSA",SupplyChainMPSAMeasure.Target.ToString()).Result.MeasureSupplyChainName,
                                                  MeasureCode = (int) SupplyChainMPSAMeasure.Target,
                                                  CreatedDate = dateSearch.ToShortDateString(),
                                                  UpdatedDate = dateSearch.ToShortDateString(),
                                                 Remark = string.Empty,
                                                 RemarkDisplay = string.Empty
                                              },
                                              new SupplyChainMPSAModel()
                                              {
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                                                  CreatedUser = _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username,
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
                    .Result.Where(p => p.MeasureCode == (int)SupplyChainMPSAMeasure.Target).Where(p => p.CreatedDate <= dateSearch)
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
                var lastDayTarget = allsupplyChainMPSA.OrderBy(p => p.CreatedDate).Last();
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
                            if (listOldOwner!=null)
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

            var permissionIssues = await _permissionService.Authorize(PermissionProvider.WriteIssues, _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username);
            var permissionAttendance = await _permissionService.Authorize(PermissionProvider.WriteAttendance, _workContext.CurrentUser == null ? "" : _workContext.CurrentUser.Username);

            model.permissionIssue = permissionIssues;
            model.permissionAttendance = permissionAttendance;

            if (_workContext.CurrentUser!=null)
            {

                if (_workContext.CurrentUser.IsAdmin())
                {
                    
                    return View("SupplyChainAdmin", model);
                }
                else
                {
                    if (dateSearch.Date > DateTime.Now.Date || dateSearch < DateTime.Now.Date)
                    {
                        return View("SupplyChainEmployeeNotUpdate", model);
                    }
                    return View("SupplyChainEmployee", model);
                }
            }
            return View("SupplyChainForGuest", model);
        }

        public ActionResult Result(DateTime date)
        {
            var model = new SupplyChainModel
            {
                Date = date
            };
            return PartialView("_Result", model);
        }
        public ActionResult Hse(DateTime date)
        {
            var ddsMeeting = _ddsMeetingService.GetDdsMeetingByDate(date, SupplyChainDepartmentId).Result;
            var model = new HseModel();
            //if (ddsMeeting != null)
            //{
            //    var hseDms = _dmsService.GetDmsByTypeAndDepartmentId(DmsType.HSE, SupplyChainDepartmentId).Result;
            //    var details = ddsMeeting.SupplyChainDetails.Where(d => d.DmsId == hseDms.Id);

            //    var bosCompleteMeasure = hseDms.Measures.FirstOrDefault(m => m.MeasureSystemTypeId == (int)MeasureSystemType.BosComplete);
            //    if (bosCompleteMeasure != null)
            //    {
            //        model.BosComplete = bosCompleteMeasure.MeasureName;
            //        model.Target = GetSupplyChangeValue(details, (int) SupplyChainOptionType.Enum, (int) SupplyChainOptions.Target);
            //        model.TargetRemark = GetSupplyChangeValue(details, (int) SupplyChainOptionType.Enum, (int) SupplyChainOptions.TargetRemark);
            //        model.BosToday = GetSupplyChangeValue(details, (int) SupplyChainOptionType.Enum, (int) SupplyChainOptions.BosToday);
            //        model.BosDone = GetSupplyChangeValue(details, (int) SupplyChainOptionType.Enum, (int) SupplyChainOptions.BoisDone);
            //        model.BosCompleteOwnerId = int.Parse(GetSupplyChangeValue(details, bosCompleteMeasure.Id, "Owner"));
            //    }
            //    var bosUnsafeMeasure = hseDms.Measures.FirstOrDefault(m => m.MeasureSystemTypeId == (int)MeasureSystemType.BosUnsafe);
            //    if (bosUnsafeMeasure != null)
            //    {
            //        model.BosUnsafe = bosUnsafeMeasure.MeasureName;
            //        model.BosUnsafeCommon = GetSupplyChangeValue(details, (int) SupplyChainOptionType.Enum, (int) SupplyChainOptions.CommonRemark);
            //        model.BosUnsafeRemark = GetSupplyChangeValue(details, bosUnsafeMeasure.Id, "Remark");
            //        model.BosUnsafeOwnerId = int.Parse(GetSupplyChangeValue(details, bosUnsafeMeasure.Id, "Owner"));
            //        var departments = _departmentService.SearchDepartment(isActive:true);
            //        foreach (var department in departments)
            //        {
            //            model.BosUnsafeDeps.Add(new HseModel.HseDepartmentModel
            //            {
            //                DepartmentId = department.Id,
            //                DepartmentName = department.Name,
            //                Value = GetSupplyChangeValue(details, (int)SupplyChainOptionType.Department, department.Id)
            //            });
            //        }
            //    }
            //}
            return PartialView("_Hse",model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Hse(FormCollection form, DateTime date)
        {
            return Json(new object());
        }
        //private string GetSupplyChangeValue(IEnumerable<SupplyChainDetail> details, int opionTypeId, int optionId)
        //{
        //    var detail = details.FirstOrDefault(d => d.OptionTypeId == opionTypeId && d.OptionId == optionId);
        //    return detail == null ? "" : detail.OptionValue;
        //}
        //private string GetSupplyChangeValue(IEnumerable<SupplyChainDetail> details, int measureId, string optionName)
        //{
        //    var detail = details.FirstOrDefault(d => d.MeasureId == measureId && d.OptionName == optionName);
        //    return detail == null ? "" : detail.OptionValue;
        //}
        public ActionResult Fpq(DateTime date)
        {
            var model = new FpqModel();

            model.Measures.Add(new FpqModel.FpqMeasureModel
            {
                Name = "Test"
            });

            return PartialView("_Fpq", model);
        }
        public ActionResult Mpsa(DateTime date)
        {
            var model = new MpsaModel();
            return PartialView("_Mpsa", model);
        }

        public ActionResult Dds(DateTime date)
        {
            return PartialView("_Dds");
        }

        public ActionResult Service(DateTime date)
        {
            return PartialView("_Service");
        }
        public async Task<ActionResult> ProductPlanning(DateTime date)
        {
            return PartialView("_ProductPlanning");
        }


        public async Task<JsonResult> UpdateHSE(SupplyChainHSEModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainHSE.GetSupplyChainHSEMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainHSE()
                                          {
                                              CommonArea = model.CommonArea,
                                              CreatedDate = model.Date,
                                              Owner = model.OwnerId,
                                              UpdatedDate = DateTime.Now,
                                              MeasureCode = model.MeasureCode,
                                              Making = model.Making,
                                              Packing = model.Packing,
                                              Remarks = model.Remarks ?? "",
                                              
                                          };
                await _supplyChainHSE.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CommonArea = model.CommonArea;
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.Owner = model.Owner;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.Making = model.Making;
                supplychainResult.Packing = model.Packing;
                supplychainResult.Remarks = model.Remarks ?? "";

                await _supplyChainHSE.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "create" });
            }
        }
         
        public async Task<JsonResult> UpdateDDS(SupplyChainDDSModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(model.MeasureCode.ToString(), model.Date, model.type);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainDDS()
                {

                    CreatedDate = model.Date,
                    Owner = model.Owner,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = model.type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    LPD1 = model.LPD1,
                    LPD2 = model.LPD2,
                    LPD3 = model.LPD3,
                    FRPK = model.FRPK,
                    Pouch = model.Pouch,
                    Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>"),
                    FRMK = model.FRMK,
                    FE = model.FE,
                    Bottle = model.Bottle,
                    Sachet = model.Sachet,
                    Batch = model.Batch
                };
                await _supplyChainDDS.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.Owner = model.Owner;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.type = model.type;
                supplychainResult.LPD1 = model.LPD1;
                supplychainResult.LPD2 = model.LPD2;
                supplychainResult.LPD3 = model.LPD3;
                supplychainResult.FRPK = model.FRPK;
                supplychainResult.Pouch = model.Pouch;
                supplychainResult.Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>");
                supplychainResult.FRMK = model.FRMK;
                supplychainResult.FE = model.FE;
                supplychainResult.Bottle = model.Bottle;
                supplychainResult.Sachet = model.Sachet;
                supplychainResult.Batch = model.Batch;
                await _supplyChainDDS.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }


        }

        public async Task<JsonResult> UpdateDDSFromPacking(SupplyChainDDSUpdatePackingModel model)
        {
            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(model.MeasureCode.ToString(), model.Date, 2);
            var currentUser = _workContext.CurrentUser;
            var allUser = _userService.GetAllUsersAsync();
            var owner = allUser.LastOrDefault();

            if (supplychainResult == null)
            {
                var newModel = new Entities.Domain.SupplyChainDDS()
                {
                    CreatedDate = model.Date,

                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = 2,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id
                };

                if (model.LineCode == (int)LineHardCodeType.Sac1)
                {
                    newModel.Sachet = model.Value;
                    newModel.Sachet1 = model.Value;
                    newModel.Sachet1Remark = model.Remark;
                    newModel.SachetRemark = model.Remark;
                }

                if (model.LineCode == (int)LineHardCodeType.Sac2)
                {
                    newModel.Sachet2 = model.Value;
                    newModel.Sachet2Remark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Bottle)
                {
                    newModel.Bottle = model.Value;
                    newModel.BottleRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Pou)
                {
                    newModel.Pouch = model.Value;
                    newModel.PouchRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {
                    newModel.FRPK = model.Value;
                    newModel.FRPKRemark = model.Remark;
                }


                // Owner
                var allsupplyChainDDSOwner =
                        _supplyChainDDS.GetAllSupplyChainDDSs()
                            .Result.Where(p => p.MeasureCode == model.MeasureCode && p.type == 2).OrderBy(p => p.UpdatedDate)
                            .ToList();
                if (allsupplyChainDDSOwner.Count == 0)
                {
                    newModel.Owner = owner == null ? "" : owner.Username;
                }
                else
                {
                    if (model.Date > allsupplyChainDDSOwner.LastOrDefault().CreatedDate)
                    {
                        var lastDayTarget = allsupplyChainDDSOwner.Last();
                        if (lastDayTarget.CreatedDate.Date != model.Date.Date)
                        {
                            newModel.Owner = lastDayTarget.Owner;
                        }
                    }
                }


                await _supplyChainDDS.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.UpdatedDate = model.Date;
                if (model.LineCode == (int)LineHardCodeType.Sac1)
                {
                    supplychainResult.Sachet = model.Value;
                    supplychainResult.Sachet1 = model.Value;
                    supplychainResult.Sachet1Remark = model.Remark;
                    supplychainResult.SachetRemark = model.Remark;
                }

                if (model.LineCode == (int)LineHardCodeType.Sac2)
                {
                    supplychainResult.Sachet2 = model.Value;
                    supplychainResult.Sachet2Remark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Bottle)
                {
                    supplychainResult.Bottle = model.Value;
                    supplychainResult.BottleRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Pou)
                {
                    supplychainResult.Pouch = model.Value;
                    supplychainResult.PouchRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {
                    supplychainResult.FRPK = model.Value;
                    supplychainResult.FRPKRemark = model.Remark;
                }


                await _supplyChainDDS.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }


        }

        public async Task<JsonResult> UpdateDDSFromMaking(SupplyChainDDSUpdateMakingModel model)
        {

            var supplychainResult = _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType(model.MeasureCode.ToString(), model.Date, 1);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainDDS()
                {

                    CreatedDate = model.Date,

                    UpdatedDate = model.Date,
                    MeasureCode = model.MeasureCode,
                    type = 1,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                   
                };
                if (model.LineCode == (int)LineHardCodeType.LPD1)
                {
                    newModel.LPD1 = model.Value;

                }
                if (model.LineCode == (int)LineHardCodeType.LPD2)
                {
                    newModel.LPD2 = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.LPD3)
                {
                    newModel.LPD3 = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.FEBatch)
                {
                    newModel.Batch = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.FRMK)
                {
                    newModel.FRMK = model.Value;
                }


                // Owner

                var allUser = _userService.GetAllUsersAsync();
                var owner = allUser.LastOrDefault();

                var allsupplyChainDDSOwner =
                        _supplyChainDDS.GetAllSupplyChainDDSs()
                            .Result.Where(p => p.MeasureCode == model.MeasureCode && p.type == 1)
                            .ToList();
                if (allsupplyChainDDSOwner.Count == 0)
                {
                    newModel.Owner = owner == null ? "" : owner.Username;
                }
                else
                {
                    if (model.Date > allsupplyChainDDSOwner.LastOrDefault().CreatedDate)
                    {
                        var lastDayTarget = allsupplyChainDDSOwner.Last();
                        if (lastDayTarget.CreatedDate.Date != model.Date.Date)
                        {
                            newModel.Owner = lastDayTarget.Owner;
                        }
                    }
                }


                await _supplyChainDDS.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.UpdatedDate = model.Date;
                if (model.LineCode == (int)LineHardCodeType.LPD1)
                {
                    supplychainResult.LPD1 = model.Value;
                    
                }
                if (model.LineCode == (int)LineHardCodeType.LPD2)
                {
                    supplychainResult.LPD2 = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.LPD3)
                {
                    supplychainResult.LPD3 = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.FEBatch)
                {
                    supplychainResult.Batch = model.Value;
                }
                if (model.LineCode == (int)LineHardCodeType.FRMK)
                {
                    supplychainResult.FRMK = model.Value;
                }


                await _supplyChainDDS.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }


        }


        public async Task<JsonResult> UpdateFPQFromPacking(SupplyChainFPQUpdatePackingModel model)
        {
            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainFPQ()
                {

                    CreatedDate = model.Date,

                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = model.type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,

                };

                if (model.LineCode == (int)LineHardCodeType.Sac1)
                {
                    var Sachet1 = 0;
                    try
                    {
                        Sachet1 = int.Parse(model.Value);
                    }
                    catch (Exception)
                    {

                        Sachet1 = 0;
                    }
                    var Sachet2 = 0;
                    try
                    {
                        Sachet2 = int.Parse(newModel.Sachet2);
                    }
                    catch (Exception)
                    {

                        Sachet2 = 0;
                    }
                    newModel.Sachet1 = model.Value;
                    newModel.SachetRemark = model.Remark;
                    newModel.Sachet1Remark = model.Remark;
                    newModel.Sachet = (Sachet1 + Sachet2).ToString();

                }

                if (model.LineCode == (int)LineHardCodeType.Sac2)
                {
                    var Sachet1 = 0;
                    try
                    {
                        Sachet1 = int.Parse(newModel.Sachet1);
                    }
                    catch (Exception)
                    {

                        Sachet1 = 0;
                    }
                    var Sachet2 = 0;
                    try
                    {
                        Sachet2 = int.Parse(model.Value);
                    }
                    catch (Exception)
                    {

                        Sachet2 = 0;
                    }
                    newModel.Sachet2 = model.Value;
                    newModel.SachetRemark = model.Remark;
                    newModel.Sachet2Remark = model.Remark;
                    newModel.Sachet = (Sachet1 + Sachet2).ToString();
                }

                if (model.LineCode == (int)LineHardCodeType.Bottle)
                {
                    newModel.Bottle = model.Value;
                    newModel.BottleRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Pou)
                {
                    newModel.Pouch = model.Value;
                    newModel.PouchRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {
                    newModel.FR = model.Value;
                    newModel.FRRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {

                }
                await _supplyChainFPQ.CreateAsync(newModel);
                var yesterdayFPQ = DateTime.Now.AddDays(-1).AddHours(23);
                var allsupplyChainFPQOwner =
                 _supplyChainFPQ.GetAllSupplyChainFPQs()
                     .Result.Where(p => p.MeasureCode == model.MeasureCode && p.CreatedDate <= yesterdayFPQ).OrderBy(p => p.UpdatedDate)
                     .ToList();
                var listOldOwner =
                              _supplyChainFPQ.GetUserIdInSupplyChainFPQ(allsupplyChainFPQOwner.Last().Id);

                foreach (var itemOwner in listOldOwner)
                {
                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                    {
                        SupplyChainFpqId =
                            newModel.Id,
                        UserId = itemOwner
                    });
                }
                 

             
                return Json(new { status = "success", type = "create" });
            }
            else
            {

                supplychainResult.UpdatedDate = DateTime.Now;
                if (model.LineCode == (int)LineHardCodeType.Sac1)
                {
                    var Sachet1 = 0;
                    try
                    {
                        Sachet1 = int.Parse(model.Value);
                    }
                    catch (Exception)
                    {

                        Sachet1 = 0;
                    }
                    var Sachet2 = 0;
                    try
                    {
                        Sachet2 = int.Parse(supplychainResult.Sachet2);
                    }
                    catch (Exception)
                    {

                        Sachet2 = 0;
                    }
                    supplychainResult.Sachet1 = model.Value;
                    supplychainResult.SachetRemark = model.Remark;
                    supplychainResult.Sachet1Remark = model.Remark;
                    supplychainResult.Sachet = (Sachet1 + Sachet2).ToString();

                }

                if (model.LineCode == (int)LineHardCodeType.Sac2)
                {
                    var Sachet1 = 0;
                    try
                    {
                        Sachet1 = int.Parse(supplychainResult.Sachet1);
                    }
                    catch (Exception)
                    {

                        Sachet1 = 0;
                    }
                    var Sachet2 = 0;
                    try
                    {
                        Sachet2 = int.Parse(model.Value);
                    }
                    catch (Exception)
                    {

                        Sachet2 = 0;
                    }
                    supplychainResult.Sachet2 = model.Value;
                    supplychainResult.SachetRemark = model.Remark;
                    supplychainResult.Sachet2Remark = model.Remark;
                    supplychainResult.Sachet = (Sachet1 + Sachet2).ToString();
                }

                if (model.LineCode == (int)LineHardCodeType.Bottle)
                {
                    supplychainResult.Bottle = model.Value;
                    supplychainResult.BottleRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.Pou)
                {
                    supplychainResult.Pouch = model.Value;
                    supplychainResult.PouchRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {
                    supplychainResult.FR = model.Value;
                    supplychainResult.FRRemark = model.Remark;
                }
                if (model.LineCode == (int)LineHardCodeType.FRPK)
                {

                }

                await _supplyChainFPQ.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }


        }

        public async Task<JsonResult> UpdateFPQ(SupplyChainFPQModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;

            var supplychainResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainFPQ()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = model.type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    LPD1 = model.LPD1,
                    LPD2 = model.LPD2,
                    LPD3 = model.LPD3,
                    FRMK = model.FRMK,
                    FR = model.FR,
                    Pouch = model.Pouch,
                    Remark = model.Remark.Replace("\n", "<br/>"),
                    Bottle = model.Bottle,
                    Sachet = model.Sachet,
                    Batch = model.Batch
                };
                await _supplyChainFPQ.CreateAsync(newModel);
                supplychainResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(model.MeasureCode.ToString(), DateTime.Now);
                foreach (var userNAme in model.ListUsernameInSupplyChainFPQ)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                    {
                        SupplyChainFpqId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.type = model.type;
                supplychainResult.LPD1 = model.LPD1;
                supplychainResult.LPD2 = model.LPD2;
                supplychainResult.LPD3 = model.LPD3;
                supplychainResult.FR = model.FR;
                supplychainResult.Pouch = model.Pouch;
                supplychainResult.Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>");
                supplychainResult.Bottle = model.Bottle;
                supplychainResult.Sachet = model.Sachet;
                supplychainResult.Batch = model.Batch;
                supplychainResult.FRMK = model.FRMK;

                await _supplyChainFPQ.UpdateAsync(supplychainResult);
                await _supplyChainFPQ.DeletaAllUserInSupplyChainFPQ(supplychainResult.Id);
                //create user
                foreach (var userNAme in model.ListUsernameInSupplyChainFPQ)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                    {
                        SupplyChainFpqId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateService(SupplyChainServiceModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainService.GetSupplyChainServiceMeasureCodeAndDateAndType(model.MeasureCode.ToString(), model.Date, model.type);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);
            var supplychainOwner = _supplyChainService.GetSupplyChainServiceByDate(model.Date);
            if (!String.IsNullOrEmpty(model.Owner))
            {
                foreach (var item in supplychainOwner.Result)
                {
                    item.Owner = model.Owner;
                    await _supplyChainService.UpdateAsync(item);
                }
            }
            if (supplychainResult == null)
            {
                var newModel = new Entities.Domain.SupplyChainService()
                {

                    CreatedDate = model.Date,
                    Owner = model.Owner,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = model.type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    CFR = model.CFR,
                    PriorityLine = model.PriorityLine,
                    PrioritySKU = model.PrioritySKU,
                    SAMBC = model.SAMBC,
                    Shipment = model.Shipment,
                };
                await _supplyChainService.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.Owner = model.Owner;
               
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.type = model.type;
                supplychainResult.Owner = model.Owner;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.UserUpdatedId = currentUser.Id;
                supplychainResult.type = model.type;
                supplychainResult.CFR = model.CFR;
                supplychainResult.PriorityLine = model.PriorityLine;
                supplychainResult.PrioritySKU = model.PrioritySKU;
                supplychainResult.SAMBC = model.SAMBC;
                supplychainResult.Shipment = model.Shipment;
                await _supplyChainService.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateProductionPlanning(SupplyChainProductionPlanningModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainProductionPlanning.GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(model.MeasureCode.ToString(), model.Date, model.type);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainProductionPlanning()
                {

                    CreatedDate = model.Date,
                    Owner = model.Owner,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    type = model.type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    MTD = model.MTD,
                    MonthTarget = model.MonthTarget,
                    Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>"),
                    Shift1 = model.Shift1,
                    Shift2 = model.Shift2,
                    Shift3 = model.Shift3,
                    TodayPlan = model.TodayPlan,
                    Gap = model.Gap,
                };

                await _supplyChainProductionPlanning.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.Owner = model.Owner;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.MTD = model.MTD;
                supplychainResult.MonthTarget = model.MonthTarget;
                supplychainResult.Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>");
                supplychainResult.Shift1 = model.Shift1;
                supplychainResult.Shift2 = model.Shift2;
                supplychainResult.Shift3 = model.Shift3;
                supplychainResult.TodayPlan = model.TodayPlan;
                supplychainResult.UserUpdatedId = currentUser.Id;
                supplychainResult.Gap = model.Gap;
                await _supplyChainProductionPlanning.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateMPSA(SupplyChainMPSAModel model)
        {
            var checkDate = new DateTime();
            if (model.Date == checkDate)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>"),
                    Bottle = model.Bottle,
                    Sachet1 = model.Sachet1,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR
                };
                await _supplyChainMPSA.CreateAsync(newModel);
                supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), DateTime.Now);
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.CreatedDate = model.Date;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = model.MeasureCode;
                supplychainResult.Pouch = model.Pouch;
                supplychainResult.Remark = model.Remark == null ? "" : model.Remark.Replace("\n", "<br/>");
                supplychainResult.Bottle = model.Bottle;
                supplychainResult.Sachet1 = model.Sachet1;
                supplychainResult.Sachet2 = model.Sachet2;
                supplychainResult.MPSAFE = model.MPSAFE;
                supplychainResult.MPSAFR = model.MPSAFR;
                supplychainResult.FR = model.FR;
                supplychainResult.UserUpdatedId = currentUser.Id;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                await _supplyChainMPSA.DeletaAllUserInSupplyChainMPSA(supplychainResult.Id);
                //create user
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateMPSABottle(SupplyChainMPSAModel model)
        {
            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);
            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    BottleRemarks = model.Remark,
                    Bottle = model.Bottle,
                    Sachet1 = model.Sachet1,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR
                };
                newModel.Remark = MPSAReMarks(newModel);
                await _supplyChainMPSA.CreateAsync(newModel);


                var yesterdayMPSA = model.Date.AddDays(-1).AddHours(23);
                var allsupplyChainMPSAOwner =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == newModel.MeasureCode && p.CreatedDate <= yesterdayMPSA).OrderBy(p => p.UpdatedDate)
                        .ToList();

                var listOldOwner =
                                 _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);
            

                foreach (var itemOwner in listOldOwner)
                {
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId =
                            newModel.Id,
                        UserId = itemOwner
                    });
                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.BottleRemarks = model.Remark;
                supplychainResult.Bottle = model.Bottle;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateMPSAPouch(SupplyChainMPSAModel model)
        {

            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new SupplyChainMPSA()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    PouchRemark = model.Remark,
                    Bottle = model.Bottle,
                    Sachet1 = model.Sachet1,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR
                };
                newModel.Remark = MPSAReMarks(newModel);
                await _supplyChainMPSA.CreateAsync(newModel);
                supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                var yesterdayMPSA = model.Date.AddDays(-1).AddHours(23);
                var allsupplyChainMPSAOwner =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == newModel.MeasureCode && p.CreatedDate <= yesterdayMPSA).OrderBy(p => p.UpdatedDate)
                        .ToList();

                var listOldOwner =
                                 _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);


                foreach (var itemOwner in listOldOwner)
                {
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId =
                            newModel.Id,
                        UserId = itemOwner
                    });
                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.PouchRemark = model.Remark;
                supplychainResult.Pouch = model.Pouch;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }
        }

        public async Task<JsonResult> UpdateMPSASachet1(SupplyChainMPSAModel model)
        {

            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    Sachet1 = model.Sachet1,
                    Bottle = model.Bottle,
                    Sachet1Remarks = model.Remark,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR
                };
                newModel.Remark = MPSAReMarks(newModel);
                await _supplyChainMPSA.CreateAsync(newModel);
                supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                var yesterdayMPSA = model.Date.AddDays(-1).AddHours(23);
                var allsupplyChainMPSAOwner =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == newModel.MeasureCode && p.CreatedDate <= yesterdayMPSA)
                        .ToList();

                var listOldOwner =
                                 _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);


                foreach (var itemOwner in listOldOwner)
                {
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId =
                            newModel.Id,
                        UserId = itemOwner
                    });
                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.Sachet1Remarks = model.Remark;
                supplychainResult.Sachet1 = model.Sachet1;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateMPSASachet2(SupplyChainMPSAModel model)
        {
            var checkDate = new DateTime();
            if (checkDate == model.Date)
                model.Date = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), model.Date);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = model.Date,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    Sachet2Remarks = model.Remark,
                    Bottle = model.Bottle,
                    Sachet1 = model.Sachet1,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR
                };
                newModel.Remark = MPSAReMarks(newModel);
                await _supplyChainMPSA.CreateAsync(newModel);
                supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), DateTime.Now);
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = await _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Id
                    });

                }
                var yesterdayMPSA = model.Date.AddDays(-1).AddHours(23);
                var allsupplyChainMPSAOwner =
                    _supplyChainMPSA.GetAllSupplyChainMPSAs()
                        .Result.Where(p => p.MeasureCode == newModel.MeasureCode && p.CreatedDate <= yesterdayMPSA).OrderBy(p => p.UpdatedDate)
                        .ToList();

                var listOldOwner =
                                 _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);


                foreach (var itemOwner in listOldOwner)
                {
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId =
                            newModel.Id,
                        UserId = itemOwner
                    });
                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.Sachet2Remarks = model.Remark;
                supplychainResult.Sachet2 = model.Sachet2;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateMPSAFR(SupplyChainMPSAModel model)
        {
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), DateTime.Now);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                    Pouch = model.Pouch,
                    FRRemarks = model.Remark,
                    Bottle = model.Bottle,
                    Sachet1 = model.Sachet1,
                    Sachet2 = model.Sachet2,
                    MPSAFE = model.MPSAFE,
                    MPSAFR = model.MPSAFR,
                    FR = model.FR,

                };
                newModel.Remark = MPSAReMarks(newModel);
                await _supplyChainMPSA.CreateAsync(newModel);
                supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(model.MeasureCode.ToString(), DateTime.Now);
                foreach (var userNAme in model.ListUsernameInSupplyChainMPSA)
                {
                    var user = _userService.GetUserByUsernameAsync(userNAme);
                    await _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                    {
                        SupplyChainMPSAId = supplychainResult.Id,
                        UserId = user.Result.Id
                    });

                }
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                supplychainResult.FRRemarks = model.Remark;
                supplychainResult.FR = model.FR;

                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> UpdateSupplyChainMPSATotalPO(SupplyChainMPSAUpdateTotalPo model)
        {
            var measureCode = (int)
                SupplyChainMPSAMeasure.TotalPO;
            var checkDate = new DateTime();
            if (checkDate == model.CreatedDate)
                model.CreatedDate = DateTime.Now;
            var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(measureCode.ToString()
                                                     , model.CreatedDate);
            var currentUser = await _userService.GetUserByUsernameAsync(_workContext.CurrentUser.Username);

            if (supplychainResult == null)
            {

                var newModel = new Entities.Domain.SupplyChainMPSA()
                {

                    CreatedDate = model.CreatedDate,
                    UpdatedDate = model.CreatedDate,
                    MeasureCode = model.MeasureCode,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,

                };
                if (model.linceCode == (int)LineHardCodeType.Bottle)
                {

                    newModel.Bottle = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Pou)
                {

                    newModel.Pouch = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Sac1)
                {

                    newModel.Sachet1 = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Sac2)
                {

                    newModel.Sachet2 = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.FRPK)
                {

                    newModel.FR = model.result;
                }
                await _supplyChainMPSA.CreateAsync(newModel);
                return Json(new { status = "success", type = "create" });
            }
            else
            {

                if (model.linceCode == (int)LineHardCodeType.Bottle)
                {

                    supplychainResult.Bottle = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Pou)
                {

                    supplychainResult.Pouch = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Sac1)
                {

                    supplychainResult.Sachet1 = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.Sac2)
                {

                    supplychainResult.Sachet2 = model.result;
                }
                if (model.linceCode == (int)LineHardCodeType.FRPK)
                {

                    supplychainResult.FR = model.result;
                }
                await _supplyChainMPSA.UpdateAsync(supplychainResult);
                return Json(new { status = "success", type = "update" });
            }

        }

        public async Task<JsonResult> GetDataSupplyChainMPSADataPacking(int lineCode, DateTime date)
        {
            var checkDate = new DateTime();
            if (checkDate == date.Date)
                date = DateTime.Now;
            var listResult = new List<SupplyChainMPSADataPacking>();
            for (int i = 1; i < 4; i++)
            {
                var supplychainResult = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate((i + 2).ToString(), date);
                var model = new SupplyChainMPSADataPacking();
                if (supplychainResult != null)
                {

                    if (lineCode == (int)LineHardCodeType.Bottle)
                    {
                        model.result = supplychainResult.Bottle;
                        model.remarks = supplychainResult.BottleRemarks;
                    }
                    if (lineCode == (int)LineHardCodeType.Pou)
                    {
                        model.result = supplychainResult.Pouch;
                        model.remarks = supplychainResult.PouchRemark;
                    }
                    if (lineCode == (int)LineHardCodeType.Sac1)
                    {
                        model.result = supplychainResult.Sachet1;
                        model.remarks = supplychainResult.Sachet1Remarks;
                    }
                    if (lineCode == (int)LineHardCodeType.Sac2)
                    {
                        model.result = supplychainResult.Sachet2;
                        model.remarks = supplychainResult.Sachet2Remarks;
                    }
                    if (lineCode == (int)LineHardCodeType.FRPK)
                    {
                        model.result = supplychainResult.FR;
                        model.remarks = supplychainResult.FRRemarks;
                    }

                }
                listResult.Add(model);
            }
            var result = new DataSourceResult()
            {
                Data = listResult.AsEnumerable(), // Process data (paging and sorting applied)
                Total = 5 // Total number of records
            };

            // Return the result as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDataDDsForPacking(int lineCode, DateTime date)
        {
            var listResult = new List<SupplyChainDDSDataPackingModel>();
            var checkDate = new DateTime();
            if (checkDate == date.Date)
                date = DateTime.Now;
            var supplychainResultPRLastDay = _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType("2", date, 2);
            var supplychainResultPRMTD = _supplyChainDDS.GetSupplyChainDDSMeasureCodeAndDateAndType("3", date, 2);
           
            var model = new SupplyChainDDSDataPackingModel();

            if (supplychainResultPRLastDay != null)
            {
                

                    if (lineCode == (int)LineHardCodeType.Bottle)
                    {
                        model.PRLastDay = supplychainResultPRLastDay.Bottle;
                      
                        model.PRLastDayRemark = supplychainResultPRLastDay.BottleRemark;
                       
                    }
                    if (lineCode == (int)LineHardCodeType.Pou)
                    {
                        model.PRLastDay = supplychainResultPRLastDay.Pouch;
                      
                        model.PRLastDayRemark = supplychainResultPRLastDay.PouchRemark;
                       
                    }
                    if (lineCode == (int)LineHardCodeType.Sac1)
                    {
                        model.PRLastDay = supplychainResultPRLastDay.Sachet1;
                     
                        model.PRLastDayRemark = supplychainResultPRLastDay.Sachet1Remark;
                        
                    }
                    if (lineCode == (int)LineHardCodeType.Sac2)
                    {
                        model.PRLastDay = supplychainResultPRLastDay.Sachet2;

                        model.PRLastDayRemark = supplychainResultPRLastDay.Sachet2Remark;

                    }
                    if (lineCode == (int)LineHardCodeType.FRPK)
                    {
                        model.PRLastDay = supplychainResultPRLastDay.FRPK;
                       
                        model.PRLastDayRemark = supplychainResultPRLastDay.FRPKRemark;
                       
                    }


                
            }


            if (supplychainResultPRMTD != null)
            {


                if (lineCode == (int)LineHardCodeType.Bottle)
                {
                  
                    model.PRMTD = supplychainResultPRMTD.Bottle;
                 
                    model.PRMTDRemark = supplychainResultPRMTD.BottleRemark;
                }
                if (lineCode == (int)LineHardCodeType.Pou)
                {
                 
                    model.PRMTD = supplychainResultPRMTD.Pouch;
                  
                    model.PRMTDRemark = supplychainResultPRMTD.PouchRemark;
                }
                if (lineCode == (int)LineHardCodeType.Sac1)
                {
                   
                    model.PRMTD = supplychainResultPRMTD.Sachet1;
                   
                    model.PRMTDRemark = supplychainResultPRMTD.Sachet1Remark;
                }
                if (lineCode == (int)LineHardCodeType.Sac2)
                {
                    model.PRMTD = supplychainResultPRMTD.Sachet2;

                    model.PRMTDRemark = supplychainResultPRMTD.Sachet2Remark;

                }
                if (lineCode == (int)LineHardCodeType.FRPK)
                {
                   
                    model.PRMTD = supplychainResultPRMTD.FRPK;
                 
                    model.PRMTDRemark = supplychainResultPRMTD.FRPKRemark;
                }



            }
            listResult.Add(model);




            var result = new DataSourceResult()
            {
                Data = listResult.AsEnumerable(), // Process data (paging and sorting applied)
                Total = 5 // Total number of records
            };

            // Return the result as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDataQualityAlertForPacking(int lineCode, DateTime date)
        {
            var listResult = new List<SupplyChainFPQDataPackingModel>();
            var checkDate = new DateTime();
            if (checkDate == date.Date)
                date = DateTime.Now;
            var supplychainResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(((int)SupplyChainFPQMeasure.QuanlityBOS).ToString(), date);
            var model = new SupplyChainFPQDataPackingModel();

            if (supplychainResult != null)
            {
                

                    if (lineCode == (int)LineHardCodeType.Bottle)
                    {
                        model.result = supplychainResult.Bottle;
                        model.remark = supplychainResult.BottleRemark;
                    }
                    if (lineCode == (int)LineHardCodeType.Pou)
                    {
                        model.result = supplychainResult.Pouch;
                        model.remark = supplychainResult.PouchRemark;
                    }
                    if (lineCode == (int)LineHardCodeType.Sac1)
                    {
                        model.result = supplychainResult.Sachet1;
                        model.remark = supplychainResult.Sachet1Remark;

                    }

                    if (lineCode == (int)LineHardCodeType.Sac2)
                    {
                        model.result = supplychainResult.Sachet2;
                        model.remark = supplychainResult.Sachet2Remark;
                    }
                    if (lineCode == (int)LineHardCodeType.FRPK)
                    {
                        model.result = supplychainResult.FR;
                        model.remark = supplychainResult.FRRemark;
                    }



                
            }
            listResult.Add(model);

            var result = new DataSourceResult()
            {
                Data = listResult.AsEnumerable(), // Process data (paging and sorting applied)
                Total = 5 // Total number of records
            };

            // Return the result as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class SearchSupplyModel
        {
            public string Datetime { get; set; }

            public string SearchKeyword { get; set; }


        }


        #region funtion

        public string GetColorByProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                return "#ffff";
            productName = productName.TrimEnd().TrimStart();
           // var listProductionPlanningColor = _xmlService.GetAllProductionPlanningColors();
            foreach (var item in _listProductionPlanningColor)
            {
                if (productName.Contains(item.ProductionName))
                {
                    return item.Color;
                }
            }
            return "#ffff";
        }

        public string GetColorByProductResult(string productPlanningResult)
        {
            if (String.IsNullOrEmpty(productPlanningResult))
                return "#ffff";
            productPlanningResult = productPlanningResult.TrimEnd().TrimStart();

            foreach (var item in _listProductionPlanningColor)
            {
                if (productPlanningResult.ToUpper().StartsWith(item.ProductionName.ToUpper()))
                {
                    return item.Color;
                }
            }
            return "#ffff";
        }
        #endregion

        public string MPSAReMarks(SupplyChainMPSA mpsa)
        {
            var result = "";
            if (!String.IsNullOrEmpty(mpsa.FRRemarks))
            {
                result += "FR: " + mpsa.BottleRemarks + "</br>";
            }
            if (!String.IsNullOrEmpty(mpsa.BottleRemarks))
            {
                result += "Bottle: " + mpsa.BottleRemarks + "</br>";
                if (!String.IsNullOrEmpty(mpsa.Sachet1Remarks))
                {
                    result += "Sachet 1: " + mpsa.Sachet1Remarks + "</br>";
                }
                if (!String.IsNullOrEmpty(mpsa.Sachet2Remarks))
                {
                    result += "Sachet 2: " + mpsa.Sachet2Remarks + "</br>";
                }
                if (!String.IsNullOrEmpty(mpsa.PouchRemark))
                {
                    result += "Pouch: " + mpsa.PouchRemark + "</br>";
                }

            } return result;
        }
    }
}