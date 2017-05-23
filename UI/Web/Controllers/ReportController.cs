using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Entities.Domain.Departments;
using Service.Common;
using Service.Departments;
using Service.Interface;
using Service.Lines;
using Service.Messages;
using Service.Security;
using Service.Users;
using Web.Models.Report;
using LineResultReport = Entities.Domain.LineResultReport;
using Utils;
using Service.Dds;
using Web.Models.DdsMeeting;
using Entities.Domain.Users;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IWorkContext _workContext;
        private readonly IDdsMeetingService _ddsMeetingService;
        private readonly IDepartmentService _departmentService;

        //private readonly List<string> _listMeasureForMaking = new List<string>()
        //                                                      {
        //                                                          "SafetyTriggerCompliance ",
        //     "SafetyNearMiss",
        //     "QualityTriggerCompliance",
        //     "PrimaryQFactorIncompliance",
        //     "HoldReworkForDay",
        //     "DefectOpen",
        //     "TaskNotCompletedOnTime",
        //     "CLNotCompliance",
        //     "ScrapDueToCO",
        //     "RCOGTGOutOfTarget",
        //     "WorkOrderNotCompletionOnTime",
        //     "POMissed",
        //     "ProductionVolume",
        //    "OfUnplannedStopsDayLineConstraint",
        //     "PRImpactedToPackingLines",
        //     "EOCoachingCompletion",
        //     "QualityAlert",
        //     "FEBulk7Days",
        //     "FEBulk3Days",
        //     "PCSAdditionSigmaIncompliance",
        //     "PCSCrTzCompliance",
        //     "PRYesterday",
        //     "PROfLPD1BatchProcess",
        //     "PROfLPD2ImpactedToPackingLine",
        //     "DDIError",
        //                                                      };


        //private readonly List<string> ListMeasureForPacking = new List<string>()
        //                                                      {
        //                                                          "SafetyTriggerCompliance",
        //                                                          "SafetyNearMiss",
        //                                                          "QualityTriggerCompliance",
        //                                                          "PrimaryQFactorIncompliance",
        //                                                          "HoldReworkForDay",
        //                                                          "DefectOpen",
        //                                                          "TaskNotCompletedOnTime",
        //                                                          "CLNotCompliance",
        //                                                          "ActualScrapOnLine",
        //                                                          "AmountOfBulkProduceDay",
        //                                                          "ScaptOnlineMSU",
        //                                                          "RCOGTGOutOfTarget",
        //                                                          "WorkOrderNotCompletionOnTime",
        //                                                          "TotalPO",
        //                                                          "POMissed",
        //                                                          "MPSA",
        //                                                          "UnplannedStopsDayLineConstraint",
        //                                                          "PR",
        //                                                          "EOCoachingCompletion",
        //                                                          "QualityAlert",
        //                                                          //"DDIError",
        //                                                          "ContractorPlan",
        //                                                          "DefectFoundByLineLeader",
        //                                                          "DDI"
        //                                                      };

        public ReportController(
            IReportService reportService, 
            IWorkContext workContext,
            IDdsMeetingService ddsMeetingService,
            IDepartmentService departmentService)
        {
            
            _reportService = reportService;
            _workContext = workContext;
            _ddsMeetingService = ddsMeetingService;
            _departmentService = departmentService;
        }


        //public async Task<ActionResult> Index(int id, string fromDate, string toDate)
        //{
        //    DateTime dateSearchStart;

        //    if (String.IsNullOrEmpty(fromDate)) dateSearchStart = DateTime.Now;
        //    else
        //    {
        //        var culture = CultureInfo.CreateSpecificCulture("en-US");
        //        const DateTimeStyles styles = DateTimeStyles.None;
        //        if (!DateTime.TryParse(fromDate, culture, styles, out dateSearchStart))
        //        {
        //            dateSearchStart = DateTime.Now;
        //        }
        //    }

        //    DateTime dateSearchEnd;


        //    if (String.IsNullOrEmpty(toDate)) dateSearchEnd = DateTime.Now;
        //    else
        //    {
        //        var culture = CultureInfo.CreateSpecificCulture("en-US");
        //        const DateTimeStyles styles = DateTimeStyles.None;
        //        if (!DateTime.TryParse(toDate, culture, styles, out dateSearchEnd))
        //        {
        //            dateSearchEnd = DateTime.Now;
        //        }
        //    }

        //    var totalDay = (dateSearchEnd - dateSearchStart).TotalDays;


        //    if (_workContext.CurrentUser != null)
        //    {
        //        var department = await _departmentService.GetByIdAsync(id);
        //        var model = new ReportViewModel
        //        {
        //            DepartmentId = id,
        //            DepartmentName = department.Name,
        //            FromDate = dateSearchStart,
        //            ToDate = dateSearchEnd
        //        };
        //        for (var j = 0; j <= totalDay; j++)
        //        {
        //            var ddsMeeting = _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(j), id).Result;
        //            var lines = new List<Web.Models.Report.ReportViewModel.ResultLineModel>();
        //            if (ddsMeeting != null)
        //            {
        //                var query = ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();
        //                if (!_workContext.CurrentUser.IsAdmin())
        //                    query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));
        //                lines = query.Select((l, i) => new ReportViewModel.ResultLineModel { Name = l.LineCode, Field = "LinesInDayResult[" + j + "].Lines[" + i + "].Value", IsReadOnly = "Lines[" + i + "].IsReadOnly", LineId = l.Id, Remark = "Lines[" + i + "].Remark" }).ToList();
        //                var LineInDay = new Web.Models.Report.ReportViewModel.LineInDayModel();
        //                LineInDay.Lines = lines;
        //                model.LinesInDay.Add(LineInDay);
        //            }
        //        }

        //        return View("Index", model);
        //    }

        //    return View("Index");
        //}

        //[HttpPost]
        //public async Task<ActionResult> List(int departmentId, string fromDate, string toDate)
        //{
        //    DateTime dateSearchStart;

        //    if (String.IsNullOrEmpty(fromDate)) dateSearchStart = DateTime.Now;
        //    else
        //    {
        //        var culture = CultureInfo.CreateSpecificCulture("en-US");
        //        const DateTimeStyles styles = DateTimeStyles.None;
        //        if (!DateTime.TryParse(fromDate, culture, styles, out dateSearchStart))
        //        {
        //            dateSearchStart = DateTime.Now;
        //        }
        //    }

        //    DateTime dateSearchEnd;


        //    if (String.IsNullOrEmpty(toDate)) dateSearchEnd = DateTime.Now;
        //    else
        //    {
        //        var culture = CultureInfo.CreateSpecificCulture("en-US");
        //        const DateTimeStyles styles = DateTimeStyles.None;
        //        if (!DateTime.TryParse(toDate, culture, styles, out dateSearchEnd))
        //        {
        //            dateSearchEnd = DateTime.Now;
        //        }
        //    }

        //    var totalDay = (dateSearchEnd - dateSearchStart).TotalDays;

        //    var listReportResultModel = new List<ReportResultModel>();
        //    var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart, departmentId);
        //    var listMeasure = ddsMeeting == null ? new List<Measure>() : ddsMeeting.DdsMeetingResults.Select(d => d.Measure).Distinct();
        //    foreach (var measure in listMeasure)
        //    {
        //        var model = new ReportResultModel()
        //        {
        //            Dms = measure.Dms.DmsCode,
        //            MeasureId = measure.Id,
        //            Measure = measure.MeasureName,
        //            IPorOP = measure.MeasureType.ToString(),
        //            Owner = string.Join(",", measure.Users.Select(u => u.Username)),
        //            Target = measure.Target,
        //            Unit = measure.Unit
        //        };

        //        for (var i = 0; i <= totalDay; i++)
        //        {
        //            var ddsMeetingForLine = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(i), departmentId);
        //            var listLine = new List<Line>();
        //            if (ddsMeetingForLine != null)
        //            {
        //                var query = ddsMeetingForLine.DdsMeetingResults.Select(d => d.Line).Distinct();
        //                if (!_workContext.CurrentUser.IsAdmin())
        //                    query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));
        //                listLine = query.ToList();
        //            }
        //            var LineInDayResultModel = new Web.Models.Report.ReportResultModel.LineInDayResultModel();
        //            foreach (var line in listLine)
        //            {
        //                var resultValue = ddsMeetingForLine.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
        //                var ResultModel = new Web.Models.Report.ReportResultModel.LineModelForReportView
        //                {
        //                    LineId = line.Id,
        //                    LineName = line.LineName,
        //                    LineCode = line.LineCode,
        //                    Value = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
        //                    IsReadOnly = resultValue == null ? false : resultValue.ReadOnly,
        //                    Remark = String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark
        //                };

        //                LineInDayResultModel.Lines.Add(ResultModel);                       
        //            }
        //            model.LinesInDayResult.Add(LineInDayResultModel);                    
        //        }
        //        listReportResultModel.Add(model);
        //    }

        //    var gridModel = new DataSourceResult
        //    {
        //        Data = listReportResultModel,
        //        Total = listReportResultModel.Count()
        //    };

        //    return Json(gridModel);

        //}

        public async Task<ActionResult> Index(int id, string fromDate, string toDate)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department.DepartmentType == DepartmentType.SupplyChain)
                return RedirectToAction("Index", "SupplyChainReport");

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


            
            var model = new ReportViewModel
            {
                DepartmentId = id,
                DepartmentName = department.Name,
                FromDate = dateSearchStart,
                ToDate = dateSearchEnd
            };
            var lines = new List<Web.Models.Report.ReportViewModel.ResultLineModel>();

            int index = 0;
            for (var j = 0; j <= totalDay; j++)
            {
                var ddsMeeting = _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(j), id).Result;

                if (ddsMeeting != null)
                {
                    var query = ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();
                    if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                        query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));

                    foreach (var l in query)
                    {
                        lines.Add(new ReportViewModel.ResultLineModel 
                                    { 
                                        Name = l.LineName, 
                                        Field = "Lines[" + index + "].Value", 
                                        IsReadOnly = "Lines[" + index + "].IsReadOnly", 
                                        LineId = l.Id, 
                                        Remark = "Lines[" + index + "].Remark", 
                                        DayIndex = j,
                                        Colspan = "Lines[" + index + "].Colspan",
                                        IsHiddenForSpanColumns = "Lines[" + index + "].IsHiddenForSpanColumns",
                                        IsLastLineOfDay = "Lines[" + index + "].IsLastLineOfDay"
                                    });
                        index += 1;
                    }
                }
                model.GroupLines.Add(new ReportViewModel.GroupLine { DayIndex = j, Date = dateSearchStart.AddDays(j).ToShortDateString() });
            }
            model.Lines = lines;
            return View("Index", model);
        }


        [HttpPost]
        public async Task<ActionResult> List(int departmentId, string fromDate, string toDate)
        {
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

            var listReportResultModel = new List<ReportResultModel>();
            //var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart, departmentId);
            //var listMeasure = new List<Measure>();
            //if (ddsMeeting != null)
            //{
            //    var query = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
            //    //if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
            //    //    query = query.Where(m => m.Users.Contains(_workContext.CurrentUser));
            //    listMeasure = query.ToList();
            //}

            var count = 0;
            var listMeasure = new List<Measure>();
            while (listMeasure.Count == 0 && count <= totalDay)
            {
                var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(count), departmentId);           
                if (ddsMeeting != null)
                {
                    var query = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
                    listMeasure = query.ToList();
                }
                count++;
            }
            foreach (var measure in listMeasure)
            {
                var model = new ReportResultModel()
                {
                    Dms = measure.Dms.Description,
                    MeasureId = measure.Id,
                    MeasureName = measure.MeasureName,
                    IPorOP = measure.MeasureType.ToString() == "Null" ? "" : (measure.MeasureType.ToString() == "IPorOP" ? "IP/OP" : measure.MeasureType.ToString()),
                    Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                    Target = measure.Target,
                    Unit = measure.Unit
                };

                for (var i = 0; i <= totalDay; i++)
                {
                    var ddsMeetingForLine = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(i), departmentId);
                    var listLine = new List<Line>();
                    if (ddsMeetingForLine != null)
                    {
                        var query = ddsMeetingForLine.DdsMeetingResults.Select(d => d.Line).Distinct();
                        if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                            query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));
                        listLine = query.ToList();
                    }

                    foreach (var line in listLine)
                    {
                        var resultValue = ddsMeetingForLine.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                        var ResultModel = new Web.Models.Report.ReportResultModel.LineModelForReportView
                        {
                            LineId = line.Id,
                            LineName = line.LineName,
                            LineCode = line.LineCode,
                            Value = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                            IsReadOnly = resultValue == null ? false : resultValue.ReadOnly,
                            Remark = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark),
                            Colspan = ((listLine.FirstOrDefault() == line) && (measure.MeasureSystemTypeId == 15)) ? listLine.Count.ToString() : "",
                            IsHiddenForSpanColumns = ((listLine.FirstOrDefault() != line) && (measure.MeasureSystemTypeId == 15)),
                            IsLastLineOfDay = listLine.LastOrDefault() == line || measure.MeasureSystemTypeId == 15
                        };

                        model.Lines.Add(ResultModel);
                    }
                }
                listReportResultModel.Add(model);
            }

            var gridModel = new DataSourceResult
            {
                Data = listReportResultModel,
                Total = listReportResultModel.Count()
            };

            return Json(gridModel);

        }

        #region comment send mail to attackment
        /*
        [System.Web.Http.HttpPost]
        public async Task<JsonResult> SendMailWithAttachment(string Email, string fromDate, string toDate, int departmentId)
        {

            var listEmail = Email.Split(';');
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

            var listReportResultModel = new List<ReportResultModel>();
            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart, departmentId);
            var listMeasure = new List<Measure>();
            if (ddsMeeting != null)
            {
                var query = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
                //if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                //    query = query.Where(m => m.Users.Contains(_workContext.CurrentUser));
                listMeasure = query.ToList();
            }
            foreach (var measure in listMeasure)
            {
                var model = new ReportResultModel()
                {
                    Dms = measure.Dms.DmsCode,
                    MeasureId = measure.Id,
                    MeasureName = measure.MeasureName,
                    IPorOP = measure.MeasureType.ToString(),
                    Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                    Target = measure.Target,
                    Unit = measure.Unit
                };

                for (var i = 0; i <= totalDay; i++)
                {
                    var ddsMeetingForLine = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(i), departmentId);
                    var listLine = new List<Line>();
                    if (ddsMeetingForLine != null)
                    {
                        var query = ddsMeetingForLine.DdsMeetingResults.Select(d => d.Line).Distinct();
                        if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                            query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));
                        listLine = query.ToList();
                    }

                    foreach (var line in listLine)
                    {
                        var resultValue = ddsMeetingForLine.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                        var ResultModel = new Web.Models.Report.ReportResultModel.LineModelForReportView
                        {
                            LineId = line.Id,
                            LineName = line.LineName,
                            LineCode = line.LineCode,
                            Value = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                            IsReadOnly = resultValue == null ? false : resultValue.ReadOnly,
                            Remark = String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark
                        };

                        model.Lines.Add(ResultModel);
                    }
                }
                listReportResultModel.Add(model);
            }

            #endregion

            #region convert MeetingReportModel to MeetingReport

            var listMeetingReport = new List<MeetingReport>();

            foreach (var meetingReportModel in listReportResultModel)
            {
                var itemMeetingReport = new MeetingReport();
                var listItemLineResultReport = new List<LineResultReport>();
                foreach (var lineResultReport in meetingReportModel.Lines)
                {
                    var itemLineResultReport = new LineResultReport();
                    itemLineResultReport.LineCode = lineResultReport.LineCode;
                    itemLineResultReport.LineName = lineResultReport.LineName;
                    itemLineResultReport.Result = lineResultReport.Value;
                    listItemLineResultReport.Add(itemLineResultReport);
                }
                itemMeetingReport.MeasureType = meetingReportModel.IPorOP;
                itemMeetingReport.DmsCode = meetingReportModel.Dms;
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
                    _reportService.ExportDdsMeetingToXlsx(stream, listMeetingReport, filePath);
                    //bytes = stream.ToArray();
                    foreach (var itemEmail in listEmail)
                    {
                        if (!string.IsNullOrEmpty(itemEmail))
                        {
                            var userNeedToSendMail = await _userService.GetUserByEmailAsync(itemEmail);
                            if (userNeedToSendMail != null)
                            {
                                var listAttachment = new List<string>() { filename };
                                if (type == 1)
                                {
                                    var queueEmail = _workFlowMessageService.SendReportToUser(userNeedToSendMail, "Making", fromDate, toDate);
                                    _sendMailService.Sendmail(queueEmail, listAttachment);
                                }
                                else
                                {
                                    var queueEmail = _workFlowMessageService.SendReportToUser(userNeedToSendMail, "Packing", fromDate, toDate);
                                    _sendMailService.Sendmail(queueEmail, listAttachment);
                                }

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
        */
        #endregion
        
        
        public async Task<ActionResult> ExportDdsMeetingToExcel(int departmentId, string fromDate, string toDate)
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

            var listReportResultModel = new List<ReportResultModel>();
            //var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart, departmentId);
            //var listMeasure = new List<Measure>();
            //if (ddsMeeting != null)
            //{
            //    var query = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
            //    //if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
            //    //    query = query.Where(m => m.Users.Contains(_workContext.CurrentUser));
            //    listMeasure = query.ToList();
            //}
            var count = 0;
            var listMeasure = new List<Measure>();
            while (listMeasure.Count == 0 && count <= totalDay)
            {
                var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(count), departmentId);
                if (ddsMeeting != null)
                {
                    var query = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
                    listMeasure = query.ToList();
                }
                count++;
            }
            foreach (var measure in listMeasure)
            {
                var model = new ReportResultModel()
                {
                    Dms = measure.Dms.Description,
                    DmsId = measure.Dms.Id,
                    MeasureId = measure.Id,
                    MeasureName = measure.MeasureName,
                    IPorOP = measure.MeasureType.ToString() == "Null" ? "" : (measure.MeasureType.ToString() == "IPorOP" ? "IP/OP" : measure.MeasureType.ToString()),
                    Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                    Target = measure.Target,
                    Unit = measure.Unit
                };

                for (var i = 0; i <= totalDay; i++)
                {
                    var ddsMeetingForLine = await _ddsMeetingService.GetDdsMeetingByDate(dateSearchStart.AddDays(i), departmentId);
                    var listLine = new List<Line>();
                    if (ddsMeetingForLine != null)
                    {
                        var query = ddsMeetingForLine.DdsMeetingResults.Select(d => d.Line).Distinct();
                        if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                            query = query.Where(l => l.Users.Contains(_workContext.CurrentUser));
                        listLine = query.ToList();
                    }

                    foreach (var line in listLine)
                    {
                        var resultValue = ddsMeetingForLine.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                        var ResultModel = new Web.Models.Report.ReportResultModel.LineModelForReportView
                        {
                            Date = dateSearchStart.AddDays(i),
                            LineId = line.Id,
                            LineName = line.LineName,
                            LineCode = line.LineCode,
                            Value = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                            IsReadOnly = resultValue == null ? false : resultValue.ReadOnly,
                            //Remark = String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark,
                            IsLastLineOfDay = listLine.LastOrDefault() == line || measure.MeasureSystemTypeId == 15
                        };

                        model.Lines.Add(ResultModel);
                    }
                }
                listReportResultModel.Add(model);
            }

            #endregion

            #region convert MeetingReportModel to MeetingReport

            var listMeetingReport = new List<MeetingReport>();

            foreach (var meetingReportModel in listReportResultModel)
            {
                var itemMeetingReport = new MeetingReport();
                var listItemLineResultReport = new List<LineResultReport>();
                foreach (var lineResultReport in meetingReportModel.Lines)
                {
                    var itemLineResultReport = new LineResultReport();
                    itemLineResultReport.DateTimeCreate = lineResultReport.Date;
                    itemLineResultReport.LineCode = lineResultReport.LineCode;
                    itemLineResultReport.LineName = lineResultReport.LineName;
                    itemLineResultReport.Result = lineResultReport.Value;
                    itemLineResultReport.IsLastLineOfDay = lineResultReport.IsLastLineOfDay;
                    listItemLineResultReport.Add(itemLineResultReport);
                }
                itemMeetingReport.MeasureType = meetingReportModel.IPorOP;
                itemMeetingReport.DmsDecription = meetingReportModel.Dms;
                itemMeetingReport.DmsId = meetingReportModel.DmsId;
                itemMeetingReport.MeasureName = meetingReportModel.MeasureName;
                itemMeetingReport.Owner = meetingReportModel.Owner;
                itemMeetingReport.Target = meetingReportModel.Target;
                itemMeetingReport.Unit = meetingReportModel.Unit;
                itemMeetingReport.ListResult = listItemLineResultReport;
                listMeetingReport.Add(itemMeetingReport);
            }

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
                    _reportService.ExportDdsMeetingToXlsx(stream, listMeetingReport, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception)
            {

                return RedirectToAction("MakingReport");
            }

            #endregion
        }











        
        /*

        public async Task<ActionResult> MakingReport(string fromDate, string toDate)
        {

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

            #region result

            //thien comment
            //var listLine = _lineService.SearchLines(lineType: LineType.Making, includeDeedmacOperation: false);
            var listLine = await _lineService.SearchLines(departmentId: 2);
            var listLineCode = listLine.ToList();
            var listModel = new List<MeetingReportModel>();

            foreach (var measureCode in _listMeasureForMaking)
            {
                //thien comment
                //var measure = await _measureService.GetMeasureByCode(measureCode, (int)DmsLiquidType.Making);
                var measure = await _measureService.GetMeasureByCode(measureCode, 1);
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

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                                    dateSearchStart.AddDays(i),
                                    lineCode.LineCode,
                                    measureCode);
                            var ResultModel = new LineResultReportModel()
                                              {
                                                  DateTimeCreate = dateSearchStart.AddDays(i),
                                                  LineName = lineCode.LineName,
                                                  LineCode = lineCode.LineCode
                                              };
                            if (measureResult != null)
                            {
                                ResultModel.Result = measureResult.Result ?? "";
                            }
                            else ResultModel.Result = "";
                            model.ListResult.Add(ResultModel);
                        }
                    }
                    listModel.Add(model);
                }


            }

            #endregion

            return View("MakingReport", listModel);

        }


        public async Task<ActionResult> PackingReport(string fromDate, string toDate)
        {

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

            #region result

            //thien comment
            //var listLine = _lineService.SearchLines(lineType:LineType.Packing, includeDeedmacOperation:false);
            var listLine = await _lineService.SearchLines(departmentId: 2);
            var listLineCode = listLine.ToList();
            var listModel = new List<MeetingReportModel>();

            foreach (var measureCode in ListMeasureForPacking)
            {
                //thien comment
                //var measure = await _measureService.GetMeasureByCode(measureCode, (int)DmsLiquidType.Packing);
                var measure = await _measureService.GetMeasureByCode(measureCode, 2);
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

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                                    dateSearchStart.AddDays(i),
                                    lineCode.LineCode,
                                    measureCode);
                            var ResultModel = new LineResultReportModel()
                                              {
                                                  DateTimeCreate = dateSearchStart.AddDays(i),
                                                  LineName = lineCode.LineName,
                                                  LineCode = lineCode.LineCode
                                              };
                            if (measureResult != null)
                            {
                                ResultModel.Result = measureResult.Result ?? "";
                            }
                            else ResultModel.Result = "";
                            model.ListResult.Add(ResultModel);
                        }
                    }
                    listModel.Add(model);
                }


            }

            #endregion

            return View("PackingReport", listModel);

        }


        public ActionResult ExportPackingToExcel(string fromDate, string toDate)
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

            //thien comment
            //var listLine = _lineService.SearchLines(lineType:LineType.Packing, includeDeedmacOperation:false);
            var listLine = _lineService.SearchLines(departmentId: 2).Result;
            var listLineCode = listLine.ToList();
            var listModel = new List<MeetingReportModel>();

            foreach (var measureCode in ListMeasureForPacking)
            {
                //thien comment
                //var measure = _measureService.GetMeasureByCode(measureCode, (int)DmsLiquidType.Packing).Result;
                var measure = _measureService.GetMeasureByCode(measureCode, 2).Result;
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

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                                    dateSearchStart.AddDays(i),
                                    lineCode.LineCode,
                                    measureCode);
                            var ResultModel = new LineResultReportModel()
                                              {
                                                  DateTimeCreate = dateSearchStart.AddDays(i),
                                                  LineName = lineCode.LineName,
                                                  LineCode = lineCode.LineCode
                                              };
                            if (measureResult != null)
                            {
                                ResultModel.Result = measureResult.Result ?? "";
                            }
                            else ResultModel.Result = "";
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
                    _reportService.ExportMakingToXlsx(stream, listMeetingReport, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception)
            {

                return RedirectToAction("MakingReport");
            }

            #endregion
        }

        public ActionResult ExportMakingToExcel(string fromDate, string toDate)
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

            //thien comment
            //var listLine = _lineService.SearchLines(lineType:LineType.Making,includeDeedmacOperation:false);
            var listLine = _lineService.SearchLines(departmentId: 1).Result;
            var listLineCode = listLine.ToList();
            var listModel = new List<MeetingReportModel>();

            foreach (var measureCode in _listMeasureForMaking)
            {
                //thien comment
                //var measure = _measureService.GetMeasureByCode(measureCode, (int)DmsLiquidType.Making).Result;
                var measure = _measureService.GetMeasureByCode(measureCode, 1).Result;
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

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                                    dateSearchStart.AddDays(i),
                                    lineCode.LineCode,
                                    measureCode);
                            var ResultModel = new LineResultReportModel()
                            {
                                DateTimeCreate = dateSearchStart.AddDays(i),
                                LineName = lineCode.LineName,
                                LineCode = lineCode.LineCode
                            };
                            if (measureResult != null)
                            {
                                ResultModel.Result = measureResult.Result ?? "";
                            }
                            else ResultModel.Result = "";
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
                    _reportService.ExportMakingToXlsx(stream, listMeetingReport, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception)
            {

                return RedirectToAction("MakingReport");
            }

            #endregion
        }
        */
        #region backup SendMailWithAttachment
        /*
        [System.Web.Http.HttpPost]
        public async Task<JsonResult> SendMailWithAttachment(string Email, string fromDate, string toDate, int type)
        {

            var listEmail = Email.Split(';');
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
            //var listLine = _lineService.SearchLines(lineType:LineType.Making,includeDeedmacOperation:false);
            var listLine = await _lineService.SearchLines(departmentId: 1);
            if (type == 1)
            {
                //thien comment
                //listLine = _lineService.SearchLines(lineType:LineType.Making,includeDeedmacOperation:false);
                listLine = await _lineService.SearchLines(departmentId: 1);
            }
            else
            {
                //thien comment
                //listLine = _lineService.SearchLines(lineType: LineType.Packing, includeDeedmacOperation: false);
                listLine = await _lineService.SearchLines(departmentId: 2);
            }

            var listLineCode = listLine.ToList();
            var listModel = new List<MeetingReportModel>();



            var listMeasure = _listMeasureForMaking;

            if (type == 2)
            {
                listMeasure = ListMeasureForPacking;
            }

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

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(
                                    dateSearchStart.AddDays(i),
                                    lineCode.LineCode,
                                    measureCode);
                            var ResultModel = new LineResultReportModel()
                            {
                                DateTimeCreate = dateSearchStart.AddDays(i),
                                LineName = lineCode.LineName,
                                LineCode = lineCode.LineCode
                            };
                            if (measureResult != null)
                            {
                                ResultModel.Result = measureResult.Result ?? "";
                            }
                            else ResultModel.Result = "";
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
                itemMeetingReport.DmsCode = meetingReportModel.DmsCode;
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
                    //bytes = stream.ToArray();
                    foreach (var itemEmail in listEmail)
                    {
                        if (!string.IsNullOrEmpty(itemEmail))
                        {
                            var userNeedToSendMail = await _userService.GetUserByEmailAsync(itemEmail);
                            if (userNeedToSendMail != null)
                            {
                                var listAttachment = new List<string>() { filename };
                                if (type == 1)
                                {
                                    var queueEmail = _workFlowMessageService.SendReportToUser(userNeedToSendMail, "Making", fromDate, toDate);
                                    _sendMailService.Sendmail(queueEmail, listAttachment);
                                }
                                else
                                {
                                    var queueEmail = _workFlowMessageService.SendReportToUser(userNeedToSendMail, "Packing", fromDate, toDate);
                                    _sendMailService.Sendmail(queueEmail, listAttachment);
                                }

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
        */
        #endregion
    }

}