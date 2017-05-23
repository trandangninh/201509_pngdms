using System.Data;
using Entities.Domain.QualityAlerts;
using Service.Interface;
using Service.Lines;
using Service.QualityAlerts;
using Service.Security;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utils;
using Web.Models.QualityAlert;
using Entities.Domain.Users;
using System.IO;
using Service.Common;
using Entities.Domain;
using Nois.Web.Framework.Kendoui;
using Service.Suppliers;
using Service.ClassificationDefects;
using Service.Categories;
using Nois.Web.Framework;
using Service.ScMeasures;
using Entities.Domain.ScMeasures;
using Entities.Domain.ClassificationDefects;
using Service.Frequencys;
using Entities.Domain.Frequencys;
using Service.Messages;
using Service.Tasks;

namespace Web.Controllers
{
    public class QualityAlertController : BaseController
    {
        private readonly IQualityAlertService _qualityAlertService;
        private readonly IClassificationService _classificationService;
        private readonly ILineService _lineService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IReportService _reportService;
        private readonly ISupplierService _supplierService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly IClassificationDefectService _classificationDefectService;
        private readonly IXmlService _xmlService;
        private readonly IScMeasureService _scMeasureService;
        private readonly IMqsMeasureService _mqsMeasureService;
        private readonly IFrequencyService _frequencyService;
        private readonly IWorkFlowMessageService _workFlowMessageService;
        private readonly ISendMailService _sendMailService;
        private readonly ITask _task;

        public QualityAlertController(IQualityAlertService qualityAlertService,
            IPermissionService permissionService,
            IUserService userService,
            IWorkContext workContext,
            IClassificationService classificationService,
            ILineService lineService,
            IReportService reportService,
            ISupplierService supplierService,
            IMaterialService materialService,
            ICategoryService categoryService,
            IClassificationDefectService classificationDefectService,
            IXmlService xmlService,
            IScMeasureService scMeasureService,
            IMqsMeasureService mqsMeasureService,
            IFrequencyService frequencyService,
            IWorkFlowMessageService workFlowMessageService,
            ISendMailService sendMailService,
            ITask task)
        {
            _qualityAlertService = qualityAlertService;
            _permissionService = permissionService;
            _userService = userService;
            _workContext = workContext;
            _classificationService = classificationService;
            _lineService = lineService;
            _reportService = reportService;
            _supplierService = supplierService;
            _materialService = materialService;
            _categoryService = categoryService;
            _classificationDefectService = classificationDefectService;
            _xmlService = xmlService;
            _scMeasureService = scMeasureService;
            _mqsMeasureService = mqsMeasureService;
            _frequencyService = frequencyService;
            _workFlowMessageService = workFlowMessageService;
            _sendMailService = sendMailService;
            _task = task;
        }

        //list
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();

            var dt = new DataTable();
            var result = dt.Compute("(10+5)*2", "");


            //if (await _permissionService.Authorize(PermissionProvider.ManageUser, User.Identity.Name))
            //return View();
            var qualityAlertFillterModel = new QualityAlertFillterModel
            {
                StartDate = DateTime.Today.AddDays(-1).Add(new TimeSpan(7, 0, 0)).ToString("MM/dd/yyyy HH:mm"),
                EndDate = DateTime.Today.Add(new TimeSpan(7, 0, 0)).ToString("MM/dd/yyyy HH:mm")
            };
            if (_workContext.CurrentUser != null)
                qualityAlertFillterModel.IsAdminLogin = _workContext.CurrentUser.IsAdmin();

            return View(qualityAlertFillterModel);
        }

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, QualityAlertFillterModel qualityAlertFillterModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();

