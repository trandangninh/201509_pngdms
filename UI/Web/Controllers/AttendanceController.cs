using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Entities.Domain.Departments;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Service.Common;
using Service.Dds;
using Service.Departments;
using Service.Interface;
using Service.Lines;
using Service.Messages;
using Service.Security;
using Service.SupplyChain;
using Service.Users;
using Utils;
using Web.Models.Report;
using Service.Meetings;
using Web.Models.Attendance;
using Web.Models.SupplyChain;
using AttendanceModel = Web.Models.Attendance.AttendanceModel;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class AttendanceController : BaseController
    {

        //private readonly IAttendanceService _attendanceService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly ISendMailService _sendMailService;
        private readonly IWorkFlowMessageService _workFlowMessageService;
        private readonly IReportService _reportService;
        private readonly ILineService _lineService;
        private readonly IMeasureService _measureService;
        private readonly ISupplyChainHSEService _supplyChainHSE;
        private readonly ISupplyChainDDSService _supplyChainDDS;
        private readonly ISupplyChainFPQService _supplyChainFPQ;
        private readonly ISupplyChainMPSAService _supplyChainMPSA;
        private readonly ISupplyChainProductionPlanningService _supplyChainProductionPlanning;
        private readonly ISupplyChainServiceService _supplyChainService;
        //private readonly INoisMainMeasureService _noisMainMeasureService;
        private readonly IMeasureSupplyChainService _measureSupplyChainService;
        private readonly IWorkContext _workContext;
        private readonly IUserInMeetingService _userInMeetingService;
        private readonly IDdsMeetingService _ddsMeetingService;
        private readonly IDepartmentService _departmentService;




        private readonly List<string> _listMeasureForMaking = new List<string>()
                                                              {
                                                                  "SafetyTriggerCompliance ",
                                                                 "SafetyNearMiss",
                                                                 "QualityTriggerCompliance",
                                                                 "PrimaryQFactorIncompliance",
                                                                 "HoldReworkForDay",
                                                                 "DefectOpen",
                                                                 "TaskNotCompletedOnTime",
                                                                 "CLNotCompliance",
                                                                 "ScrapDueToCO",
                                                                 "RCOGTGOutOfTarget",
                                                                 "WorkOrderNotCompletionOnTime",
                                                                 "POMissed",
                                                                 "ProductionVolume",
                                                                "OfUnplannedStopsDayLineConstraint",
                                                                 "PRImpactedToPackingLines",
                                                                 "EOCoachingCompletion",
                                                                 "QualityAlert",
                                                                 "FEBulk7Days",
                                                                 "FEBulk3Days",
                                                                 "PCSAdditionSigmaIncompliance",
                                                                 "PCSCrTzCompliance",
                                                                 "PRYesterday",
                                                                 "PROfLPD1BatchProcess",
                                                                 "PROfLPD2ImpactedToPackingLine",
                                                                 "DDIError",
                                                              };
        private readonly List<string> ListMeasureForPacking = new List<string>()
                                                              {
                                                                  "SafetyTriggerCompliance",
                                                                  "SafetyNearMiss",
                                                                  "QualityTriggerCompliance",
                                                                  "PrimaryQFactorIncompliance",
                                                                  "HoldReworkForDay",
                                                                  "DefectOpen",
                                                                  "TaskNotCompletedOnTime",
                                                                  "CLNotCompliance",
                                                                  "ActualScrapOnLine",
                                                                  "AmountOfBulkProduceDay",
                                                                  "ScaptOnlineMSU",
                                                                  "RCOGTGOutOfTarget",
                                                                  "WorkOrderNotCompletionOnTime",
                                                                  "TotalPO",
                                                                  "POMissed",
                                                                  "MPSA",
                                                                  "UnplannedStopsDayLineConstraint",
                                                                  "PR",
                                                                  "EOCoachingCompletion",
                                                                  "QualityAlert",
                                                                  //"DDIError",
                                                                  "ContractorPlan",
                                                                  "DefectFoundByLineLeader",
                                                                  "DDI"
                                                              };

        public AttendanceController(
            //IAttendanceService attendanceService, 
            IUserService userService,
            IPermissionService permissionService, 
            //IUserAllowInMeetingService userAllowInMeetingService,
            ISendMailService sendMailService, IWorkFlowMessageService workFlowMessageService, 
            IReportService reportService, ILineService lineService, IMeasureService measureService, 
            //INoisMainMeasureService noisMainMeasureService, 
            ISupplyChainHSEService supplyChainHse, 
            ISupplyChainDDSService supplyChainDds, ISupplyChainFPQService supplyChainFpq, 
            ISupplyChainMPSAService supplyChainMpsa, ISupplyChainProductionPlanningService supplyChainProductionPlanning, 
            IMeasureSupplyChainService measureSupplyChainService, ISupplyChainServiceService supplyChainService, 
            IWorkContext workContext, IUserInMeetingService userInMeetingService, IDdsMeetingService ddsMeetingService, IDepartmentService departmentService)
        {
            //_attendanceService = attendanceService;
            _userService = userService;
            _permissionService = permissionService;
            //_userAllowInMeetingService = userAllowInMeetingService;
            _sendMailService = sendMailService;
            _workFlowMessageService = workFlowMessageService;
            _reportService = reportService;
            _lineService = lineService;
            _measureService = measureService;
            //_noisMainMeasureService = noisMainMeasureService;
            _supplyChainHSE = supplyChainHse;
            _supplyChainDDS = supplyChainDds;
            _supplyChainFPQ = supplyChainFpq;
            _supplyChainMPSA = supplyChainMpsa;
            _supplyChainProductionPlanning = supplyChainProductionPlanning;
            _measureSupplyChainService = measureSupplyChainService;
            _supplyChainService = supplyChainService;
            _workContext = workContext;
            _userInMeetingService = userInMeetingService;
            _ddsMeetingService = ddsMeetingService;
            _departmentService = departmentService;
        }


        //
        // GET: /Attendance/
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ViewAttendance))
                return AccessDeniedView();

            var departments = _departmentService.SearchDepartment(null,true);

            var deps = new List<Department>();

            foreach (var d in departments)
            {
                if(_permissionService.Authorize(PermissionProvider.ViewAttendance))
                    deps.Add(d);
            }

            var model = new SearchAttendanceModel
            {
                DepartmentId = 0,
                AvailableDepartments = deps.Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            model.AvailableDepartments.Insert(0, new SelectListItem{Text = "-- Any Department --", Value = "0"});

            return View(model);
        }

        //[HttpPost]
        //public async Task<ActionResult> UpdateAttendance([System.Web.Http.FromBody] AttendanceModel model)
        //{
        //    //if (String.IsNullOrEmpty(model.Note))
        //    //    return Json(new {status="failed"});
        //    if (model.Id <= 0)
        //        return Json(new {status = "failed"});
        //    if (model.ListUserIdInAttendance == null || model.ListUserIdInAttendance.Count == 0)
        //        return Json(new {status = "failed"});
        //    var attendance = await _attendanceService.GetByIdAsync(model.Id);
        //    if (attendance == null) return Json(new {status = "failed"});

        //    attendance.Note = model.Note;
        //    await _attendanceService.UpdateAsync(attendance);

        //    //delete old user
        //    await _attendanceService.DeletaAllUserInAttendance(attendance.Id);

        //    //create user
        //    foreach (var userId in model.ListUserIdInAttendance)
        //    {
        //        await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                             {
        //                                                                 AttendanceId = model.Id,
        //                                                                 UserId = userId
        //                                                             });
        //    }
        //    var attendanceModel = new AttendanceModel
        //                          {
        //                              UserCreated = attendance.CreatedUserId,
        //                              CreatedDate = attendance.CreatedDate,
        //                              Id = attendance.Id,
        //                              Note = attendance.Note,
        //                              Type = attendance.Type.ToString(),
        //                              TypeId = (int) attendance.Type,
        //                              ListUsernameInAttendance =
        //                                  _attendanceService.GetUsernameInAttendance(attendance.Id),
        //                              ListUserIdInAttendance = _attendanceService.GetUserIdInAttendance(attendance.Id)
        //                          };
        //    return Json(new {status = "success", model = attendanceModel});
        //}

        //public async Task<ActionResult> CreateAttendance(AttendanceModel model)
        //{
        //    //if (String.IsNullOrEmpty(model.Note))
        //    //    return Json(new { status = "failed" });
        //    if (model.ListUserIdInAttendance == null || model.ListUserIdInAttendance.Count == 0)
        //        return Json(new {status = "failed"});
        //    if (model.Id > 0)
        //    {
        //        if (model.ListUserIdInAttendance == null || model.ListUserIdInAttendance.Count == 0)
        //            return Json(new {status = "failed"});
        //        var attendance = await _attendanceService.GetByIdAsync(model.Id);
        //        if (attendance == null) return Json(new {status = "failed"});

        //        attendance.Note = model.Note;
        //        await _attendanceService.UpdateAsync(attendance);


        //        //delete old user
        //        await _attendanceService.DeletaAllUserInAttendance(attendance.Id);

        //        // Get all User in addtend

        //        //var listUserIdInAttendance =
        //        //    await _userAllowInMeetingService.GetByMeetingType((MeetingType) model.TypeId);
        //        var listUserIdInAttendance =
        //             _userInMeetingService.GetAllUserInMeetingByDepartmentId(model.TypeId);

        //        //create user
        //        foreach (var user in listUserIdInAttendance)
        //        {
        //            if (model.ListUserIdInAttendance.Any(p => p == user.UserId))
        //            {
        //                await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                                     {
        //                                                                         AttendanceId = attendance.Id,
        //                                                                         UserId = user.UserId,
        //                                                                         IsAttend = true,
        //                                                                     });
        //            }
        //            else
        //                await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                                     {
        //                                                                         AttendanceId = attendance.Id,
        //                                                                         UserId = user.UserId,
        //                                                                         IsAttend = false,
        //                                                                     });
        //        }

        //        var attendanceModel = new AttendanceModel
        //                              {
        //                                  UserCreated = attendance.CreatedUserId,
        //                                  CreatedDate = attendance.CreatedDate,
        //                                  Id = attendance.Id,
        //                                  Note = attendance.Note,
        //                                  Type = attendance.Type.ToString(),
        //                                  TypeId = (int) attendance.Type,
        //                                  ListUsernameInAttendance =
        //                                      _attendanceService.GetUsernameInAttendance(attendance.Id),
        //                                  ListUserIdInAttendance =
        //                                      _attendanceService.GetUserIdInAttendance(attendance.Id)
        //                              };
        //        return Json(new {status = "success", model = attendanceModel});
        //    }
        //    else
        //    {
        //        // Create new attendance
        //        var attendance = new AttendancePerDay()
        //                         {
        //                             CreatedUserId = _workContext.CurrentUser.Id,
        //                             CreatedDate = model.CreatedDate,
        //                             Note = model.Note,
        //                             Type = (AttendanceType) model.TypeId
        //                         };
        //        var checkDate =  new DateTime();
        //        if (model.CreatedDate == checkDate)
        //            attendance.CreatedDate = DateTime.Now;
        //        await _attendanceService.InsertAsync(attendance);


        //        // Get all User in addtend

        //        //var listUserIdInAttendance =
        //        //    await _userAllowInMeetingService.GetByMeetingType((MeetingType) model.TypeId);
        //        var listUserIdInAttendance =
        //             _userInMeetingService.GetAllUserInMeetingByDepartmentId(model.TypeId);

        //        //create user
        //        foreach (var user in listUserIdInAttendance)
        //        {
        //            if (model.ListUserIdInAttendance.Any(p => p == user.UserId))
        //            {
        //                await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                                     {
        //                                                                         AttendanceId = attendance.Id,
        //                                                                         UserId = user.UserId,
        //                                                                         IsAttend = true,
        //                                                                     });
        //            }
        //            else
        //                await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                                     {
        //                                                                         AttendanceId = attendance.Id,
        //                                                                         UserId = user.UserId,
        //                                                                         IsAttend = false,
        //                                                                     });
        //        }

        //        var attendanceModel = new AttendanceModel
        //                              {
        //                                  UserCreated = attendance.CreatedUserId,
        //                                  CreatedDate = attendance.CreatedDate,
        //                                  Id = attendance.Id,
        //                                  Note = attendance.Note,
        //                                  Type = attendance.Type.ToString(),
        //                                  TypeId = (int) attendance.Type,
        //                                  ListUsernameInAttendance =
        //                                      _attendanceService.GetUsernameInAttendance(attendance.Id),
        //                                  ListUserIdInAttendance =
        //                                      _attendanceService.GetUserIdInAttendance(attendance.Id)
        //                              };

        //        return Json(new {status = "success", model = attendanceModel});
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, SearchAttendanceModel model)
        {
            var department = await _departmentService.GetByIdAsync(model.DepartmentId);
            if(department!=null)
                if (!_permissionService.Authorize(PermissionProvider.ViewAttendance))
                    return Json(null);

            var dateSearchStart = model.StartDate.HasValue? model.StartDate.Value : DateTime.Today;
            var dateSearchEnd = model.EndDate.HasValue ? model.EndDate.Value : DateTime.Today;

            var ddsMeetings = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart, dateSearchEnd, model.DepartmentId, command.Page - 1, command.PageSize);

            var data = ddsMeetings.Where(m=>m.Attendances.Count!=0).Select(m => new AttendanceModel
            {
                DateString = m.CreatedDateTime.ToShortDateString(),
                Id = m.Id,
                Department = m.Department.Name,
                UsersInAttendance = String.Join(", ", m.Attendances.Where(a=>a.IsAttendance).Select(a=>a.User.Username)),
                UsersNotInAttendance = String.Join(", ", m.Attendances.Where(a => !a.IsAttendance).Select(a => a.User.Username))
            }).ToList();

            var gridModel = new DataSourceResult
                            {
                                Data = data,
                                Total = data.Count()
                            };

            return Json(gridModel);
        }

        //[System.Web.Mvc.HttpPost]
        //public async Task<ActionResult> Create(AttendanceModel model)
        //{
        //    var attendance = new AttendancePerDay()
        //                     {

        //                         Note = model.Note,
        //                         CreatedDate = DateTime.Now,
        //                         Type = (AttendanceType) model.TypeId ,
        //                         CreatedUserId = _workContext.CurrentUser.Id

        //                     };
        //    await _attendanceService.InsertAsync(attendance);
        //    //create user
        //    foreach (var userId in model.ListUserIdInAttendance)
        //    {
        //        await _attendanceService.CreateUserInAttendanceAsync(new UserInAttendance()
        //                                                             {
        //                                                                 AttendanceId = model.Id,
        //                                                                 UserId = userId 
                                                                         
        //                                                             });

        //    }
        //    return new NullJsonResult();
        //}


        //[System.Web.Mvc.HttpPost]
        //public async Task<ActionResult> Update(AttendanceModel model)
        //{
        //    var attendance = await _attendanceService.GetByIdAsync(model.Id);
        //    attendance.Note = model.Note;
        //    attendance.Type = (AttendanceType) model.TypeId;
        //    attendance.CreatedUserId = _workContext.CurrentUser.Id;
        //    await _attendanceService.UpdateAsync(attendance);

        //    //delete old user
        //    await _attendanceService.DeletaAllUserInAttendance(attendance.Id);

        //    //create user
        //    foreach (var userName in model.ListUsernameInAttendance)
        //    {
        //        var user = await _userService.GetUserByUsernameAsync(userName);
        //        if (user != null)
        //        {
        //            var userline = new UserInAttendance()
        //                           {
        //                               AttendanceId = model.Id,
        //                               UserId = user.Id
        //                           };
        //            await _attendanceService.CreateUserInAttendanceAsync(userline);
        //        }
        //    }
        //    return new NullJsonResult();
        //}

        //[System.Web.Mvc.HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{

        //    var attendance = await _attendanceService.GetByIdAsync(id);
        //    if (attendance == null)
        //        throw new ArgumentException("No attendance found with the specified id");
        //    await _attendanceService.DeletaAllUserInAttendance(id);
        //    await _attendanceService.DeleteAsync(attendance);
        //    return new NullJsonResult();
        //}

        [HttpPost]
        public async Task<JsonResult> SendMailPacking(int attendaceId, int type)
        {
             if(attendaceId==0)
                 return Json(new { status = "error" });
             List<string> listAttachment = new List<string>();
             var fileName = await AttachmentFilePacking(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortDateString(),
                 type);
             listAttachment.Add(fileName);

            //Get attendance
            //var attendacne = await _attendanceService.GetByIdAsync(attendaceId);

            //Get User inn attendance
            //var userInAttend = await _attendanceService.GetUserInAttendance(attendaceId);

            
            //foreach (var item in userInAttend)
            //{
            //    //Get user by user Id
            //    var user = await _userService.GetUserByIdAsync(item.UserId);

            //    // Send Mail to User    
            //    var queueEmail1 = _workFlowMessageService.SendAttendToOwner(user, attendacne);
            //    var result = _sendMailService.Sendmail(queueEmail1, listAttachment);
            //}
            return Json(new { status = "success" });
        }

         [HttpGet]
         public async Task<JsonResult> SendMailSupplyChain( int attendaceId)
         {

             DateTime dateSearch = DateTime.Now;
           

             #region result


             var model = new SupplyChainModel();

             #region HSE

             var allUser = await _userService.GetAllUserRolesAsync();
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
                     Owner = owner.Name,
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
                             Owner = owner.Name,
                             UserUpdatedId = currentUser.Id,
                             CreatedDate = DateTime.Now,
                             UpdatedDate = DateTime.Now,
                             UserCreatedId = currentUser.Id,
                             MeasureCode = item.MeasureCode,


                         };
                         await _supplyChainHSE.CreateAsync(newModel);
                         item.Owner = owner.Name;
                     }
                     else
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
                                                 type = 1
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRMTD,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.Target
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.Target,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainDDSMeasure.PRLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             },
                                             new SupplyChainDDSModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName = _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("DDS",SupplyChainDDSMeasure.PRMTD
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
                     Owner = owner.Name,
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
                     Owner = owner.Name,
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
                             Owner = owner.Name,
                             UserUpdatedId = currentUser.Id,
                             CreatedDate = DateTime.Now,
                             UpdatedDate = DateTime.Now,
                             UserCreatedId = currentUser.Id,
                             MeasureCode = item.MeasureCode,
                             type = item.type

                         };
                         await _supplyChainDDS.CreateAsync(newModel);
                         item.Owner = owner.Name;
                     }
                     else
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
                                                 type = 1
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.QuanlityBOS
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.QuanlityBOS,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventPending
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventPending,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 1
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventLastDay
                                                 .ToString()).Result.MeasureSupplyChainName,
                                                 MeasureCode = (int) SupplyChainFPQMeasure.UHEventLastDay,
                                                 CreatedDate = dateSearch.ToShortDateString(),
                                                 UpdatedDate = dateSearch.ToShortDateString(),
                                                 type = 2
                                             },
                                             new SupplyChainFPQModel()
                                             {
                                                 CreatedUser = User.Identity.Name,
                                                 MeasureName =  _measureSupplyChainService.GetMeasureSupplyChainByDmsCodeAndMeasureCode("FPQ",SupplyChainFPQMeasure.UHEventMTD
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
                     Owner = owner.Name,
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
                             Owner = owner.Name,
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
                             SupplyChainFpqId = supplychainNew.Id,
                             UserId = owner.Id
                         });
                         item.ListUsernameInSupplyChainFPQ =
                             _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainNew.Id);
                     }
                     else
                     {
                         var newModel = new Entities.Domain.SupplyChainFPQ()
                         {
                             CreatedDate = DateTime.Now,
                             Owner = allsupplyChainFPQOwner.Last().Owner,
                             UpdatedDate = DateTime.Now,
                             MeasureCode = allsupplyChainFPQOwner.Last().MeasureCode,
                             type = allsupplyChainFPQOwner.Last().type,
                         };

                         var listOldOwner = _supplyChainFPQ.GetUserIdInSupplyChainFPQ(allsupplyChainFPQOwner.Last().Id);
                         await _supplyChainFPQ.CreateAsync(newModel);
                         var supplychainNew =
                             _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(item.MeasureCode.ToString(),
                                 dateSearch);
                         foreach (var itemOwner in listOldOwner)
                         {
                             await _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                             {
                                 SupplyChainFpqId = supplychainNew.Id,
                                 UserId = itemOwner
                             });
                         }

                         item.ListUsernameInSupplyChainFPQ =
                             _supplyChainFPQ.GetUserNameInSupplyChainFPQ(supplychainNew.Id);
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
                     .Result.Where(p => p.MeasureCode == (int)SupplyChainHSEMeasure.Target && p.type == 1)
                     .ToList();

             if (allsupplyChainFE.Count == 0)
             {
                 var newModel = new Entities.Domain.SupplyChainService()
                 {

                     CreatedDate = DateTime.Now,
                     Owner = owner.Name,
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
                     Owner = owner.Name,
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                                                                Remark = String.Empty
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
                     item.Remark = String.IsNullOrEmpty(supplychainResult.Remark) ? string.Empty : supplychainResult.Remark;
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
                                 Owner = owner.Name,
                                 UserUpdatedId = currentUser.Id,
                                 CreatedDate = DateTime.Now,
                                 UpdatedDate = DateTime.Now,
                                 UserCreatedId = currentUser.Id,
                                 MeasureCode = item.MeasureCode,
                                 Remark = String.IsNullOrEmpty(item.Remark) ? string.Empty : item.Remark
                             };
                             await _supplyChainProductionPlanning.CreateAsync(newModel);
                             item.Owner = owner.Name;
                         }
                         else
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
                                     Remark = String.IsNullOrEmpty(item.Remark) ? string.Empty : item.Remark
                                 };
                                 await _supplyChainProductionPlanning.CreateAsync(newModel);
                                 item.Owner = lastDayTarget.Owner;
                             }

                         }
                     }
                 }
             }

             model.SupplyChainProductionPlanning = SupplyChainProductionPlanning;


             #endregion

             #region MPSA


             //var meetingResult =
             //    _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime.Now,
             //        "0", "TotalPO");
             // var meetingResult = _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime.Now,
             // line.ToString(), "16"");
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
                     Owner = owner.Name,
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
                             Owner = owner.Name,
                             UpdatedDate = DateTime.Now,
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
                         var newModel = new Entities.Domain.SupplyChainMPSA()
                         {
                             CreatedDate = DateTime.Now,
                             Owner = allsupplyChainMPSAOwner.Last().Owner,
                             UpdatedDate = DateTime.Now,
                             MeasureCode = allsupplyChainMPSAOwner.Last().MeasureCode,
                         };

                         var listOldOwner = _supplyChainMPSA.GetUserIdInSupplyChainMPSA(allsupplyChainMPSAOwner.Last().Id);
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

                         item.ListUsernameInSupplyChainMPSA =
                             _supplyChainMPSA.GetUserNameInSupplyChainMPSA(supplychainNew.Id);
                     }
                 }
             }

             model.SupplyChainMPSA = supplyChainMPSA;


             #endregion

             #endregion

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
                 //byte[] bytes = null;
                 using (var stream = new MemoryStream())
                 {
                     ExportSupplyChainToXlsx(stream, model, filePath);
                     //bytes = stream.ToArray();

                     //Get attendance
                     //var attendacne = await _attendanceService.GetByIdAsync(attendaceId);

                     //Get User inn attendance
                     //var userInAttend = await _attendanceService.GetUserInAttendance(attendaceId);
                   
                     //foreach (var item in userInAttend)
                     //{
                     //    var listAttachment = new List<string>() { filename };
                     //    //Get user by user Id
                     //    var user = await _userService.GetUserByIdAsync(item.UserId);

                     //    // Send Mail to User    
                     //    var queueEmail1 = _workFlowMessageService.SendAttendToOwner(user, attendacne);
                     //    var result = _sendMailService.Sendmail(queueEmail1, listAttachment);
                     //}
                 }

                 return Json(new { status = "success" });

             }
             catch (Exception exc)
             {
                 return Json(new { error = exc });

             }

             #endregion


         }
                                                                            
        #region function

         public async Task<string> AttachmentFilePacking( string fromDate, string toDate, int type)
         {

           
             #region fortmart day

             DateTime dateSearchStart;

             if (String.IsNullOrEmpty(fromDate)) dateSearchStart = DateTime.Now;
             else
             {
                 var culture = CultureInfo.CreateSpecificCulture("en-US");
                 const DateTimeStyles styles = DateTimeStyles.None;
                 if (!DateTime.TryParse(fromDate, culture, styles, out dateSearchStart))
                 {
                     dateSearchStart = DateTime.Now;
                 }
             }

             DateTime dateSearchEnd;


             if (String.IsNullOrEmpty(toDate)) dateSearchEnd = DateTime.Now;
             else
             {
                 var culture = CultureInfo.CreateSpecificCulture("en-US");
                 const DateTimeStyles styles = DateTimeStyles.None;
                 if (!DateTime.TryParse(toDate, culture, styles, out dateSearchEnd))
                 {
                     dateSearchEnd = DateTime.Now;
                 }
             }

             var totalDay = (dateSearchEnd - dateSearchStart).TotalDays;

             #endregion

             #region result

             //thien comment
             //var listLine = _lineService.SearchLines(lineType: (type == 1 ? LineType.Making : LineType.Packing), includeDeedmacOperation: false);
             var listLine = await _lineService.SearchLines(departmentId: type);

             var listLineCode = listLine.ToList();
             var listModel = new List<MeetingReportModel>();


             var listMeasure = _listMeasureForMaking;
             if(type==2 )
                   listMeasure = ListMeasureForPacking;
             foreach (var measureCode in listMeasure)
             {
                 var measure = _measureService.GetMeasureByCode(measureCode, type).Result;
                 if (measure != null)
                 {
                     var model = new MeetingReportModel()
                     {
                         DmsCode = measure.Dms.DmsCode,
                         MeasureCode = measureCode,
                         MeasureName = measure.MeasureName,
                         MeasureType = measure.MeasureType.ToString(),
                         Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                         Target = measure.Target,
                         Unit = measure.Unit
                     };
                     for (var i = 0; i <= totalDay; i++)
                     {
                         foreach (var lineCode in listLineCode)
                         {

                             //var measureResult =
                             //    _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                             //        dateSearchStart.AddDays(i),
                             //        lineCode.LineCode,
                             //        measureCode);
                             var ResultModel = new LineResultReportModel()
                             {
                                 DateTimeCreate = dateSearchStart.AddDays(i),
                                 LineName = lineCode.LineName,
                                 LineCode = lineCode.LineCode
                             };
                             //if (measureResult != null)
                             //{
                             //    ResultModel.Result = measureResult.Result ?? "";
                             //}
                             //else 
                             ResultModel.Result = "";
                             model.ListResult.Add(ResultModel);
                         }
                     }
                     listModel.Add(model);
                 }
             }

             #endregion

             #region convert MeetingReportModel to MeetingReport

             var listMeetingReport = new List<MeetingReport>();

             foreach (var meetingReportModel in listModel)
             {
                 var itemMeetingReport = new MeetingReport();
                 var listItemLineResultReport = new List<LineResultReport>();
                 foreach (var lineResultReport in meetingReportModel.ListResult)
                 {
                     var itemLineResultReport = new LineResultReport();
                     itemLineResultReport.DateTimeCreate = lineResultReport.DateTimeCreate;
                     itemLineResultReport.LineCode = lineResultReport.LineCode;
                     itemLineResultReport.LineName = lineResultReport.LineName;
                     itemLineResultReport.Result = lineResultReport.Result;
                     listItemLineResultReport.Add(itemLineResultReport);
                 }
                 itemMeetingReport.MeasureType = meetingReportModel.MeasureType;
                 itemMeetingReport.DmsDecription = meetingReportModel.DmsCode;
                 itemMeetingReport.MeasureCode = meetingReportModel.MeasureCode;
                 itemMeetingReport.MeasureName = meetingReportModel.MeasureName;
                 itemMeetingReport.Owner = meetingReportModel.Owner;
                 itemMeetingReport.Target = meetingReportModel.Target;
                 itemMeetingReport.Unit = meetingReportModel.Unit;
                 itemMeetingReport.ListResult = listItemLineResultReport;
                 listMeetingReport.Add(itemMeetingReport);
             }

             #endregion

             #region save temp excel under folder of IIS

             var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

             var random = new Random();
             int us = random.Next(10000);

             var filename = "report-export-" + us + "-" + currentTime + "-" + ".xlsx";

             var folderForSaveFile = ConfigurationManager.AppSettings["excelforderpath"].ToString();
             var folderpath = AppDomain.CurrentDomain.BaseDirectory + folderForSaveFile;
             var filePath = Path.Combine(folderpath, filename);

             try
             {
                 //byte[] bytes = null;
                 using (var stream = new MemoryStream())
                 {
                     _reportService.ExportMakingToXlsx(stream, listMeetingReport, filePath);
                     
                 }
                 return filePath;
             }
             catch
             {
                 return filePath;
             }   
             #endregion
         }


         public void ExportSupplyChainToXlsx(Stream stream, SupplyChainModel model, string path)
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

                 #region create header

                 worksheetHSE.Cells["A1:G5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetHSE.Cells["A1"].Value = "DMS";
                 worksheetHSE.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["A1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["B1"].Value = "Measure";
                 worksheetHSE.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["C1"].Value = "Owner";
                 worksheetHSE.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["C1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["D1"].Value = "Making";
                 worksheetHSE.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetHSE.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["D1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["D1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["E1"].Value = "Packing";
                 worksheetHSE.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["E1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["E1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["F1"].Value = "Common Area";
                 worksheetHSE.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["F1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["F1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["G1"].Value = "Remarks";
                 worksheetHSE.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetHSE.Cells["G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetHSE.Cells["A2:A5"].Merge = true;
                 worksheetHSE.Cells["A2:A5"].Value = "HS&E";
                 worksheetHSE.Cells["A2:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["A2:A5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["A2:A5"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetHSE.Cells["A2:A5"].Style.Font.Color.SetColor(Color.White); // set color fon

                 #endregion

                 #region create content

                 // target
                 worksheetHSE.Cells["B2"].Value = "Target";
                 worksheetHSE.Cells["B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["B2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color

                 worksheetHSE.Cells["C2"].Value = "";
                 worksheetHSE.Cells["C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["C2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color

                 worksheetHSE.Cells["D2:F2"].Merge = true;
                 worksheetHSE.Cells["D2:F2"].Value = model.SupplyChainHSE[0].Making;
                 worksheetHSE.Cells["D2:F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["D2:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["D2:F2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color


                 worksheetHSE.Cells["G2"].Value = model.SupplyChainHSE[0].Remarks;
                 worksheetHSE.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetHSE.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetHSE.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color



                 // BOS-Completetion 
                 worksheetHSE.Cells["B3:B4"].Merge = true;
                 worksheetHSE.Cells["B3:B4"].Value = model.SupplyChainHSE[1].MeasureName;

                 worksheetHSE.Cells["C3:C4"].Value = model.SupplyChainHSE[1].Owner;

                 worksheetHSE.Cells["D3:F3"].Merge = true;
                 worksheetHSE.Cells["D3:F3"].Value = "BOS today: " + model.SupplyChainHSE[1].Making;

                 worksheetHSE.Cells["D4:F4"].Merge = true;
                 worksheetHSE.Cells["D4:F4"].Value = "BOS done yesterday: " + model.SupplyChainHSE[1].Packing;

                 worksheetHSE.Cells["G3:G4"].Value = model.SupplyChainHSE[1].Remarks;



                 // BOS-Top Unsafe Behaviour 

                 worksheetHSE.Cells["B5"].Value = model.SupplyChainHSE[2].MeasureName;

                 worksheetHSE.Cells["C5"].Value = model.SupplyChainHSE[2].Owner;

                 worksheetHSE.Cells["D5"].Value = model.SupplyChainHSE[2].Making;

                 worksheetHSE.Cells["E5"].Value = model.SupplyChainHSE[2].Packing;

                 worksheetHSE.Cells["F5"].Value = model.SupplyChainHSE[2].CommonArea;

                 worksheetHSE.Cells["G5"].Value = model.SupplyChainHSE[2].Remarks;

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

                 #endregion

                 #region DDS

                 var worksheetDDS = xlPackage.Workbook.Worksheets.Add("DDS");

                 #region create header
                 worksheetDDS.Cells["A1:I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetDDS.Cells["A1"].Value = "DMS";
                 worksheetDDS.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["A1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["B1"].Value = "Measure-Making";
                 worksheetDDS.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["C1"].Value = "Owner";
                 worksheetDDS.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["C1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["D1"].Value = "LPD1";
                 worksheetDDS.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["D1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["D1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["E1"].Value = "LPD2";
                 worksheetDDS.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["E1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["E1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["F1"].Value = "LPD3";
                 worksheetDDS.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["F1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["F1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["G1"].Value = "Bat";
                 worksheetDDS.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["H1"].Value = "FR (MK)";
                 worksheetDDS.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["H1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["H1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["I1"].Value = "Remark";
                 worksheetDDS.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetDDS.Cells["I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["I1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetDDS.Cells["I1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetDDS.Cells["A2:A8"].Merge = true;
                 worksheetDDS.Cells["A2:A8"].Value = "DDS";
                 worksheetDDS.Cells["A2:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Orange); // set border color
                 worksheetDDS.Cells["A2:A8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["A2:A8"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetDDS.Cells["A2:A8"].Style.Font.Color.SetColor(Color.White); // set color font


                 #endregion


                 #region create content


                 for (int i = 0; i < 3; i++)
                 {
                     int colum = i + 2;
                     worksheetDDS.Cells["B" + colum].Value = model.SupplyChainDDS[i].MeasureName;
                     worksheetDDS.Cells["C" + colum].Value = model.SupplyChainDDS[i].Owner;
                     worksheetDDS.Cells["D" + colum].Value = model.SupplyChainDDS[i].LPD1;
                     worksheetDDS.Cells["E" + colum].Value = model.SupplyChainDDS[i].LPD2;
                     worksheetDDS.Cells["F" + colum].Value = model.SupplyChainDDS[i].LPD3;
                     worksheetDDS.Cells["G" + colum].Value = model.SupplyChainDDS[i].Batch;
                     worksheetDDS.Cells["H" + colum].Value = model.SupplyChainDDS[i].FRMK;
                     worksheetDDS.Cells["I" + colum].Value = model.SupplyChainDDS[i].Remark;
                 }

                 worksheetDDS.Cells["B5"].Value = "Measure-Packing";
                 worksheetDDS.Cells["C5"].Value = "Owner";
                 worksheetDDS.Cells["D5"].Value = "Bottle";
                 worksheetDDS.Cells["E5"].Value = "Sachet";
                 worksheetDDS.Cells["F5"].Value = "Pouch";
                 worksheetDDS.Cells["G5"].Value = "FR (PK)";
                 worksheetDDS.Cells["H5"].Value = "FE";
                 worksheetDDS.Cells["I5"].Value = "Remarks";
                 worksheetDDS.Cells["B2:I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["B2:I2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                 for (int j = 3; j < 6; j++)
                 {
                     int colum = j + 3;
                     worksheetDDS.Cells["B" + colum].Value = model.SupplyChainDDS[j].MeasureName;
                     worksheetDDS.Cells["C" + colum].Value = model.SupplyChainDDS[j].Owner;
                     worksheetDDS.Cells["D" + colum].Value = model.SupplyChainDDS[j].Bottle;
                     worksheetDDS.Cells["E" + colum].Value = model.SupplyChainDDS[j].Sachet;
                     worksheetDDS.Cells["F" + colum].Value = model.SupplyChainDDS[j].Pouch;
                     worksheetDDS.Cells["G" + colum].Value = model.SupplyChainDDS[j].FRPK;
                     worksheetDDS.Cells["H" + colum].Value = model.SupplyChainDDS[j].FE;
                     worksheetDDS.Cells["I" + colum].Value = model.SupplyChainDDS[j].Remark;
                 }
                 worksheetDDS.Cells["B6:I6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetDDS.Cells["B6:I6"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                 #endregion

                 #region set width
                 worksheetDDS.Cells["A2:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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

                 #endregion

                 #region FPQ

                 var worksheetFPQ = xlPackage.Workbook.Worksheets.Add("FPQ");

                 #region create header
                 worksheetFPQ.Cells["A1:L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetFPQ.Cells["A1"].Value = "DMS";
                 worksheetFPQ.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["A1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["B1"].Value = "Measure-Making";
                 worksheetFPQ.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["C1"].Value = "Owner";
                 worksheetFPQ.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["C1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["D1"].Value = "LPD1";
                 worksheetFPQ.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["D1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["D1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["E1"].Value = "LPD2";
                 worksheetFPQ.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["E1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["E1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["F1"].Value = "LPD3";
                 worksheetFPQ.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["F1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["F1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["G1"].Value = "Batch";
                 worksheetFPQ.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["H1"].Value = "FR";
                 worksheetFPQ.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["H1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["H1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["I1"].Value = "Bottle";
                 worksheetFPQ.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["I1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["I1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["J1"].Value = "Sachet";
                 worksheetFPQ.Cells["J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["J1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["J1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["K1"].Value = "Pouch";
                 worksheetFPQ.Cells["K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["K1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["K1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["L1"].Value = "Remark";
                 worksheetFPQ.Cells["L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetFPQ.Cells["L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["L1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetFPQ.Cells["L1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetFPQ.Cells["A2:A6"].Merge = true;
                 worksheetFPQ.Cells["A2:A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                 worksheetFPQ.Cells["A2:A6"].Value = "FPQ";
                 worksheetFPQ.Cells["A2:A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Orange); // set border color
                 worksheetFPQ.Cells["A2:A6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["A2:A6"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetFPQ.Cells["A2:A6"].Style.Font.Color.SetColor(Color.White); // set color font


                 #endregion


                 #region create content

                 worksheetFPQ.Cells["B2"].Value = model.SupplyChainFPQ[0].MeasureName;
                 worksheetFPQ.Cells["D2:L2"].Merge = true;
                 worksheetFPQ.Cells["D2:L2"].Value = model.SupplyChainFPQ[0].LPD1;
                 worksheetFPQ.Cells["B2:L2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetFPQ.Cells["B2:L2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                 for (int i = 1; i < 5; i++)
                 {
                     var listUser = "";
                     listUser = String.Join(",", model.SupplyChainFPQ[i].ListUsernameInSupplyChainFPQ);
                     int colum = i + 2;
                     worksheetFPQ.Cells["B" + colum].Value = model.SupplyChainFPQ[i].MeasureName;
                     worksheetFPQ.Cells["C" + colum].Value = listUser;
                     worksheetFPQ.Cells["D" + colum].Value = model.SupplyChainFPQ[i].LPD1;
                     worksheetFPQ.Cells["E" + colum].Value = model.SupplyChainFPQ[i].LPD2;
                     worksheetFPQ.Cells["F" + colum].Value = model.SupplyChainFPQ[i].LPD3;
                     worksheetFPQ.Cells["G" + colum].Value = model.SupplyChainFPQ[i].Batch;
                     worksheetFPQ.Cells["H" + colum].Value = model.SupplyChainFPQ[i].FR;
                     worksheetFPQ.Cells["I" + colum].Value = model.SupplyChainFPQ[i].Bottle;
                     worksheetFPQ.Cells["J" + colum].Value = model.SupplyChainFPQ[i].Sachet;
                     worksheetFPQ.Cells["K" + colum].Value = model.SupplyChainFPQ[i].Pouch;
                     worksheetFPQ.Cells["L" + colum].Value = model.SupplyChainFPQ[i].Remark;
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

                 #endregion

                 #endregion

                 #region MPSA

                 var worksheetMPSA = xlPackage.Workbook.Worksheets.Add("MPSA");

                 #region create header

                 worksheetMPSA.Cells["A1:I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetMPSA.Cells["A1"].Value = "DMS";
                 worksheetMPSA.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["A1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["A2:A9"].Merge = true;
                 worksheetMPSA.Cells["A2:A9"].Value = "MPSA";
                 worksheetMPSA.Cells["A2:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["A2:A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["A2:A9"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetMPSA.Cells["A2:A9"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["B1"].Value = "Measure-Making";
                 worksheetMPSA.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["B7"].Value = "Output Measure";
                 worksheetMPSA.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["B7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["B7"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["B7"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["C1"].Value = "Owner";
                 worksheetMPSA.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["C1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["C7"].Value = "Owner";
                 worksheetMPSA.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["C7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["C7"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["C7"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["D1"].Value = "FR";
                 worksheetMPSA.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["D1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["D1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["D7"].Value = "MPSA-FR";
                 worksheetMPSA.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["D7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["D7"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["D7"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["E7:H7"].Merge = true;
                 worksheetMPSA.Cells["E7:H7"].Value = "MPSA-FE";
                 worksheetMPSA.Cells["E7:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["E7:H7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["E7:H7"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["E7:H7"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["E1"].Value = "Bottle";
                 worksheetMPSA.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["E1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["E1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["F1"].Value = "Sachet1";
                 worksheetMPSA.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["F1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["F1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["G1"].Value = "Sachet2";
                 worksheetMPSA.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["H1"].Value = "Pouch";
                 worksheetMPSA.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["H1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["H1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["I1"].Value = "Remarks";
                 worksheetMPSA.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["I1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["I1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetMPSA.Cells["I7"].Value = "Remarks";
                 worksheetMPSA.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetMPSA.Cells["I7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["I7"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetMPSA.Cells["I7"].Style.Font.Color.SetColor(Color.White); // set color font

                 #endregion


                 #region create content

                 worksheetMPSA.Cells["B2"].Value = model.SupplyChainMPSA[0].MeasureName;
                 worksheetMPSA.Cells["D2:H2"].Merge = true;
                 worksheetMPSA.Cells["D2:H2"].Value = model.SupplyChainMPSA[0].FR;
                 worksheetMPSA.Cells["B2:I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetMPSA.Cells["B2:I2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color
                 worksheetMPSA.Cells["I2"].Value = model.SupplyChainMPSA[0].Remark;

                 for (int i = 1; i < 5; i++)
                 {
                     int colum = i + 2;
                     var listUser = "";
                     listUser = String.Join(",", model.SupplyChainMPSA[i].ListUsernameInSupplyChainMPSA);
                     worksheetMPSA.Cells["B" + colum].Value = model.SupplyChainMPSA[i].MeasureName;
                     worksheetMPSA.Cells["C" + colum].Value = listUser;
                     worksheetMPSA.Cells["D" + colum].Value = model.SupplyChainMPSA[i].FR;
                     worksheetMPSA.Cells["E" + colum].Value = model.SupplyChainMPSA[i].Bottle;
                     worksheetMPSA.Cells["F" + colum].Value = model.SupplyChainMPSA[i].Sachet1;
                     worksheetMPSA.Cells["G" + colum].Value = model.SupplyChainMPSA[i].Sachet2;
                     worksheetMPSA.Cells["H" + colum].Value = model.SupplyChainMPSA[i].Pouch;
                     worksheetMPSA.Cells["I" + colum].Value = model.SupplyChainMPSA[i].Remark;

                 }
                 for (int i = 5; i < 7; i++)
                 {
                     int colum = i + 3;
                     var listUser = "";
                     listUser = String.Join(",", model.SupplyChainMPSA[i].ListUsernameInSupplyChainMPSA);
                     worksheetMPSA.Cells["B" + colum].Value = model.SupplyChainMPSA[i].MeasureName;
                     worksheetMPSA.Cells["C" + colum].Value = listUser;
                     worksheetMPSA.Cells["D" + colum].Value = model.SupplyChainMPSA[i].MPSAFR;
                     worksheetMPSA.Cells["E" + colum + ":" + "H" + colum].Merge = true;
                     worksheetMPSA.Cells["E" + colum + ":" + "H" + colum].Value = model.SupplyChainMPSA[i].MPSAFE;
                     worksheetMPSA.Cells["I" + colum].Value = model.SupplyChainMPSA[i].Remark;

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

                 #endregion

                 #region ProductionPlanning

                 var worksheetProductionPlanning = xlPackage.Workbook.Worksheets.Add("ProductionPlanning");

                 #region create header
                 worksheetProductionPlanning.Cells["A1:G14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetProductionPlanning.Cells["A1"].Value = "DMS";
                 worksheetProductionPlanning.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["A1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["A2:A14"].Merge = true;
                 worksheetProductionPlanning.Cells["A2:A14"].Value = "ProductionPlanning";
                 worksheetProductionPlanning.Cells["A2:A14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["A1:A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["A1:A9"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetProductionPlanning.Cells["A1:A9"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["B1"].Value = "MProduction Unit-Making";
                 worksheetProductionPlanning.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["B8"].Value = "Production Unit Pkg";
                 worksheetProductionPlanning.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["B8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["B8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["B8"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["C1"].Value = "Owner";
                 worksheetProductionPlanning.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["C1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["C8"].Value = "Month Target";
                 worksheetProductionPlanning.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["C8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["C8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["C8"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["D1"].Value = "Shift 2";
                 worksheetProductionPlanning.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["D1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["D1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["D8"].Value = "Today 's Plan";
                 worksheetProductionPlanning.Cells["D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["D8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["D8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["D8"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["E1"].Value = "Shift 3";
                 worksheetProductionPlanning.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["E1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["E1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["E8"].Value = "MTD";
                 worksheetProductionPlanning.Cells["E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["E8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["E8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["E8"].Style.Font.Color.SetColor(Color.White); // set color font


                 worksheetProductionPlanning.Cells["F1"].Value = "Shift 1";
                 worksheetProductionPlanning.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["F1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["F1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["F8"].Value = "Gap";
                 worksheetProductionPlanning.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["F8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["F8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["F8"].Style.Font.Color.SetColor(Color.White); // set color font


                 worksheetProductionPlanning.Cells["G1"].Value = "Remarks";
                 worksheetProductionPlanning.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetProductionPlanning.Cells["G8"].Value = "Remarks";
                 worksheetProductionPlanning.Cells["G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetProductionPlanning.Cells["G8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetProductionPlanning.Cells["G8"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetProductionPlanning.Cells["G8"].Style.Font.Color.SetColor(Color.White); // set color font

                 #endregion


                 #region create content

                 for (int i = 0; i < 6; i++)
                 {
                     int colum = i + 2;
                     worksheetProductionPlanning.Cells["B" + colum].Value = model.SupplyChainProductionPlanning[i].MeasureName;
                     worksheetProductionPlanning.Cells["C" + colum].Value = model.SupplyChainProductionPlanning[i].Owner;
                     worksheetProductionPlanning.Cells["D" + colum].Value = model.SupplyChainProductionPlanning[i].Shift2;
                     worksheetProductionPlanning.Cells["E" + colum].Value = model.SupplyChainProductionPlanning[i].Shift3;
                     worksheetProductionPlanning.Cells["F" + colum].Value = model.SupplyChainProductionPlanning[i].Shift1;
                     worksheetProductionPlanning.Cells["G" + colum].Value = model.SupplyChainProductionPlanning[i].Remark;

                 }
                 for (int i = 6; i < 12; i++)
                 {
                     int colum = i + 3;
                     worksheetProductionPlanning.Cells["B" + colum].Value = model.SupplyChainProductionPlanning[i].MeasureName;
                     worksheetProductionPlanning.Cells["C" + colum].Value = model.SupplyChainProductionPlanning[i].MonthTarget;
                     worksheetProductionPlanning.Cells["D" + colum].Value = model.SupplyChainProductionPlanning[i].TodayPlan;
                     worksheetProductionPlanning.Cells["E" + colum].Value = model.SupplyChainProductionPlanning[i].MTD;
                     worksheetProductionPlanning.Cells["F" + colum].Value = model.SupplyChainProductionPlanning[i].Gap;
                     worksheetProductionPlanning.Cells["G" + colum].Value = model.SupplyChainProductionPlanning[i].Remark;
                 }




                 #endregion

                 #region set width

                 worksheetProductionPlanning.Cells["A2:A14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                 worksheetProductionPlanning.Column(2).Width = 30;
                 worksheetProductionPlanning.Column(3).Width = 20;
                 worksheetProductionPlanning.Column(4).Width = 20;
                 worksheetProductionPlanning.Column(5).Width = 20;
                 worksheetProductionPlanning.Column(6).Width = 20;
                 worksheetProductionPlanning.Column(7).Width = 20;

                 #endregion

                 #endregion

                 #region Service

                 var worksheetService = xlPackage.Workbook.Worksheets.Add("Service");

                 #region create header
                 worksheetService.Cells["A1:L5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); // set border color
                 worksheetService.Cells["A1:A2"].Merge = true;
                 worksheetService.Cells["A1:A2"].Value = "DMS";
                 worksheetService.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["A3:A5"].Merge = true;
                 worksheetService.Cells["A3:A5"].Value = "Service";
                 worksheetService.Cells["A3:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["A3:A5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["A3:A5"].Style.Fill.BackgroundColor.SetColor(Color.Orange); // set background color
                 worksheetService.Cells["A3:A5"].Style.Font.Color.SetColor(Color.White); // set color font


                 worksheetService.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["B1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["B2"].Value = model.SupplyChainService[0].Owner;
                 worksheetService.Cells["B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["B2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["C1:G1"].Merge = true;
                 worksheetService.Cells["C1:G1"].Value = "Daily-FE";
                 worksheetService.Cells["C1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["C1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["C1:G1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["C1:G1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["H1:L1"].Merge = true;
                 worksheetService.Cells["H1:L1"].Value = "Daily-FE";
                 worksheetService.Cells["H1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["H1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["H1:L1"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["H1:L1"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["C2"].Value = "CFR(%)";
                 worksheetService.Cells["C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["C2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["D2"].Value = "SAMBC(%)";
                 worksheetService.Cells["D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["D2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["E2"].Merge = true;
                 worksheetService.Cells["E2"].Value = "Priority Line";
                 worksheetService.Cells["E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["E2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["F2"].Value = "Priority SKU";
                 worksheetService.Cells["F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["F2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["G2"].Value = "Shipment";
                 worksheetService.Cells["G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["G2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["G2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["H2"].Value = "CFR(%)";
                 worksheetService.Cells["H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["H2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["H2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["I2"].Value = "SAMBC(%)";
                 worksheetService.Cells["I2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["I2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["I2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["J2"].Merge = true;
                 worksheetService.Cells["J2"].Value = "Priority Line";
                 worksheetService.Cells["J2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["J2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["J2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["J2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["K2"].Value = "Priority SKU";
                 worksheetService.Cells["K2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["K2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["K2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["K2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["L2"].Value = "Shipment";
                 worksheetService.Cells["L2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                 worksheetService.Cells["L2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["L2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                 worksheetService.Cells["L2"].Style.Font.Color.SetColor(Color.White); // set color font

                 worksheetService.Cells["B3:L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                 worksheetService.Cells["B3:L3"].Style.Fill.BackgroundColor.SetColor(Color.Yellow); // set background color

                 #endregion


                 #region create content


                 for (int i = 0; i < 3; i++)
                 {
                     int colum = i + 3;
                     worksheetService.Cells["B" + colum].Value = model.SupplyChainService[i].MeasureName;
                     worksheetService.Cells["C" + colum].Value = model.SupplyChainService[i].CFR;
                     worksheetService.Cells["D" + colum].Value = model.SupplyChainService[i].SAMBC;
                     worksheetService.Cells["E" + colum].Value = model.SupplyChainService[i].PriorityLine;
                     worksheetService.Cells["F" + colum].Value = model.SupplyChainService[i].PrioritySKU;
                     worksheetService.Cells["G" + colum].Value = model.SupplyChainService[i].Shipment;
                 }
                 for (int i = 3; i < 6; i++)
                 {
                     int colum = i;

                     worksheetService.Cells["H" + colum].Value = model.SupplyChainService[i].CFR;
                     worksheetService.Cells["I" + colum].Value = model.SupplyChainService[i].SAMBC;
                     worksheetService.Cells["J" + colum].Value = model.SupplyChainService[i].PriorityLine;
                     worksheetService.Cells["K" + colum].Value = model.SupplyChainService[i].PrioritySKU;
                     worksheetService.Cells["L" + colum].Value = model.SupplyChainService[i].Shipment;
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

         

        #endregion
    }
}