            var userId = 0;
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && !_permissionService.Authorize(PermissionProvider.QAMPD))
                userId = _workContext.CurrentUser.Id;

            DateTime? dateSearchStart, dateSearchEnd, supplierReplyDateSearch;
            dateSearchStart = ParseStringToDateTimeFormat(qualityAlertFillterModel.StartDate, "MM/dd/yyyy HH:mm");
            dateSearchEnd = ParseStringToDateTimeFormat(qualityAlertFillterModel.EndDate, "MM/dd/yyyy HH:mm");
            supplierReplyDateSearch = ParseStringToDateTimeFormat(qualityAlertFillterModel.SupplierReplyDate, "MM/dd/yyyy");

            IPagedList<QualityAlertFullObject> qualityAlerts;

            qualityAlerts = await _qualityAlertService.SearchQualityAlertObjectAsync(qualityAlertFillterModel.DepartmentId,
                                                            userId,
                                                            qualityAlertFillterModel.LineId,
                                                            dateSearchStart,
                                                            dateSearchEnd,
                                                            qualityAlertFillterModel.SupplierIds,
                                                            qualityAlertFillterModel.CategoryId,
                                                            qualityAlertFillterModel.ComplaintTypeId,
                                                            qualityAlertFillterModel.ClassificationDefectId,
                                                            qualityAlertFillterModel.DefectRepeatId,
                                                            supplierReplyDateSearch,
                                                            qualityAlertFillterModel.StatusId,
                                                            qualityAlertFillterModel.FoundByFunctionId,
                                                            command.Page - 1, command.PageSize);

            DataSourceResult gridModel;
            //Get all classification
            var classifications = _classificationService.GetAllAsync().Result;
            //if (_workContext.CurrentUser != null && _workContext.CurrentUser.IsAdmin())
            //{
            gridModel = new DataSourceResult
            {
                Data = qualityAlerts.Select(x => new QualityAlertModel
                {
                    Id = x.Id,
                    AlertDateTime = x.AlertDateTime,
                    UserNameCreated = x.UserNameCreated,
                    Line = new Models.QualityAlert.Line(x.LineId, x.LineName),
                    Machine = x.Machine,
                    Detail = x.Detail,
                    Action = x.Action,
                    GCAS = x.GCAS,
                    SAPLot = x.SAPLot,
                    NumBlock = x.NumBlock,
                    Owner = new Models.QualityAlert.User(x.OwnerId, x.OwnerName),
                    FollowUpAction = x.FollowUpAction,
                    When = x.When,
                    Status = new Models.QualityAlert.QualityAlertStatus(x.QualityAlertStatusId, x.QualityAlertStatus.ToString()),
                    Classification = new Classification(x.ClassificationId, x.ClassificationName),
                    CreatedDate = x.CreatedDate,
                    SupplierLot = x.SupplierLot,
                    Supplier = new SupplierCbxModel(x.SupplierId, x.SupplierName),
                    SupplierEmail = x.SupplierEmail,
                    //Material = GetMaterialById(x.MaterialId),
                    Material = x.Material,
                    Unit = x.Unit,
                    Category = new CategoryCbxModel(x.CategoryId, x.CategoryName),
                    ComplaintType = new ComplaintTypeCbxModel(x.ComplaintTypeId, x.ComplaintTypeId != 0 ? x.ComplaintType.ToString() : string.Empty),
                    ClassificationDefect = new ClassificationDefectCbxModel(x.ClassificationDefectId, x.ClassificationDefectName),
                    DefectRepeat = new DefectRepeatCbxModel(x.DefectRepeatId, x.DefectRepeatId != 0 ? x.DefectRepeat.ToString() : string.Empty),
                    SupplierReplyDate = x.SupplierReplyDate,
                    //CostImpacted = x.CostImpacted.ToString().Replace(",", "."),
                    PRLossPercent = x.PRLossPercent.ToString().Replace(",", "."),
                    QuantityReturn = x.QuantityReturn,
                    NumStop = x.NumStop,
                    DownTime = x.DownTime,
                    DefectedQty = x.DefectedQty,
                    InformedToSupplierDate = x.InformedToSupplierDate,
                    PGerEffortLoss = x.PGerEffortLoss,
                    ContractorEffortLoss = x.ContractorEffortLoss,
                    QARelatedToMaterials = x.QARelatedToMaterials,
                    QARelatedToFG = x.QARelatedToFG,
                    FoundByFunction = new Models.QualityAlert.FoundByFunction(x.FoundByFunctionId != null ? x.FoundByFunctionId.Value : 0, x.FoundByFunctionName),
                    
                    Severity = GetSeverityByClassification(x.ClassificationId, classifications),
                    ClassificationRPN = x.ClassificationRPN

                }),
                Total = qualityAlerts.TotalCount
            };
            //}
            //else
            //{
            //    gridModel = new DataSourceResult
            //    {
            //        Data = qualityAlerts.Select(x => new QualityAlertModel
            //        {
            //            Id = x.Id,
            //            AlertDateTime = x.AlertDateTime,
            //            UserNameCreated = x.UserNameCreated,
            //            Line = new Models.QualityAlert.Line(x.LineId, x.LineName),
            //            Machine = x.Machine,
            //            Detail = x.Detail,
            //            Action = x.Action,
            //            GCAS = x.GCAS,
            //            SAPLot = x.SAPLot,
            //            NumBlock = x.NumBlock,
            //            DefectedQty = x.DefectedQty,
            //            CreatedDate = x.CreatedDate
            //        }),
            //        Total = qualityAlerts.TotalCount
            //    };
            //}

            return Json(gridModel);
        }

        private DateTime? ParseStringToDateTimeFormat(string dateStr, string format)
        {
            if (String.IsNullOrEmpty(dateStr) || String.IsNullOrEmpty(format)) return null;

            DateTime dateReturn;
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            const DateTimeStyles styles = DateTimeStyles.None;

            if (!DateTime.TryParseExact(dateStr, format, culture, styles, out dateReturn))
                return null;

            return dateReturn;
        }

        private MaterialCbxModel GetMaterialById(int materialId)
        {
            var material = _materialService.GetMaterialById(materialId);
            if (material != null)
                return new MaterialCbxModel(material.Id, material.Name);

            return new MaterialCbxModel();
        }

        private Classification GetClassification(int id)
        {
            var classification = _classificationService.GetByIdAsync(id);
            Classification classificationTemp;
            if (classification.Result != null)
            {
                classificationTemp = new Classification()
                {
                    Id = classification.Result.Id,
                    Name = classification.Result.Name
                };
                return classificationTemp;
            }
            classificationTemp = new Classification();
            return classificationTemp;
        }

        private Web.Models.QualityAlert.Line GetLine(int id)
        {
            var line = _lineService.GetByIdAsync(id);

            if (line != null)
            {
                var lineTemp = new Web.Models.QualityAlert.Line()
                {
                    Id = line.Result.Id,
                    LineName = line.Result.LineName
                };
                return lineTemp;
            }
            return null;
        }

        private Models.QualityAlert.User GetUser(int id)
        {
            var user = _userService.GetByIdAsync(id).Result;

            if (user != null)
            {
                var userTemp = new Models.QualityAlert.User()
                {
                    Id = user.Id,
                    Name = user.Username
                };
                return userTemp;
            }
            return new Models.QualityAlert.User();
        }


        [HttpPost]
        public async Task<ActionResult> Create(DataSourceRequest request, QualityAlertModel qualityAlertModel)
        {
            try
            {
                if ((!_permissionService.Authorize(PermissionProvider.QualityAlert) &&
                     (!_permissionService.Authorize(PermissionProvider.QAGuest)))
                    ||
                    (!_permissionService.Authorize(PermissionProvider.QualityAlert) &&
                     (!_permissionService.Authorize(PermissionProvider.QAMPD))))
                    return AccessDeniedView();

                //if (!ValidateQualityAlertModel(qualityAlertModel))
                //    return Json(new { status = "error" });
                //if (!ValidateLine(qualityAlertModel.Line))
                //    return Content("Line is invalid");
                //if (String.IsNullOrEmpty(qualityAlertModel.Machine))
                //    return Content("Machine is invalid");
                //ModelState.AddModelError("Line", "Line is null.");
                //if (!ValidateDatetime(qualityAlertModel.AlertDateTime))
                //    return Content("Datime is invalid");
                //if (String.IsNullOrEmpty(qualityAlertModel.GCAS) || qualityAlertModel.GCAS.Length > 8)
                //    return Content("GCAS must less than or equal 8 charaters");
                //if (String.IsNullOrEmpty(qualityAlertModel.SAPLotOrSupplierLot) || qualityAlertModel.SAPLotOrSupplierLot.Length > 10)
                //    return Content("SAP Lot/Supplier Lot must less than or equal 10 charaters");
                //if (String.IsNullOrEmpty(qualityAlertModel.SAPLot))
                //    return Content("SAP Lot is required");

                //if (String.IsNullOrEmpty(qualityAlertModel.SupplierLot))
                //    return Content("Supplier Lot is required");

                //if (qualityAlertModel.NumBlock < 0 || qualityAlertModel.NumBlock > 9999999)
                //    return Content("Block must less than or equal 7 digits");

                //if (qualityAlertModel.DefectedQty < 0 || qualityAlertModel.DefectedQty > 9999999)
                //    return Content("Defected Qty is required");
                //if (!String.IsNullOrEmpty(qualityAlertModel.FollowUpAction) && qualityAlertModel.FollowUpAction.Length > 40)
                //    return Content("Follow up action must less than 40 charaters");           

                //ModelState.AddModelError("Datetime", "Datetime is invalid.");
                if (ModelState.IsValid)
                {
                    //if (!ValidateDatetime(qualityAlertModel.AlertDateTime))
                    //    return Content("Datime is invalid");

                    //when create the status alway is open, user not allow choose status
                    qualityAlertModel.Status = new Models.QualityAlert.QualityAlertStatus(
                        (int)Entities.Domain.QualityAlerts.QualityAlertStatus.Open,
                        Entities.Domain.QualityAlerts.QualityAlertStatus.Open.ToString());

                    QualityAlert qualityAlert;

                    qualityAlert = new QualityAlert()
                    {
                        AlertDateTime = qualityAlertModel.AlertDateTime,
                        CreatedDate = GetQualityAlertCreatedDate(),
                        UserId = _workContext.CurrentUser != null ? _workContext.CurrentUser.Id : 0,
                        UserNameCreated = qualityAlertModel.UserNameCreated,
                        LineId = qualityAlertModel.Line.Id,
                        Machine = qualityAlertModel.Machine,
                        Detail = qualityAlertModel.Detail,
                        Action = qualityAlertModel.Action,
                        FollowUpAction = qualityAlertModel.FollowUpAction,
                        OwnerId = qualityAlertModel.Owner.Id,
                        When = qualityAlertModel.When,
                        QualityAlertStatusId = qualityAlertModel.Status.Id,
                        ClassificationId = qualityAlertModel.Classification.Id,
                        //SupplierLot = qualityAlertModel.SupplierLot,
                        //SupplierId = qualityAlertModel.Supplier.Id,
                        //Material = qualityAlertModel.Material,
                        Unit = qualityAlertModel.Unit,
                        CategoryId = qualityAlertModel.Category.Id,
                        ComplaintTypeId = qualityAlertModel.ComplaintType.Id,
                        ClassificationDefectId = qualityAlertModel.ClassificationDefect.Id,
                        DefectRepeatId = qualityAlertModel.DefectRepeat.Id,
                        SupplierReplyDate = qualityAlertModel.SupplierReplyDate,
                        //PRLossPercent = string.IsNullOrEmpty(qualityAlertModel.PRLossPercent) ? null : (double?)double.Parse(qualityAlertModel.PRLossPercent),
                        QuantityReturn = qualityAlertModel.QuantityReturn,
                        //NumStop = qualityAlertModel.NumStop,
                        //DownTime = qualityAlertModel.DownTime,
                        InformedToSupplierDate = qualityAlertModel.InformedToSupplierDate,
                        //PGerEffortLoss = qualityAlertModel.PGerEffortLoss,
                        //ContractorEffortLoss = qualityAlertModel.ContractorEffortLoss,
                        QARelatedToMaterials = qualityAlertModel.QARelatedToMaterials,
                        QARelatedToFG = qualityAlertModel.QARelatedToFG,                        
                        FoundByFunctionId = qualityAlertModel.FoundByFunction.Id == 0 ? null : (int?)qualityAlertModel.FoundByFunction.Id
                };

                    if (_permissionService.Authorize(PermissionProvider.QAMPD))
                    {
                        qualityAlert.GCAS = qualityAlertModel.GCAS;
                        qualityAlert.SAPLot = qualityAlertModel.SAPLot;
                        qualityAlert.NumBlock = qualityAlertModel.NumBlock;
                        qualityAlert.DefectedQty = qualityAlertModel.DefectedQty;
                    }

                    //if permission is qa guest
                    if ((_permissionService.Authorize(PermissionProvider.QAGuest) &&
                         qualityAlert.QARelatedToMaterials.HasValue && qualityAlert.QARelatedToMaterials.Value)
                        || _permissionService.Authorize(PermissionProvider.QAMPD))
                    {

                        if (qualityAlertModel.QARelatedToFG != null && qualityAlertModel.QARelatedToFG.Value)
                        {
                            qualityAlert.GCAS = qualityAlertModel.GCAS;
                            qualityAlert.SAPLot = qualityAlertModel.SAPLot;
                            qualityAlert.NumBlock = qualityAlertModel.NumBlock;
                            qualityAlert.DefectedQty = qualityAlertModel.DefectedQty;
                        }

                        qualityAlert.SupplierLot = qualityAlertModel.SupplierLot;
                        qualityAlert.SupplierId = qualityAlertModel.Supplier.Id;
                        qualityAlert.Material = qualityAlertModel.Material;
                        qualityAlert.PRLossPercent = string.IsNullOrEmpty(qualityAlertModel.PRLossPercent)
                            ? null
                            : (double?)double.Parse(qualityAlertModel.PRLossPercent);
                        qualityAlert.NumStop = qualityAlertModel.NumStop;
                        qualityAlert.DownTime = qualityAlertModel.DownTime;
                        qualityAlert.PGerEffortLoss = qualityAlertModel.PGerEffortLoss;
                        qualityAlert.ContractorEffortLoss = qualityAlertModel.ContractorEffortLoss;
                    }
                    else
                    {
                        qualityAlert.SupplierLot = null;
                        qualityAlert.SupplierId = 0;
                        qualityAlert.Material = null;
                        qualityAlert.PRLossPercent = null;
                        qualityAlert.NumStop = 0;
                        qualityAlert.DownTime = null;
                        qualityAlert.PGerEffortLoss = null;
                        qualityAlert.ContractorEffortLoss = null;
                    }

                    await _qualityAlertService.InsertAsync(qualityAlert);

                    //Update ClassificationRPN                    
                    var classifications = _classificationService.GetAllAsync().Result;
                    var severity = GetSeverityByClassification(qualityAlert.ClassificationId, classifications);
                    var classificationRPN = (severity * GetDectabilityByClassification(qualityAlert.ClassificationId, classifications) * GetMarkByClassification(qualityAlert.Id));
                    qualityAlert.ClassificationRPN = classificationRPN;
                    await _qualityAlertService.UpdateAsync(qualityAlert);

                    //goi email khi giá trị (1)*(2)*(4) > 216 hoac la Severity diem 8 tro len
                    SendEmail_WarningMark(qualityAlert.Id, qualityAlert.CreatedDate, classificationRPN, severity);

                    return Json(new { status = "success" });
                }
                //var p = ModelState.Where(x => x.Value.Errors.Count > 0);
                //return Json(new { Data = new[] { qualityAlertModel } });
                return Json(new { status = "failed" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }

        private void SendEmail_WarningMark(int id, DateTime createDate,int classificationRPN, int severity)
        {
            int markLimit = _xmlService.GetQA_MarkLimit();
            int severityLimit = _xmlService.GetQA_SeverityLimit();
            if (classificationRPN > markLimit || severity >= severityLimit)
            {
                //send email
                var emailTo = _xmlService.GetQA_EmailWarning();
                var date = createDate.ToShortDateString();
                var mMark = classificationRPN > markLimit ? classificationRPN : 0;
                var mSeverity = severity >= severityLimit ? severity : 0;
                QueuedEmail queueEmail = _workFlowMessageService.SendWarningMark(emailTo, id, date, mMark, mSeverity);
                var result = _sendMailService.Sendmail(queueEmail, null);
                _task.Execute();
            }
        }

        private DateTime parseStringToDateTime(string dateTimeStr)
        {
            if (string.IsNullOrEmpty(dateTimeStr) || dateTimeStr == "1/1/0001" || dateTimeStr == "01/01/0001")
                return new DateTime();
            //TimeZoneInfo.ConvertTime()
            var timeZoneIndex = dateTimeStr.IndexOf("(", System.StringComparison.Ordinal);
            var timeZoneId = dateTimeStr.Substring(timeZoneIndex);
            var dateTimeString = dateTimeStr.Substring(0, timeZoneIndex - 1);
            return DateTime.ParseExact(dateTimeString, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz", System.Globalization.CultureInfo.InvariantCulture);
        }

        private DateTime GetQualityAlertCreatedDate()
        {
            var createdDate = DateTime.Now;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    createdDate.AddDays(3);
                    break;
                case DayOfWeek.Saturday:
                    createdDate.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    createdDate.AddDays(1);
                    break;
            }
            return createdDate;
        }

        private bool ValidateQualityAlertModel(QualityAlertModel qualityAlertModel)
        {
            //if (ValidateLine(qualityAlertModel.Line) && ValidateDatetime(qualityAlertModel.AlertDateTime))
            //    return true;
            if (ValidateLine(qualityAlertModel.Line))
                return true;
            return false;
        }

        private bool ValidateLine(Web.Models.QualityAlert.Line line)
        {
            return line != null;
        }

        private bool ValidateDatetime(string alertDatetime)
        {
            DateTime dateTime;
            var timeZoneIndex = alertDatetime.IndexOf("(", System.StringComparison.Ordinal);
            if (timeZoneIndex < 0)
            {
                if (DateTime.TryParse(alertDatetime, out dateTime))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                var timeZoneId = alertDatetime.Substring(timeZoneIndex);
                var dateTimeString = alertDatetime.Substring(0, timeZoneIndex - 1);
                string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz";

                if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    return true;
                }
                return false;
            }

        }


        [HttpPost]
        public async Task<ActionResult> Update(QualityAlertModel qualityAlertModel)
        {
            if ((!_permissionService.Authorize(PermissionProvider.QualityAlert) && (!_permissionService.Authorize(PermissionProvider.QAGuest)))
                || (!_permissionService.Authorize(PermissionProvider.QualityAlert) && (!_permissionService.Authorize(PermissionProvider.QAMPD))))
                return AccessDeniedView();
            //if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
            //    return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var qualityAlert = await _qualityAlertService.GetByIdAsync(qualityAlertModel.Id);
                if (qualityAlert == null)
                    throw new ArgumentException("No Quality Alert found with the specified id");

                //if (!ValidateDatetime(qualityAlertModel.AlertDateTime))
                //    return Content("Datime is invalid");

                NumberFormatInfo format = new NumberFormatInfo();
                // Set the 'splitter' for thousands
                format.NumberDecimalSeparator = ".";

                //qualityAlert.AlertDateTime = DateTime.ParseExact(qualityAlertModel.AlertDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                //DateTime dateTime;
                //if(!DateTime.TryParseExact(qualityAlertModel.AlertDateTime, "ddd MMM dd yyyy HH:mm:ss 'GMT+0700' '(SE Asia Standard Time)'", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                //    DateTime.TryParseExact(qualityAlertModel.AlertDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                //qualityAlert.AlertDateTime = parseStringToDateTime(qualityAlertModel.AlertDateTime);

                qualityAlert.AlertDateTime = qualityAlertModel.AlertDateTime;
                qualityAlert.UserNameCreated = qualityAlertModel.UserNameCreated;
                qualityAlert.LineId = qualityAlertModel.Line.Id;
                qualityAlert.Machine = qualityAlertModel.Machine;
                qualityAlert.Detail = qualityAlertModel.Detail;
                qualityAlert.Action = qualityAlertModel.Action;

                //if (string.IsNullOrEmpty(qualityAlertModel.When))
                //    qualityAlert.When = new DateTime();
                //else
                //{
                //    DateTime when_temp = DateTime.ParseExact(qualityAlertModel.When, "ddd MMM dd yyyy HH:mm:ss 'GMT+0700' '(SE Asia Standard Time)'", CultureInfo.InvariantCulture);
                //    if (when_temp != null)
                //        qualityAlert.When = when_temp;
                //}
                qualityAlert.When = qualityAlertModel.When;
                if (qualityAlertModel.Classification.Id >= 0)
                    qualityAlert.ClassificationId = qualityAlertModel.Classification.Id;
                qualityAlert.OwnerId = qualityAlertModel.Owner.Id;
                qualityAlert.FollowUpAction = qualityAlertModel.FollowUpAction;

                //Change Status must check permission
                if (_permissionService.Authorize(PermissionProvider.QAEditStatus))
                    qualityAlert.QualityAlertStatusId = qualityAlertModel.Status.Id;

                //qualityAlert.SupplierLot = qualityAlertModel.SupplierLot;
                //qualityAlert.SupplierId = qualityAlertModel.Supplier.Id;
                //qualityAlert.Material = qualityAlertModel.Material;
                qualityAlert.Unit = qualityAlertModel.Unit;
                qualityAlert.CategoryId = qualityAlertModel.Category.Id;
                qualityAlert.ComplaintTypeId = qualityAlertModel.ComplaintType.Id;
                qualityAlert.ClassificationDefectId = qualityAlertModel.ClassificationDefect.Id;
                qualityAlert.DefectRepeatId = qualityAlertModel.DefectRepeat.Id;
                qualityAlert.SupplierReplyDate = qualityAlertModel.SupplierReplyDate;
                //qualityAlert.PRLossPercent = string.IsNullOrEmpty(qualityAlertModel.PRLossPercent) ? null : (double?)double.Parse(qualityAlertModel.PRLossPercent);
                qualityAlert.QuantityReturn = qualityAlertModel.QuantityReturn;
                //qualityAlert.NumStop = qualityAlertModel.NumStop;
                //qualityAlert.DownTime = qualityAlertModel.DownTime;
                qualityAlert.InformedToSupplierDate = qualityAlertModel.InformedToSupplierDate;
                //qualityAlert.PGerEffortLoss = qualityAlertModel.PGerEffortLoss;
                //qualityAlert.ContractorEffortLoss = qualityAlertModel.ContractorEffortLoss;
                qualityAlert.QARelatedToMaterials = qualityAlertModel.QARelatedToMaterials;
                qualityAlert.QARelatedToFG = qualityAlertModel.QARelatedToFG;
                qualityAlert.FoundByFunctionId = qualityAlertModel.FoundByFunction.Id == 0 ? null : (int?)qualityAlertModel.FoundByFunction.Id;


                if (_permissionService.Authorize(PermissionProvider.QAMPD))
                {
                    qualityAlert.GCAS = qualityAlertModel.GCAS;
                    qualityAlert.SAPLot = qualityAlertModel.SAPLot;
                    qualityAlert.NumBlock = qualityAlertModel.NumBlock;
                    qualityAlert.DefectedQty = qualityAlertModel.DefectedQty;
                }

                //if permission is qa guest
                if ((_permissionService.Authorize(PermissionProvider.QAGuest) && qualityAlert.QARelatedToMaterials.HasValue && qualityAlert.QARelatedToMaterials.Value)
                    || _permissionService.Authorize(PermissionProvider.QAMPD))
                {

                    if (qualityAlertModel.QARelatedToFG != null && qualityAlertModel.QARelatedToFG.Value)
                    {
                        qualityAlert.GCAS = qualityAlertModel.GCAS;
                        qualityAlert.SAPLot = qualityAlertModel.SAPLot;
                        qualityAlert.NumBlock = qualityAlertModel.NumBlock;
                        qualityAlert.DefectedQty = qualityAlertModel.DefectedQty;
                    }

                    qualityAlert.SupplierLot = qualityAlertModel.SupplierLot;
                    qualityAlert.SupplierId = qualityAlertModel.Supplier.Id;
                    qualityAlert.Material = qualityAlertModel.Material;
                    qualityAlert.PRLossPercent = string.IsNullOrEmpty(qualityAlertModel.PRLossPercent) ? null : (double?)double.Parse(qualityAlertModel.PRLossPercent);
                    qualityAlert.NumStop = qualityAlertModel.NumStop;
                    qualityAlert.DownTime = qualityAlertModel.DownTime;
                    qualityAlert.PGerEffortLoss = qualityAlertModel.PGerEffortLoss;
                    qualityAlert.ContractorEffortLoss = qualityAlertModel.ContractorEffortLoss;
                }

                //Update ClassificationRPN                
                var classifications = _classificationService.GetAllAsync().Result;
                var severity = GetSeverityByClassification(qualityAlert.ClassificationId, classifications);
                var classificationRPN = (severity * GetDectabilityByClassification(qualityAlert.ClassificationId, classifications) * GetMarkByClassification(qualityAlert.Id));
                qualityAlert.ClassificationRPN = classificationRPN;              

                await _qualityAlertService.UpdateAsync(qualityAlert);

                //goi email khi giá trị (1)*(2)*(4) > 216 hoac la Severity diem 8 tro len
                SendEmail_WarningMark(qualityAlert.Id, qualityAlert.CreatedDate, classificationRPN, severity);

                return Json(new
                {
                    status = "success",
                });
            }
            return Json(new
            {
                status = "failed",
            });
        }

        //[HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var qualityAlert = await _qualityAlertService.GetQualityAlertByIdAsync(id);
        //    if (qualityAlert == null)
        //        throw new ArgumentException("No Quality Alert found with the specified id");
        //    await _qualityAlertService.DeleteQualityAlertAsync(qualityAlert);
        //    return Json(new
        //    {
        //        status = "success",
        //    });
        //}

        //Remove delete QA function
        //[HttpPost]
        //public async Task<ActionResult> DeleteQualityAlerts(List<int> listId)
        //{
        //    if ((!_permissionService.Authorize(PermissionProvider.QualityAlert) && (!_permissionService.Authorize(PermissionProvider.QAGuest)))
        //        || (!_permissionService.Authorize(PermissionProvider.QualityAlert) && (!_permissionService.Authorize(PermissionProvider.QAMPD))))
        //        return AccessDeniedView();
        //    try
        //    {
        //        await _qualityAlertService.DeleteQualityAlertAsync(listId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "false", Content = ex.Message });
        //    }

        //    return Json(new
        //    {
        //        status = "success",
        //    });
        //}


        public async Task<ActionResult> ExportToExcel(int departmentId,
                                                        int lineId,
                                                        int categoryId,
                                                        int complaintTypeId,
                                                        int classificationDefectId,
                                                        int defectRepeatId,
                                                        int statusId,
                                                        string supplierIds = null,
                                                        string supplierReplyDate = null,
                                                        string fromDate = null,
                                                        string toDate = null,
                                                        string listId = null)
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();

            List<QualityAlertFullObject> listQualityAlert;

            listQualityAlert = await GetDataToExport(departmentId, lineId, categoryId, complaintTypeId, classificationDefectId, defectRepeatId, statusId, supplierIds, supplierReplyDate, fromDate, toDate, listId);

            #region result


            #endregion


            #region export excell

            var path = "";
            try
            {
                var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

                var r = new Random();
                int u = r.Next(10000);

                var filename = "report-export-" + u + "-" + currentTime + "-" + ".xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _reportService.ExportQualityAlertToXlsx(stream, listQualityAlert, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("List");
            }

            #endregion
        }

        private async Task<List<QualityAlertFullObject>> GetDataToExport(int departmentId, int lineId, int categoryId, int complaintTypeId, int classificationDefectId, int defectRepeatId, int statusId, string supplierIds = null,
                                                                            string supplierReplyDate = null, string fromDate = null, string toDate = null, string listId = null)
        {
            var listQualityAlert = new List<QualityAlertFullObject>();

            DateTime? dateSearchStart, dateSearchEnd, supplierReplyDateSearch;
            dateSearchStart = ParseStringToDateTimeFormat(fromDate, "MM/dd/yyyy HH:mm");
            dateSearchEnd = ParseStringToDateTimeFormat(toDate, "MM/dd/yyyy HH:mm");
            supplierReplyDateSearch = ParseStringToDateTimeFormat(supplierReplyDate, "MM/dd/yyyy");

            var userId = 0;
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && !_permissionService.Authorize(PermissionProvider.QAMPD))
                userId = _workContext.CurrentUser.Id;

            List<int> listSupplierId = null;
            if (!String.IsNullOrEmpty(supplierIds))
                listSupplierId = supplierIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

            var qualityAlerts = await _qualityAlertService.SearchQualityAlertObjectAsync(departmentId, userId, lineId, dateSearchStart, dateSearchEnd, listSupplierId, categoryId, complaintTypeId, classificationDefectId,
                                                                                            defectRepeatId, supplierReplyDateSearch, statusId, 0, int.MaxValue);

            if (!String.IsNullOrEmpty(listId))
            {
                var ids = listId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

                listQualityAlert = qualityAlerts.Where(q => ids.Any(i => i == q.Id)).ToList();
            }
            else
                listQualityAlert = qualityAlerts.ToList();

            //var listMaterial = _materialService.GetAllMaterials();
            //for (int i = 0; i < listQualityAlert.Count; i++)
            //{
            //    var material = listMaterial.FirstOrDefault(m => m.Id == listQualityAlert[i].MaterialId);
            //    if (material != null)
            //        listQualityAlert[i].MaterialName = material.Name;
            //}

            return listQualityAlert;
        }
        [FileDownload]
        public async Task<ActionResult> ExportComplaintLetterToExcel(int departmentId, int lineId, int categoryId, int complaintTypeId, int classificationDefectId, int defectRepeatId, int statusId, string supplierIds,
                                                        string supplierReplyDate = null, string fromDate = null, string toDate = null, string listId = null)
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();
            List<QualityAlertFullObject> listQualityAlert;

            listQualityAlert = await GetDataToExport(departmentId, lineId, categoryId, complaintTypeId, classificationDefectId, defectRepeatId, statusId, supplierIds, supplierReplyDate, fromDate, toDate, listId);

            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelData/Complaint letter.xlsx");
                byte[] file = System.IO.File.ReadAllBytes(path);

                var filename = "Complaint letter.xlsx";
                var qa = listQualityAlert.FirstOrDefault();
                if (qa != null) ;
                filename = String.Format("{0}-{1}-{2}-{3}.xlsx", qa.SupplierName, qa.InformedToSupplierDate.Month, qa.ComplaintType == ComplaintType.Complaint ? "C" : "FB", qa.Id);

                byte[] bytes = null;
                using (var streamSource = new MemoryStream(file))
                {
                    var streamDestination = new MemoryStream();
                    //streamSource.CopyTo(streamDestination);
                    _reportService.ExportComplaintLetterToXlsx(streamSource, streamDestination, listQualityAlert, "");
                    bytes = streamDestination.ToArray();
                }

                //jquery.fileDownload uses this cookie to determine that a file download has completed successfully
                //Response.SetCookie(new HttpCookie(filename, "true") { Path = "/" });
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("Index");
            }
        }

        /*
        public async Task<ActionResult> ExportToExcel(string fromDate, string toDate, int? lineId = null)
        {
            #region fortmart day again

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


            var qualityAlerts = await _qualityAlertService.SearchQualityAlertAsync(lineId, dateSearchStart, dateSearchEnd, 0, int.MaxValue);


            var Data = qualityAlerts.Select(x => new QualityAlertReport
            {
                AlertDateTime = x.AlertDateTime.ToLongTimeString(),
                //UserName = GetUser(x.UserId).Name,
                UserName = x.UserNameCreated,
                LineName = GetLine(x.LineId).LineName,
                Detail = x.Detail,
                Action = x.Action,
                GCAS = x.GCAS,
                SAPLotOrSupplierLot = x.SAPLotOrSupplierLot,
                NumBlock = x.NumBlock,
                Owner = GetUser(x.OwnerId).Name,
                FollowUpAction = x.FollowUpAction,
                When = x.When == DateTime.Parse("01/01/0001") ? "" : x.When.ToShortDateString(),
                Status = x.QualityAlertStatus.ToString(),
                Classification = GetClassification(x.ClassificationId).Name
            });

            //List<QualityAlertReport> listQualityAlertReport = new List<QualityAlertReport>();
            //listQualityAlertReport = Data.ToList();
            //foreach (var item in Data)
            //    listQualityAlertReport.Add(item);

            #endregion


            #region export excell

            var path = "";
            try
            {
                var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

                var r = new Random();
                int u = r.Next(10000);

                var filename = "report-export-" + u + "-" + currentTime + "-" + ".xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _reportService.ExportQualityAlertToXlsx(stream, Data.ToList(), path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception)
            {

                return RedirectToAction("List");
            }

            #endregion
        }

        */

        /*
        public async Task<ActionResult> ExportSelectedToExcel(string listId)
        {

            #region result
            var ids = listId.Split(new []{','},StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            List<QualityAlert> listQualityAlert = new List<QualityAlert>();
            for (int i = 0; i < ids.Count; i++)
            {
                var qualityAlerts = await _qualityAlertService.GetByIdAsync(ids[i]);
                listQualityAlert.Add(qualityAlerts);
            }

            var Data = listQualityAlert.Select(x => new QualityAlertReport
            {
                AlertDateTime = x.AlertDateTime.ToString(),
                //UserName = GetUser(x.UserId).Name,
                UserName = x.UserNameCreated,
                LineName = GetLine(x.LineId).LineName,
                Detail = x.Detail,
                Action = x.Action,
                GCAS = x.GCAS,
                SAPLotOrSupplierLot = x.SAPLotOrSupplierLot,
                NumBlock = x.NumBlock,
                Owner = GetUser(x.OwnerId).Name,
                FollowUpAction = x.FollowUpAction,
                When = x.When == DateTime.Parse("01/01/0001") ? "" : x.When.ToShortDateString(),
                Status = x.QualityAlertStatus.ToString(),
                Classification = GetClassification(x.ClassificationId).Name
            });


            #endregion


            #region export excell

            var path = "";
            try
            {
                var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

                var r = new Random();
                int u = r.Next(10000);

                var filename = "report-export-" + u + "-" + currentTime + "-" + ".xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _reportService.ExportQualityAlertToXlsx(stream, Data.ToList(), path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception)
            {

                return RedirectToAction("List");
            }

            #endregion
        }
        */

        [HttpPost]
        public JsonResult GetAllUser(DataSourceRequest command)
        {
            var users = _userService.GetAllUsersAsync();
            //var data = users.Where(u => (u.Active == true) && (u.DepartmentId == departmentId)).Select(x => new Web.Models.Meeting.UserForMeetingViewModel
            var data = users.Where(u => (u.Active == true)).Select(x => new Web.Models.Meeting.UserForMeetingViewModel
            {
                Id = x.Id,
                Name = x.Username
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all qualityalert status
        [HttpPost]
        public JsonResult GetAllQualityAlertStatus(DataSourceRequest command)
        {
            var allStatus = new List<Entities.Domain.QualityAlerts.QualityAlertStatus>()
                {
                                         Entities.Domain.QualityAlerts.QualityAlertStatus.Open,
                                         Entities.Domain.QualityAlerts.QualityAlertStatus.Delayed,
                                         Entities.Domain.QualityAlerts.QualityAlertStatus.Closed
                };

            var data = allStatus.Select(s => new Models.QualityAlert.QualityAlertStatus
            {
                Id = (int)s,
                Name = s.ToString()
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all materials
        [HttpPost]
        public JsonResult GetAllMaterial(DataSourceRequest command)
        {
            var materials = _materialService.GetAllMaterials();
            var data = materials.Select(s => new SupplierCbxModel()
            {
                Id = s.Id,
                Name = s.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all suppliers
        [HttpPost]
        public JsonResult GetAllSupplier(DataSourceRequest command)
        {
            var suppliers = _supplierService.GetAllAsync().Result;
            var data = suppliers.Select(s => new SupplierCbxModel()
            {
                Id = s.Id,
                Name = s.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all suppliers with email
        [HttpPost]
        public JsonResult GetAllSupplierWithEmail(DataSourceRequest command)
        {
            var suppliers = _supplierService.GetAllAsync().Result;
            var data = suppliers.Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.VendorContact
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all classificationDefects
        [HttpPost]
        public JsonResult GetAllClassificationDefect(DataSourceRequest command)
        {
            var classificationDefects = _classificationDefectService.GetAllAsync().Result;
            var data = classificationDefects.Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                SupplierIds = s.Suppliers.Count > 0 ? s.Suppliers.Select(x => x.Id) : new List<int>()
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all categorys
        [HttpPost]
        public JsonResult GetAllCategory(DataSourceRequest command)
        {
            var categorys = _categoryService.GetAllAsync().Result;
            var data = categorys.Select(s => new CategoryCbxModel()
            {
                Id = s.Id,
                Name = s.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all Complaint Type
        [HttpPost]
        public JsonResult GetAllComplaintType(DataSourceRequest command)
        {
            var allComplaintType = new List<ComplaintType>()
                {
                    ComplaintType.Complaint,
                    ComplaintType.Feedback
                };

            var data = allComplaintType.Select(s => new Models.QualityAlert.ComplaintTypeCbxModel
            {
                Id = (int)s,
                Name = s.ToString()
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //return all Defect Repeat
        [HttpPost]
        public JsonResult GetAllDefectRepeat(DataSourceRequest command)
        {
            var allDefectRepeat = new List<DefectRepeat>()
                {
                    DefectRepeat.Yes,
                    DefectRepeat.No
                };

            var data = allDefectRepeat.Select(s => new Models.QualityAlert.DefectRepeatCbxModel
            {
                Id = (int)s,
                Name = s.ToString()
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ScoreCard()
        {
            return View();
        }

        public async Task<ActionResult> Report(string month)
        {
            if (!_permissionService.Authorize(PermissionProvider.QAReport))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(month) || month.Contains("-1"))
                month = "-1";

            var model = new QualityAlertReportModel();
            var months = new List<int>();
            if (month == "-1")
                months = Enumerable.Range(0, 12).ToList();//Quarter
            else
                months = month.Split(',').Select(x => int.Parse(x)).OrderBy(x => x).ToList();//for months

            var startDate = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var endDate = new DateTime(DateTime.Now.Year, 6, 1);

            var qAs = await _qualityAlertService.SearchQualityAlertObjectAsync(startDate: startDate, endDate: endDate);

            var totalDelivery = await _scMeasureService.GetScMeasureByCodeAsync("Total_Of_Delivery");
            var mqss = await _mqsMeasureService.GetMqsMeasures(startDate, endDate, 0, totalDelivery.Id);

            var suppliers = await _supplierService.GetAllAsync();
            model.SuplierList = String.Join(",", suppliers.Select(s => "\"" + s.Name + "\""));

            var colors = _xmlService.GetAllNumberComplaintTrackingColor();
            var monthGroups = new List<string>();
            if (month == "-1")
                monthGroups = new List<string> { "JAS", "OND", "JFM", "AMJ" };
            else
                monthGroups = months.Select(moo => startDate.AddMonths(moo).ToString("MMM")).ToList();
            var c = 0;
            var m = 0;
            foreach (var monthGroup in monthGroups)
            {
                var ms = startDate.AddMonths(m);
                DateTime me;
                if (month == "-1")
                    me = startDate.AddMonths(m + 3);
                else
                    me = startDate.AddMonths(m + 1);

                var query = qAs.Where(qa => qa.CreatedDate >= ms && qa.CreatedDate < me && qa.ComplaintTypeId != 0);
                var data = new List<float>();
                foreach (var supplier in suppliers)
                {
                    var total = 0;
                    var mqsMeasures = mqss.Where(mqs => mqs.SupplierId == supplier.Id && mqs.Date >= startDate && mqs.Date < endDate);
                    if (mqsMeasures != null)
                    {
                        total = mqsMeasures.Sum(meas => { var t = 0; int.TryParse(meas.Value, out t); return t; });
                    }
                    if (total == 0 || query == null)
                        data.Add(0);
                    else
                        data.Add((float)query.Count(d => d.SupplierId == supplier.Id) / (float)total * 100);
                }
                model.Items.Add(new ChartItemModel
                {
                    Name = monthGroup,
                    Data = String.Join(",", data.Select(d => d.ToString("0.00"))),
                    Color = colors[c % colors.Count()]
                });
                c++;
                m += 3;
            }

            //YTD
            var ytdQuery = qAs.Where(qa => qa.ComplaintTypeId != 0);
            var ytdData = new List<float>();
            foreach (var supplier in suppliers)
            {
                var total = 0;
                var mqsMeasures = mqss.Where(mqs => mqs.SupplierId == supplier.Id);
                if (mqsMeasures != null)
                {
                    total = mqsMeasures.Sum(meas => { var t = 0; int.TryParse(meas.Value, out t); return t; });
                }
                if (total == 0 || ytdQuery == null)
                    ytdData.Add(0);
                else
                    ytdData.Add((float)ytdQuery.Count(d => d.SupplierId == supplier.Id) / (float)total * 100);
            }
            model.Items.Insert(0, new ChartItemModel
            {
                Name = "FY" + startDate.ToString("yy") + endDate.ToString("yy"),
                Data = String.Join(",", ytdData.Select(d => d.ToString("0.00"))),
                Color = colors[c % colors.Count()]
            });

            model.MonthList = String.Join(",", month.Split(','));
            return View(model);
        }
        private string Process(int m, int? numOfNextMonth, DateTime startDate, IPagedList<QualityAlertFullObject> qAs, int total, int supplierId)
        {
            var ms = startDate.AddMonths(m);
            DateTime me;
            if (numOfNextMonth.HasValue)
                me = startDate.AddMonths(m + numOfNextMonth.Value);
            else
                me = startDate.AddMonths(m + 1);

            var query = qAs.Where(qa => qa.CreatedDate >= ms && qa.CreatedDate < me && qa.ComplaintTypeId != 0);

            //var total = 0;
            //var mqsMeasures = mqss;
            //if (mqsMeasures != null)
            //{
            //    total = mqsMeasures.Sum(meas => { var t = 0; int.TryParse(meas.Value, out t); return t; });
            //}
            if (total == 0 || query == null)
                return "0%";
            else
            {
                var result = ((float)query.Count(d => d.SupplierId == supplierId) / (float)total * 100);
                return result == 0 ? "0%" : result.ToString("0.00") + "%";
            }

        }
        [HttpPost]
        public async Task<ActionResult> ReportTable(DataSourceRequest command, string month)
        {
            if (string.IsNullOrEmpty(month) || month.Contains("-1"))
                month = "-1";

            var model = new List<QualityAlertReportTableModel>();

            var months = new List<int>();
            if (month == "-1")
                months = Enumerable.Range(0, 12).ToList();//Quarter
            else
                months = month.Split(',').Select(x => int.Parse(x)).OrderBy(x => x).ToList();//for months

            var startDate = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var endDate = new DateTime(DateTime.Now.Year, 6, 1);

            //Get all quality alert in year
            var qAs = await _qualityAlertService.SearchQualityAlertObjectAsync(startDate: startDate, endDate: endDate);

            var totalDelivery = await _scMeasureService.GetScMeasureByCodeAsync("Total_Of_Delivery");
            //Get all total delivery in year
            var mqss = await _mqsMeasureService.GetMqsMeasures(startDate, endDate, 0, totalDelivery.Id);
            //Get all supliers
            var suppliers = await _supplierService.GetAllAsync();

            foreach (var supplier in suppliers)
            {
                var item = new QualityAlertReportTableModel();
                item.SupplierName = supplier.Name;
                //YF
                var ytdQuery = qAs.Where(qa => qa.ComplaintTypeId != 0);
                var total = 0;
                var mqsMeasures = mqss.Where(mqs => mqs.SupplierId == supplier.Id);
                if (mqsMeasures != null)
                {
                    total = mqsMeasures.Sum(meas => { var t = 0; int.TryParse(meas.Value, out t); return t; });
                }
                if (total == 0 || ytdQuery == null)
                    item.FY = "0%";
                else
                    item.FY = ((float)ytdQuery.Count(d => d.SupplierId == supplier.Id) / (float)total * 100).ToString("0.00") + "%";

                item.JAS = Process(0, 3, startDate, qAs, total, supplier.Id);
                item.OND = Process(3, 3, startDate, qAs, total, supplier.Id);
                item.JFM = Process(6, 3, startDate, qAs, total, supplier.Id);
                item.AMJ = Process(9, 3, startDate, qAs, total, supplier.Id);

                item.JUL = Process(0, null, startDate, qAs, total, supplier.Id);
                item.AUG = Process(1, null, startDate, qAs, total, supplier.Id);
                item.SEP = Process(2, null, startDate, qAs, total, supplier.Id);
                item.OCT = Process(3, null, startDate, qAs, total, supplier.Id);
                item.NOV = Process(4, null, startDate, qAs, total, supplier.Id);
                item.DEC = Process(5, null, startDate, qAs, total, supplier.Id);
                item.JAN = Process(6, null, startDate, qAs, total, supplier.Id);
                item.FEB = Process(7, null, startDate, qAs, total, supplier.Id);
                item.MAR = Process(8, null, startDate, qAs, total, supplier.Id);
                item.APR = Process(9, null, startDate, qAs, total, supplier.Id);
                item.MAY = Process(10, null, startDate, qAs, total, supplier.Id);
                item.JUN = Process(11, null, startDate, qAs, total, supplier.Id);

                model.Add(item);
            }

            DataSourceResult gridModel;

            gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count()
            };
            return Json(gridModel);
        }
        public async Task<ActionResult> ComplaintTracking(int supplierId)
        {
            var supplier = await _supplierService.GetByIdAsync(supplierId);

            var model = new ComplaintTrackingChartModel();

            if (supplier == null)
                return View(model);


            var startDate = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var endDate = new DateTime(DateTime.Now.Year, 6, 1);
            //startDate = startDate.AddDays(-1);
            //endDate = endDate.AddDays(1);

            var qAs = await _qualityAlertService.SearchQualityAlertObjectAsync(supplierIds: new List<int> { supplierId }, startDate: startDate, endDate: endDate);
            var cds = await _classificationDefectService.GetAllClassificationDefectAsync();

            var classDefects = new List<ClassificationDefect>();
            foreach (var cd in cds)
            {
                if (qAs.Count(qa => qa.ClassificationDefectId == cd.Id) > 0)
                    classDefects.Add(cd);
            }

            model.ClassificationDefectNameList = String.Join(",", classDefects.Select(cd => "\"" + cd.Name + "\""));
            model.SuplierName = supplier.Name;

            var mData = qAs.OrderBy(x => x.CreatedDate)
                .GroupBy(qa => qa.CreatedDate.Month);

            var date = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var colors = _xmlService.GetAllComplaintTrackingColor();
            for (var i = 0; i < 12; i++)
            {
                var monthData = mData.FirstOrDefault(d => d.Key == date.Month);

                model.Items.Add(new ChartItemModel
                {
                    Name = date.ToString("MMM"),
                    Data = monthData != null ? String.Join(",", classDefects.Select(cd => monthData.Count(d => d.ClassificationDefectId == cd.Id))) : "",
                    Color = colors[i % colors.Count()]
                });
                date = date.AddMonths(1);
            }


            return View(model);
        }

        public async Task<ActionResult> NumberComplaintTracking(int supplierId)
        {
            var supplier = await _supplierService.GetByIdAsync(supplierId);

            var model = new NumberComplaintTrackingChartModel();

            if (supplier == null)
                return View(model);

            var months = Enumerable.Range(0, 12);

            var startDate = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var endDate = new DateTime(DateTime.Now.Year, 6, 1);
            model.MonthList = "\"YTD\"," + String.Join(",", months.Select(m => "\"" + startDate.AddMonths(m).ToString("MMM") + "\""));
            model.SuplierName = supplier.Name;
            //startDate = startDate.AddDays(-1);
            //endDate = endDate.AddDays(1);

            var qAs = await _qualityAlertService.SearchQualityAlertObjectAsync(supplierIds: new List<int> { supplierId }, startDate: startDate, endDate: endDate);

            var mData = qAs.OrderBy(x => x.CreatedDate)
                .GroupBy(qa => qa.ClassificationDefectId);

            var date = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var colors = _xmlService.GetAllNumberComplaintTrackingColor();

            var cds = await _classificationDefectService.GetAllClassificationDefectAsync();
            var classDefects = new List<ClassificationDefect>();
            foreach (var cd in cds)
            {
                if (qAs.Count(qa => qa.ClassificationDefectId == cd.Id) > 0)
                    classDefects.Add(cd);
            }

            var i = 0;
            foreach (var classDefect in classDefects)
            {
                var classDefectData = mData.FirstOrDefault(d => d.Key == classDefect.Id);

                var ytd = classDefectData != null ? classDefectData.Count() : 0;

                model.Items.Add(new ChartItemModel
                {
                    Name = classDefect.Name,
                    Data = ytd + (classDefectData != null ? ("," + String.Join(",", months.Select(cd => classDefectData.Count(d => d.CreatedDate.Month == cd)))) : ""),
                    Color = colors[i % colors.Count()]
                });
                i++;
            }
            return View(model);
        }

        public async Task<ActionResult> DefectChart(string month)
        {
            var model = new List<DefectChartItemModel>();

            if (string.IsNullOrEmpty(month) || month.Contains("-1"))
                month = "-1";

            var months = new List<int>();
            if (month == "-1")
                months = Enumerable.Range(0, 12).ToList();//Quarter
            else
                months = month.Split(',').Select(x => int.Parse(x)).OrderBy(x => x).ToList();//for months

            var startDate = new DateTime(DateTime.Now.Year - 1, 7, 1);
            var endDate = new DateTime(DateTime.Now.Year, 6, 1);

            var qAs = await _qualityAlertService.SearchQualityAlertObjectAsync(startDate: startDate, endDate: endDate);

            var query = qAs.Where(qa => months.Any(m => (m + 7) % 13 == qa.CreatedDate.Month));

            var data = query.GroupBy(qa => qa.SupplierName);
            var colors = _xmlService.GetAllNumberComplaintTrackingColor();
            var i = 0;
            //var total = qAs.Sum(q => q.DefectedQty);
            foreach (var qa in data)
            {
                var defectTotal = qa.Sum(q => q.DefectedQty);
                if (defectTotal > 0)
                {
                    model.Add(new DefectChartItemModel()
                    {
                        SupplierName = qa.Key,
                        Value = defectTotal.ToString(),
                        Color = colors[i % colors.Count()]
                    });
                }
                i++;
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CountQualityAlertByClassification(int id = 0)
        {
            try
            {
                var qualityAlert = _qualityAlertService.QualityAlertByClassificationId(id,null);

                return Json(new { status = "success", qualityAlertCount = qualityAlert.Count });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }            
        }

        #region Private function

        private int GetSeverityByClassification(int classificationId, IPagedList<Entities.Domain.Classifications.Classification> classifications)
        {
            var classification = classifications.FirstOrDefault(y => y.Id == classificationId);
            //Get classification            
            return classificationId == 0 ? 0 : (classification == null ? 0 : classification.Severity);
        }

        private int GetDectabilityByClassification(int classificationId, IPagedList<Entities.Domain.Classifications.Classification> classifications)
        {
            var classification = classifications.FirstOrDefault(y => y.Id == classificationId);
            return classificationId == 0 ? 0 : (classification == null ? 0 : classification.Dectability);
        }

        private int GetMarkByClassification(int idQA)
        {
            var currentDate = DateTime.Now;
            var lastYear = DateTime.Now.AddYears(-1);
            var threeLastMonth = DateTime.Now.AddMonths(-3);
            var mark = 0;            
            var qA = _qualityAlertService.GetByIdAsync(idQA).Result;

            if (qA != null)
            {
                if (qA.ClassificationId > 0 && qA.CreatedDate >= lastYear)//Only handle QA from current to a year ago
                {
                    var quanlityAlert = _qualityAlertService.QualityAlertByClassificationId(qA.ClassificationId, lastYear);
                    if (quanlityAlert.Count == 1)//1
                    {                        
                        return _frequencyService.GetMarkFrequencyByCode(FrequencyEnum.Remote.ToString());
                    }                        
                    else if (quanlityAlert.Count <= 5)//2-5
                    {                        
                        return _frequencyService.GetMarkFrequencyByCode(FrequencyEnum.Occasionally.ToString());
                    }                        
                    else if (quanlityAlert.Count <= 10)//6-10
                    {                        
                        return _frequencyService.GetMarkFrequencyByCode(FrequencyEnum.Probably.ToString());
                    }                        
                    else if (quanlityAlert.Count > 10)
                    {
                        if (quanlityAlert.Where(x => x.CreatedDate >= threeLastMonth).ToList().Count() > 10 || quanlityAlert.Count > 40)// > 10 in 3 last months or > 40 in last year
                        {
                            return _frequencyService.GetMarkFrequencyByCode(FrequencyEnum.Always.ToString());
                        }
                        else// > 10 in last year
                        {
                            return _frequencyService.GetMarkFrequencyByCode(FrequencyEnum.Frequency.ToString());
                        }
                    }
                }
                return mark;
            }
            return mark;
        }
        #endregion

    }
}