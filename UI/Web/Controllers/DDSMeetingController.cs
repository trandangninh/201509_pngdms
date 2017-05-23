using Entities.Domain;
using Entities.Domain.Dds;
using Entities.Domain.Departments;
using Entities.Domain.Users;
using Entities.Domain.Meetings;
using Service.Dds;
using Service.Departments;
using Service.Interface;
using Service.Lines;
using Service.Meetings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Service.Security;
using Service.SupplyChain;
using Service.Users;
using Utils;
using Web.Models.DdsMeeting;
using Web.Models.Department;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class DdsMeetingController : BaseController
    {
        private readonly IIssueService _issueService;
        private readonly IWorkContext _workContext;
        private readonly IUserInMeetingService _userInMeetingService;
        private readonly IDdsMeetingService _ddsMeetingService;
        private readonly IDdsMeetingResultService _ddsMeetingResultService;
        private readonly IDepartmentService _departmentService;
        private readonly IMeasureService _measureService;
        private readonly IDdsMeetingDetailService _ddsMeetingDetailService;
        private readonly IDdsMeetingPrDetailService _ddsMeetingPrDetailService;
        private readonly ISupplyChainMPSAService _supplyChainMPSA;
        private readonly ILineService _lineService;
        private readonly ISupplyChainDDSService _supplyChainDdsService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IMeetingService _meetingService;
        public DdsMeetingController(IIssueService issueService,
            IWorkContext workContext,
            IUserInMeetingService userInMeetingService,
            IDdsMeetingService ddsMeetingService,
            IDdsMeetingResultService ddsMeetingResultService,
            IDepartmentService departmentService,
            IMeasureService measureService,
            IDdsMeetingDetailService ddsMeetingDetailService,
            IDdsMeetingPrDetailService ddsMeetingPrDetailService,
            ISupplyChainMPSAService supplyChainMpsa,
            ILineService lineService,
            ISupplyChainDDSService supplyChainDdsService,
            IUserService userService,
            IPermissionService permissionService,
            IMeetingService meetingService)
        {
            _issueService = issueService;
            _workContext = workContext;
            _userInMeetingService = userInMeetingService;
            _ddsMeetingService = ddsMeetingService;
            _ddsMeetingResultService = ddsMeetingResultService;
            _departmentService = departmentService;
            _measureService = measureService;
            _ddsMeetingDetailService = ddsMeetingDetailService;
            _ddsMeetingPrDetailService = ddsMeetingPrDetailService;
            _supplyChainMPSA = supplyChainMpsa;
            _lineService = lineService;
            _supplyChainDdsService = supplyChainDdsService;
            _userService = userService;
            _permissionService = permissionService;
            _meetingService = meetingService;
        }

        //
        // GET: /DDSMeeting/
        public async Task<ActionResult> Index(int id, string date, int? lineId)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (!_permissionService.Authorize(department))
                return AccessDeniedView();

            if (department.DepartmentType == DepartmentType.SupplyChain)
                return RedirectToAction("Index", "SupplyChain");

            DateTime dateSearch;

            if (String.IsNullOrEmpty(date)) dateSearch = DateTime.Now.Date;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(date, culture, styles, out dateSearch))
                {
                    dateSearch = DateTime.Now.Date;
                }
            }

            //if (_workContext.CurrentUser != null)
            {
                //if (_workContext.CurrentUser != null)
                //{

                var ddsMeetingModel = new DdsMeetingModel
                {
                    DepartmentId = id,
                    DepartmentName = department.Name,
                    Date = dateSearch,
                    LineId = lineId.HasValue ? lineId.Value : 0
                };

                return View(ddsMeetingModel);
                //}
                #region comment
                /*
                if (true)
                {
                    #region result
                    var listLine = _lineService.SearchLines(departmentId: id);
                    var listLineCode = listLine.ToList();
                    var listModel = new List<ResultModel>();
                    var dms = await _dmsService.GetDmsByDepartmentId(id);                    
                    var listMeasure = dms.Measures;
                    foreach(var measure in listMeasure)
                    {
                        var model = new ResultModel()
                        {
                            Dms = measure.Dms.DmsCode,
                            //MeasureCode = measureCode,
                            Measure = measure.MeasureName,
                            IPorOP = measure.MeasureType.ToString(),
                            Owner = string.Join(",", _measureService.GetOwnerOfMeasure(measure.Id)),
                            Target = measure.Target,
                            Unit = measure.Unit
                        };

                        foreach (var lineCode in listLineCode)
                        {

                            var measureResult =
                                _noisMainMeasureService.GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime.Now.Date,
                                    lineCode.LineCode,
                                    measure.MeasureCode);
                            var measureResultConfig = _noisMainMeasureConfigService.GetMainMeasureByLineCodeAndMeasureCode(
                                                               lineCode.LineCode,
                                                               measure.MeasureCode);

                            var linec = lineCode.LineCode.Replace("(", "").Replace(")", "");
                            LineHardCodeType lc;
                            Enum.TryParse(linec, out lc);
                            var lci = (int)lc;

                            var lineRemark = _lineRemarkService.GetLineByDateAndLineCode(DateTime.Now.Date,
                                lci.ToString(), (int)LineRemarkType.Making);



                            var ResultModel = new Web.Models.DdsMeeting.ResultModel.LineModelForDdsMeetingView()
                                              {
                                                  LineName = lineCode.LineName,
                                                  LineCode = lineCode.LineCode,
                                                  //Remark = lineRemark == null ? "" : lineRemark.Remark
                                              };
                            if (measureResult != null)
                            {
                                ResultModel.Value = measureResult.Result ?? "";
                            }
                            else ResultModel.Value = "";

                            model.Lines.Add(ResultModel);
                        }
                        listModel.Add(model);
                    }
                    
                    
                    #endregion

                    //#region attendance

                    //var attendanceThisDay = _attendanceService.GetAttendancesByDateAndType(DateTime.Now.Date,
                    //    AttendanceType.Making).FirstOrDefault();
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
                    //                          ListUsernameInAttendance = _attendanceService.GetUsernameInAttendance(attendanceThisDay.Id),
                    //                          ListUserIdInAttendance = _attendanceService.GetUserIdInAttendance(attendanceThisDay.Id),
                    //                          ListUsernameNotInAttendance = _attendanceService.GetUsernameNotInAttendance(attendanceThisDay.Id),

                    //                      };
                    //}
                    //#endregion

                    //var permissionIssues = await _permissionService.Authorize(PermissionProvider.MakingWriteIssues, _workContext.CurrentUser.Username);
                    //var permissionAttendance = await _permissionService.Authorize(PermissionProvider.MakingWriteAttendance, _workContext.CurrentUser.Username);
                    //var modelPass = new MeetModel()
                    //                {
                    //                    ListMeetingResultModel = listModel,
                    //                    AttendanceModel = attendanceModel,
                    //                    permissionIssue = permissionIssues,
                    //                    permissionAttendance = permissionAttendance
                    //                };

                    //if (dateSearch.Date > DateTime.Now.Date || dateSearch < DateTime.Now.Date)
                    //{
                    //    return View("MakingByAdminNotUpdate", modelPass);
                    //}
                    return View("Index", listModel);
                }    */
                #endregion

                // return View();
            }
        }

        public ActionResult Result(int departmentId, DateTime date, int? lineId)
        {
            var department = _departmentService.GetByIdAsync(departmentId).Result;
            if (!_permissionService.Authorize(department))
                return AccessDeniedView();

            var ddsMeeting = _ddsMeetingService.GetDdsMeetingByDate(date, departmentId).Result;
            var meeting = _meetingService.GetMeetingByDepartmentId(departmentId).Result;
            var lines = new List<ResultViewModel.ResultLineModel>();
            if (ddsMeeting != null)
            {
                var query = ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();

                    query = query.Where(l => _permissionService.Authorize(l));


                if (lineId.HasValue && lineId.Value != 0)
                    query = query.Where(l => l.LineCode == _lineService.GetByIdAsync(lineId.Value).Result.LineCode);

                Dictionary<int, string> listRemark = _ddsMeetingService.LineRemarkParser(ddsMeeting.LineRemark);

                lines = query.Select((l, i) => new ResultViewModel.ResultLineModel
                                                {
                                                    Name = l.LineName,
                                                    Field = "Lines[" + i + "].Value",
                                                    IsReadOnly = "Lines[" + i + "].IsReadOnly",
                                                    LineId = l.Id,
                                                    Remark = "Lines[" + i + "].Remark",
                                                    Colspan = "Lines[" + i + "].Colspan",
                                                    IsHiddenForSpanColumns = "Lines[" + i + "].IsHiddenForSpanColumns",
                                                    LineRemark = GetLineRemarkByLineId(listRemark, l.Id)
                                                }
                                     ).ToList();
            }

            var model = new ResultViewModel
            {
                Lines = lines,
                DepartmentId = departmentId,
                Date = date,
                LineId = lineId.HasValue ? lineId.Value : 0
            };
            return PartialView("_Result", model);
        }

        private string GetLineRemarkByLineId(Dictionary<int, string> listRemark, int lineId)
        {
            string lineRemark;
            listRemark.TryGetValue(lineId, out lineRemark);
            return lineRemark;
        }
        [HttpPost]
        public async Task<ActionResult> ResultList(int departmentId, DateTime date, int? lineId)
        {
            var department = _departmentService.GetByIdAsync(departmentId).Result;
            if (!_permissionService.Authorize(department))
                return AccessDeniedView();

            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(date, departmentId);
            var listLine = new List<Line>();
            var listMeasure = new List<Measure>();
            if (ddsMeeting != null)
            {
                var queryLine = ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();
                var queryMeasure = ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();
                queryLine = queryLine.Where(l => _permissionService.Authorize(l));
                listLine = queryLine.OrderBy(l => l.Index).ToList();
                listMeasure = queryMeasure.ToList();
            }
            //var listLine = ddsMeeting == null ? new List<Line>() : ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();
            var listModel = new List<ResultModel>();
            //var listMeasure = ddsMeeting == null ? new List<Measure>() : ddsMeeting.DdsMeetingResults.Select(d => d.Measure).OrderBy(d=>d.Dms.Order).ThenBy(d=>d.Order).Distinct();
            foreach (var measure in listMeasure)
            {
                var model = new ResultModel
                {
                    Dms = measure.Dms.Description,
                    MeasureId = measure.Id,
                    MeasureName = measure.MeasureName,
                    IPorOP = measure.MeasureType.ToString() == "Null" ? "" : (measure.MeasureType.ToString() == "IPorOP" ? "IP/OP" : measure.MeasureType.ToString()),
                    Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                    Target = measure.Target,
                    Unit = measure.Unit,
                    MeasureSystemType = measure.MeasureSystemType.ToString(),
                    Readonly = (_workContext.CurrentUser == null) || ((_workContext.CurrentUser.IsEmployee() && !_workContext.CurrentUser.IsLeader()) && !_workContext.CurrentUser.IsAdmin() && departmentId == 3) || measure.MeasureSystemType == MeasureSystemType.Mpsa || measure.MeasureSystemType == MeasureSystemType.QuanlityAlert,
                    IsGuest = _workContext.CurrentUser == null
                };

                foreach (var line in listLine)
                {
                    if (lineId.HasValue && lineId.Value != 0)
                    {
                        if (measure.MeasureSystemTypeId == 15 && listLine.FirstOrDefault() == line)
                        {
                            var resultValue = ddsMeeting == null ? null : ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                            var resultModel = new ResultModel.LineModelForDdsMeetingView
                            {
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineCode = line.LineCode,
                                Value = /*((_workContext.CurrentUser == null) || resultValue == null) ? "" : */(String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                                IsReadOnly = resultValue != null && resultValue.ReadOnly,
                                Remark = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark),
                                Colspan = lineId.HasValue ? "" : (((listLine.FirstOrDefault() == line) && (measure.MeasureSystemTypeId == 15)) ? listLine.Count.ToString() : ""),
                                IsHiddenForSpanColumns = ((listLine.FirstOrDefault() != line) && (measure.MeasureSystemTypeId == 15))
                            };
                            model.Lines.Add(resultModel);
                        }
                        if (measure.MeasureSystemTypeId != 15 && line.Id == lineId.Value)
                        {
                            var resultValue = ddsMeeting == null ? null : ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                            var resultModel = new ResultModel.LineModelForDdsMeetingView
                            {
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineCode = line.LineCode,
                                Value = /*((_workContext.CurrentUser == null) || resultValue == null) ? "" : */(String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                                IsReadOnly = resultValue != null && resultValue.ReadOnly,
                                Remark = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark),
                                Colspan = ((listLine.FirstOrDefault() == line) && (measure.MeasureSystemTypeId == 15)) ? listLine.Count.ToString() : "",
                                IsHiddenForSpanColumns = ((listLine.FirstOrDefault() != line) && (measure.MeasureSystemTypeId == 15))
                            };
                            model.Lines.Add(resultModel);
                        }
                    }
                    else
                    {
                        var resultValue = ddsMeeting == null ? null : ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.MeasureId == measure.Id && r.LineId == line.Id);
                        var resultModel = new ResultModel.LineModelForDdsMeetingView
                        {
                            LineId = line.Id,
                            LineName = line.LineName,
                            LineCode = line.LineCode,
                            Value = /*((_workContext.CurrentUser == null) || resultValue == null) ? "" : */(String.IsNullOrEmpty(resultValue.Result) ? "" : resultValue.Result),
                            IsReadOnly = resultValue != null && resultValue.ReadOnly,
                            Remark = resultValue == null ? "" : (String.IsNullOrEmpty(resultValue.Remark) ? "" : resultValue.Remark),
                            Colspan = ((listLine.FirstOrDefault() == line) && (measure.MeasureSystemTypeId == 15)) ? listLine.Count.ToString() : "",
                            IsHiddenForSpanColumns = ((listLine.FirstOrDefault() != line) && (measure.MeasureSystemTypeId == 15))
                        };
                        model.Lines.Add(resultModel);
                    }



                }
                listModel.Add(model);
            }

            var gridModel = new DataSourceResult
            {
                Data = listModel,
                Total = listModel.Count()
            };

            return Json(gridModel);

        }

        [HttpPost]
        public async Task<ActionResult> ResultUpdate(int lineId, int measureId, DateTime date, string result)
        {
            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                resultValue.Result = result;
                await _ddsMeetingResultService.UpdateAsync(resultValue);

                var measure = await _measureService.GetByIdAsync(measureId);
                if (measure.MeasureSystemType == MeasureSystemType.ClScrap || measure.MeasureSystemType == MeasureSystemType.ClAmount)
                {
                    var ddsMeeting = await _ddsMeetingService.GetByIdAsync(resultValue.DdsMeetingId);
                    float scrap = 0;
                    var clScrap = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.ClScrap && r.LineId == lineId);
                    if (clScrap != null)
                        float.TryParse(clScrap.Result, out scrap);
                    var clAmount = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.ClAmount && r.LineId == lineId);
                    float amount = 0;
                    if (clAmount != null)
                        float.TryParse(clAmount.Result, out amount);
                    var clScrapPerAmount = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.ClScrapPerAmount && r.LineId == lineId);
                    if (clScrapPerAmount != null)
                    {
                        var p = amount == 0 ? 0 : (((decimal)scrap / (decimal)amount) * 100);
                        clScrapPerAmount.Result = p == 0 ? "0" : p.ToString("0.00");
                        await _ddsMeetingResultService.UpdateAsync(clScrapPerAmount);
                        return Json(new { updated = true, measureId = clScrapPerAmount.MeasureId, lineId = lineId, result = p == 0 ? "0" : p.ToString("0.00") });
                    }
                }
                if (measure.MeasureSystemType == MeasureSystemType.TotalPo)
                {
                    var ddsMeeting = await _ddsMeetingService.GetByIdAsync(resultValue.DdsMeetingId);
                    int totalPo = 0;
                    int.TryParse(result, out totalPo);
                    var pom = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.PoMissed && r.LineId == lineId);
                    int pomValue = 0;
                    if (pom != null)
                        int.TryParse(pom.Result, out pomValue);
                    var pmsa = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.Mpsa && r.LineId == lineId);
                    if (pmsa != null)
                    {
                        var p = totalPo == 0 ? 0 : ((decimal)(totalPo - pomValue) / (decimal)totalPo) * 100;
                        pmsa.Result = p == 0 ? "0" : p.ToString("0.00");
                        await _ddsMeetingResultService.UpdateAsync(pmsa);
                        var line = await _lineService.GetByIdAsync(lineId);
                        NewSupplyChainMpsa(line, ((int)SupplyChainMPSAMeasure.TotalPO).ToString(), date, totalPo, "");
                        return Json(new { updated = true, measureId = pmsa.MeasureId, lineId = lineId, result = p == 0 ? "0" : p.ToString("0.00") });
                    }
                }
                if (measure.MeasureSystemType == MeasureSystemType.PrMkLastDay)
                    SaveSupplyChainDds(date, lineId, 1, result, null, resultValue.Remark, null);
                if (measure.MeasureSystemType == MeasureSystemType.PrMkMtd)
                    SaveSupplyChainDds(date, lineId, 1, null, result, null, resultValue.Remark);
                if (measure.MeasureSystemType == MeasureSystemType.PrPkLastDay)
                    SaveSupplyChainDds(date, lineId, 2, result, null, resultValue.Remark, null, false);
                if (measure.MeasureSystemType == MeasureSystemType.PrPkMtd)
                    SaveSupplyChainDds(date, lineId, 2, null, result, null, resultValue.Remark, false);
            }

            return Json(new
            {
                status = "success"
            });
        }

        [HttpPost]
        public async Task<ActionResult> CellRemarkUpdate(int lineId, int measureId, DateTime date, string remark)
        {

            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                resultValue.Remark = remark;
                await _ddsMeetingResultService.UpdateAsync(resultValue);

                var measure = await _measureService.GetByIdAsync(measureId);

                if (measure.MeasureSystemType == MeasureSystemType.PrMkLastDay)
                    SaveSupplyChainDds(date, lineId, 1, resultValue.Result, null, remark, null);
                if (measure.MeasureSystemType == MeasureSystemType.PrMkMtd)
                    SaveSupplyChainDds(date, lineId, 1, null, resultValue.Result, null, remark);
                if (measure.MeasureSystemType == MeasureSystemType.PrPkLastDay)//FR(PK) last day remark
                    SaveSupplyChainDds(date, lineId, 2, resultValue.Result, null, remark, null, false);
                if (measure.MeasureSystemType == MeasureSystemType.PrPkMtd)//FR(PK) mtd remark
                    SaveSupplyChainDds(date, lineId, 2, null, resultValue.Result, null, remark, false);
            }
            return Json(new { remark });
        }
        public async Task<ActionResult> PomUpdate(DateTime date, int measureId, int lineId)
        {
            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                var detail = await _ddsMeetingDetailService.GetByDdsMeetingResult(resultValue.Id);
                if (detail != null)
                    return Json(new
                    {
                        pomMaking = detail.PomMaking,
                        pomPacking = detail.PomPacking,
                        pomPlaning = detail.PomPlaning,
                        pomMakingRemark = detail.PomMakingRemark,
                        pomPackingRemark = detail.PomPackingRemark,
                        pomPlaningRemark = detail.PomPlaningRemark
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> PomUpdate(FormCollection form, DateTime date)
        {
            var measureId = 0;
            var lineId = 0;
            var result = 0;
            int.TryParse(form["measureId"], out measureId);
            int.TryParse(form["lineId"], out lineId);
            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                int pomMaking, pomPacking, pomPlaning;
                int.TryParse(form["pomMaking"], out pomMaking);
                int.TryParse(form["pomPacking"], out pomPacking);
                int.TryParse(form["pomPlaning"], out pomPlaning);
                result = (pomMaking + pomPacking + pomPlaning);

                resultValue.Result = result.ToString();
                await _ddsMeetingResultService.UpdateAsync(resultValue);
                var detail = await _ddsMeetingDetailService.GetByDdsMeetingResult(resultValue.Id);
                if (detail == null)
                {
                    detail = new DdsMeetingDetail
                    {
                        DdsMeetingResultId = resultValue.Id,
                        PomMaking = pomMaking,
                        PomPacking = pomPacking,
                        PomPlaning = pomPlaning,
                        PomMakingRemark = form["pomMakingRemark"],
                        PomPackingRemark = form["pomPackingRemark"],
                        PomPlaningRemark = form["pomPlaningRemark"]
                    };
                    await _ddsMeetingDetailService.InsertAsync(detail);
                }
                else
                {

                    detail.DdsMeetingResultId = resultValue.Id;
                    detail.PomMaking = pomMaking;
                    detail.PomPacking = pomPacking;
                    detail.PomPlaning = pomPlaning;
                    detail.PomMakingRemark = form["pomMakingRemark"];
                    detail.PomPackingRemark = form["pomPackingRemark"];
                    detail.PomPlaningRemark = form["pomPlaningRemark"];
                    await _ddsMeetingDetailService.UpdateAsync(detail);
                }

                var ddsMeeting = await _ddsMeetingService.GetByIdAsync(resultValue.DdsMeetingId);
                var totalPo = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.TotalPo && r.LineId == lineId);
                var totalPoValue = 0;
                if (totalPo != null)
                    int.TryParse(totalPo.Result, out totalPoValue);
                var pmsa = ddsMeeting.DdsMeetingResults.FirstOrDefault(r => r.Measure.MeasureSystemType == MeasureSystemType.Mpsa && r.LineId == lineId);
                if (pmsa != null)
                {
                    var p = totalPoValue == 0 ? 0 : ((decimal)(totalPoValue - result) / (decimal)totalPoValue) * 100;
                    pmsa.Result = p == 0 ? "0" : p.ToString("0.00");
                    await _ddsMeetingResultService.UpdateAsync(pmsa);
                    SaveSupplyChainMpsa(date, lineId, pomMaking, pomPacking, pomPlaning, form["pomMakingRemark"], form["pomPackingRemark"], form["pomPlaningRemark"]);
                    return Json(new { updated = true, pmsaId = pmsa.MeasureId, lineId = lineId, pmsaResult = p == 0 ? "0" : p.ToString("0.00"), measureId = measureId, result = result });
                }
                SaveSupplyChainMpsa(date, lineId, pomMaking, pomPacking, pomPlaning, (string)form["pomMakingRemark"], form["pomPackingRemark"], form["pomPlaningRemark"]);
            }
            return Json(new { updated = true, measureId = measureId, lineId = lineId, result = result });
        }
        public async Task<ActionResult> PrUpdate(DateTime date, int measureId, int lineId)
        {
            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                var detail = await _ddsMeetingPrDetailService.GetByDdsMeetingResult(resultValue.Id);
                if (detail != null)
                    return Json(new
                    {
                        prLastDay = detail.PrLastDay,
                        prMtd = detail.PrMtd,
                        prLastDayRemark = detail.PrLastDayRemark,
                        prMtdRemark = detail.PrMtdRemark
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> PrUpdate(FormCollection form, DateTime date)
        {
            var measureId = 0;
            var lineId = 0;
            int.TryParse(form["measureId"], out measureId);
            int.TryParse(form["lineId"], out lineId);
            var resultValue = await _ddsMeetingResultService.GetDdsMeetingResultByMeasureIdAndLineIdAndDate(measureId, lineId, date);
            if (resultValue != null)
            {
                decimal prLastDay, prMtd;
                decimal.TryParse(form["prLastDay"], out prLastDay);
                decimal.TryParse(form["prMtd"], out prMtd);

                resultValue.Result = prLastDay.ToString();
                await _ddsMeetingResultService.UpdateAsync(resultValue);
                var detail = await _ddsMeetingPrDetailService.GetByDdsMeetingResult(resultValue.Id);
                if (detail == null)
                {
                    detail = new DdsMeetingPrDetail
                    {
                        DdsMeetingResultId = resultValue.Id,
                        PrLastDay = prLastDay,
                        PrMtd = prMtd,
                        PrLastDayRemark = form["prLastDayRemark"],
                        PrMtdRemark = form["prMtdRemark"]
                    };
                    await _ddsMeetingPrDetailService.InsertAsync(detail);
                }
                else
                {
                    detail.DdsMeetingResultId = resultValue.Id;
                    detail.PrLastDay = prLastDay;
                    detail.PrMtd = prMtd;
                    detail.PrLastDayRemark = form["prLastDayRemark"];
                    detail.PrMtdRemark = form["prMtdRemark"];
                    await _ddsMeetingPrDetailService.UpdateAsync(detail);
                }
                SaveSupplyChainDds(date, lineId, resultValue.DdsMeeting.DepartmentId, prLastDay.ToString(), prMtd.ToString(), detail.PrLastDayRemark, detail.PrMtdRemark);
                return Json(new { updated = true, measureId = measureId, lineId = lineId, result = prLastDay });
            }
            return Json(new { updated = true, measureId = measureId, lineId = lineId, result = "" });
        }

        public async Task<ActionResult> LineRemarkUpdate(DateTime date, int departmentId, int lineId)
        {
            if (!_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.Departments.All(d => d.Id != departmentId))
                return AccessDeniedView();

            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(date, departmentId);
            if (ddsMeeting != null)
            {
                var listRemark = _ddsMeetingService.LineRemarkParser(ddsMeeting.LineRemark);
                string lineRemark;
                listRemark.TryGetValue(lineId, out lineRemark);
                return Json(new
                {
                    LineRemark = string.IsNullOrEmpty(lineRemark) ? lineRemark : lineRemark.Replace("\\r\\n", System.Environment.NewLine)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> LineRemarkUpdate(FormCollection form, DateTime date)
        {
            var departmentId = 0;
            var lineId = 0;
            int.TryParse(form["departmentId-LineRemark"], out departmentId);
            int.TryParse(form["lineId-LineRemark"], out lineId);
            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(date, departmentId);
            if (ddsMeeting != null)
            {
                ddsMeeting.LineRemark = _ddsMeetingService.AddLineRemarkToXmlData(ddsMeeting.LineRemark, lineId, form["columnRemarkResult"].Replace(System.Environment.NewLine, "\\r\\n"));
                await _ddsMeetingService.UpdateAsync(ddsMeeting);
                return Json(new { updated = true, departmentId = departmentId, lineId = lineId, result = form["columnRemarkResult"] });
            }
            return Json(new { updated = true, departmentId = departmentId, lineId = lineId, result = "" });
        }

        public ActionResult Issue(int departmentId, string date, bool? showDepartmentCbx = false)
        {
            var model = new IssueListModel
            {
                DepartmentId = departmentId,
                Date = Extension.Parse(date, new DateTime()),
                CanWriteIssue = _permissionService.Authorize(PermissionProvider.WriteIssues),
                ShowDepartmentCbx = showDepartmentCbx.HasValue ? showDepartmentCbx.Value : false
            };
            model.CanWriteIssue = _permissionService.Authorize(PermissionProvider.WriteIssues);

            return PartialView("_Issue", model);
        }

        public ActionResult DoFollowUp()
        {
            var departments = _departmentService.SearchDepartment(null,true);

            var deps = departments.Where(d => _workContext.CurrentUser == null || _workContext.CurrentUser.IsAdmin() ||
                (_permissionService.Authorize(PermissionProvider.ViewIssues) && _workContext.CurrentUser.Departments.Any(ud => ud.Id == d.Id))
                                                );

            //var dep = deps.FirstOrDefault();

            var model = new IssueListModel
            {
                //DepartmentId = dep == null ? 0 : dep.Id,
                //default is Choose all department fillter
                DepartmentId = 0,
                ShowDepartmentCbx = true
                //Date = Extension.Parse(date, DateTime.Today)
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> IssueList(DataSourceRequest command, IssueListModel model)
        {
            //var userId = _workContext.CurrentUser == null? 0: _workContext.CurrentUser.IsAdmin() ? 0 : _workContext.CurrentUser.Id;
            var includeUserCreated = false;
            var includeAllUserInDepartment = false;
            List<int> users = null;

            //User can view all issue in department which he belong to. Lastest request of Mr. Nhi
            //if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && !_workContext.CurrentUser.IsLeader())
            //{
            //    users = new List<int> { _workContext.CurrentUser.Id };
            //    includeUserCreated = true;
            //}
            //else
            users = String.IsNullOrEmpty(model.UserId) ? null : model.UserId.Split(',').Select(int.Parse).ToList();



            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin())
            {
                includeAllUserInDepartment = true;
                //if user not is guest or not is admin => not have option all department ==> department alway has value > 0
                if (model.DepartmentId == 0 && _workContext.CurrentUser.Departments.Count > 0)
                    model.DepartmentId = _workContext.CurrentUser.Departments.Select(d => d.Id).FirstOrDefault();
            }

            var status = String.IsNullOrEmpty(model.IssueStatusId) ? null : model.IssueStatusId.Split(',').Select(int.Parse).ToList();

            var issues = await _issueService.SearchIssues(includeAllUserInDepartment, users, includeUserCreated, searchKeyword: model.SearchString, statusId: status, createdDate: model.Date, departmentId: model.DepartmentId, pageIndex: command.Page - 1, pageSize: command.PageSize);

            var listIssueModel = issues.Select(p => new IssueListModel.IssueModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedDate = p.CreatedDate.ToShortDateString(),
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                NextStep = p.NextStep,
                Status = p.IssueStatus.ToString(),
                StatusId = (int)p.IssueStatus,
                UserCreated = p.User.Username,
                UserAssigned = p.UserOwner != null ? p.UserOwner.Username : "",
                UserAssignedId = p.UserOwnerId,
                Type = p.Department.Name,
                ActionPlan = p.ActionPlan,
                When = p.When,
                Note = p.Note,
                SystemFixDMSLinkage = p.SystemFixDmsLinkage,
                WhenDue = p.WhenDue.ToShortDateString(),
                IsAllowEdit = _permissionService.Authorize(PermissionProvider.WriteIssues) && _permissionService.Authorize(p)
            });

            var result = new DataSourceResult
            {
                Data = listIssueModel, // Process data (paging and sorting applied)
                Total = issues.TotalCount // Total number of records
            };

            // Return the result as JSON
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(IssueListModel.IssueModel model, int departmentId)
        {
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.Departments.All(d => d.Id != departmentId))
                return AccessDeniedView();

            if (!_permissionService.Authorize(PermissionProvider.WriteIssues)
                || (!_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.Departments.All(d => d.Id != departmentId)))
                return Content("You don't have permission, please contact with Administrator to be supported");

            var status = (IssueStatus)model.StatusId;

            var newIssue = new Issue
            {
                Content = model.Content,
                CreatedDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                NextStep = model.NextStep,
                IssueStatus = status,
                DepartmentId = model.DepartmentId,
                UserId = _workContext.CurrentUser.Id,
                ActionPlan = model.ActionPlan,
                When = model.When == null ? "" : Extension.ParseKendoDateTimeString(model.When, DateTime.Today).ToShortDateString(),
                Note = model.Note,
                SystemFixDmsLinkage = model.SystemFixDMSLinkage,
                UserOwnerId = model.UserAssignedId,
                WhenDue = Extension.ParseKendoDateTimeString(model.WhenDue, DateTime.Today)
            };
            await _issueService.InsertAsync(newIssue);

            //var userMail = await _userService.GetUserByIdAsync(model.UserAssignedId);
            //var listAttachment = new List<string>();
            //var queueEmail1 = _workFlowMessageService.SendIssusToUser(userMail, newIssue);
            //var result = _sendMailService.Sendmail(queueEmail1, listAttachment);

            return Json(new { satus = "success" });
        }
        public async Task<ActionResult> Update(IssueListModel.IssueModel model)
        {
            var issue = await _issueService.GetByIdAsync(model.Id);

            //if (!_permissionService.Authorize(PermissionProvider.WriteIssues)
            //    || ((issue.UserOwner != _workContext.CurrentUser) && (issue.User != _workContext.CurrentUser) && !_workContext.CurrentUser.IsAdmin()))
            //    return Content("You don't have permission, please contact with Administrator to be supported");

            var currentUser = _workContext.CurrentUser;
            if (currentUser != null && !currentUser.IsAdmin())
            {
                //check permission
                if (!_permissionService.Authorize(PermissionProvider.WriteIssues)
                    || !_permissionService.Authorize(issue))
                    return Content("You don't have permission, please contact with Administrator to be supported");
            }


            if (issue != null)
            {
                issue.Content = model.Content;
                issue.NextStep = model.NextStep;
                issue.IssueStatus = (IssueStatus)model.StatusId;
                issue.UpdatedDate = DateTime.Now;
                issue.ActionPlan = model.ActionPlan;
                issue.When = model.When == null ? "" : Extension.ParseKendoDateTimeString(model.When, DateTime.Today).ToShortDateString();
                issue.WhenDue = Extension.ParseKendoDateTimeString(model.WhenDue, DateTime.Today);
                issue.Note = model.Note;
                issue.SystemFixDmsLinkage = model.SystemFixDMSLinkage;
                issue.UserOwnerId = model.UserAssignedId;
                issue.DepartmentId = model.DepartmentId;
                await _issueService.UpdateAsync(issue);

                //if (_workContext.CurrentUser.IsAdmin())
                //{
                //var userMail = await _userService.GetUserByUsernameAsync(model.UserAssigned);
                //var listAttachment = new List<string>();
                //var queueEmail1 = _workFlowMessageService.SendIssusToUser(userMail, issue);
                //_sendMailService.Sendmail(queueEmail1, listAttachment);
                //}
                //else
                //{
                //var userCreate = await _userService.GetUserByUsernameAsync(model.UserCreated);
                //var listAttachment = new List<string>();
                //var queueEmail1 = _workFlowMessageService.SendIssusToOwner(userCreate, issue, model.UserAssigned);
                //_sendMailService.Sendmail(queueEmail1, listAttachment);
                //}
            }

            return Json(new { satus = "success" });
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var currentUser = _workContext.CurrentUser;
            if (currentUser != null && !currentUser.IsAdmin())
            {
                //check permission
                if (!_permissionService.Authorize(PermissionProvider.WriteIssues)
                    || (currentUser != null && currentUser.IsEmployee() && !currentUser.Departments.All(d => !_meetingService.CheckUserIsMeetingLeaderByDepartmentIdAndUserId(d.Id, currentUser.Id))))
                    return Content("You don't have permission, please contact with Administrator to be supported");
            }

            var issue = await _issueService.GetIssueById(id);
            if (issue == null)
                throw new ArgumentException("No line found with the specified id");

            await _issueService.DeleteAsync(issue);
            return Json(new { satus = "success" });
        }
        public ActionResult Attendance(int departmentId, string date)
        {
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.Departments.All(d => d.Id != departmentId))
                return AccessDeniedView();

            var dateSearch = Extension.Parse(date, DateTime.Today);

            var ddsMeeting = _ddsMeetingService.GetDdsMeetingByDate(dateSearch, departmentId).Result;
            var allUsersInMeetting = _userInMeetingService.GetAllUserInMeetingByDepartmentId(departmentId);

            var model = new AttendanceModel
            {
                DepartmentId = departmentId,
                Date = dateSearch,
                UserAttendanceModels = allUsersInMeetting.Select(u =>
                    new AttendanceModel.UserAttendanceModel
                    {
                        Id = u.UserId,
                        Name = u.User.Username,
                        IsAttendance = ddsMeeting != null && (ddsMeeting.Attendances.Count == 0 || ddsMeeting.Attendances.Any(m => m.IsAttendance && m.UserId == u.UserId))
                    }).ToList()
            };
            model.CanWriteAttendance = (_permissionService.Authorize(PermissionProvider.WriteAttendance));

            return PartialView("_Attendance", model);
        }
        [HttpPost]
        public async Task<ActionResult> Attendance(int departmentId, DateTime date, FormCollection form)
        {
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.Departments.All(d => d.Id != departmentId))
                return AccessDeniedView();
            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(date, departmentId);

            if (ddsMeeting == null)
                return Json(new { status = "false", message = "DDS meeting is not available. Please contact to admin to be supported." });

            var allUsersInMeetting = _userInMeetingService.GetAllUserInMeetingByDepartmentId(departmentId);

            foreach (var userInMeeting in allUsersInMeetting)
            {
                string formKey = "allow_" + userInMeeting.UserId;
                var attendance = ddsMeeting.Attendances.FirstOrDefault(a => a.UserId == userInMeeting.UserId);
                if (attendance == null)
                {
                    attendance = new Attendance
                    {
                        DdsMeetingId = ddsMeeting.Id,
                        UserId = userInMeeting.UserId,
                        IsAttendance = form[formKey] != null
                    };
                    ddsMeeting.Attendances.Add(attendance);
                }
                else
                {
                    attendance.IsAttendance = form[formKey] != null;
                }
            }

            //remove all users whom not in user meeting list
            foreach (var attendance in ddsMeeting.Attendances.Where(a => allUsersInMeetting.All(u => u.UserId != a.UserId)))
            {
                ddsMeeting.Attendances.Remove(attendance);
            }

            await _ddsMeetingService.UpdateAsync(ddsMeeting);

            return Json(new { status = "success" });
        }
        [HttpPost]
        public async Task<JsonResult> GetAllLineForDdsMeetingByDepartmentIdAndDate(int departmentId, DateTime date)
        {

            var ddsMeeting = await _ddsMeetingService.GetDdsMeetingByDate(date, departmentId);
            var listLine = ddsMeeting == null ? new List<Line>() : ddsMeeting.DdsMeetingResults.Select(d => d.Line).Distinct();
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin() && _workContext.CurrentUser.IsEmployee())
                listLine = listLine.Where(l => l.Users.Contains(_workContext.CurrentUser));
            var data = listLine.Select(x => new { Id = x.Id, LineName = x.LineName });
            return Json(data.OrderBy(x => x.Id), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetAllIssueStatus()
        {
            var data = IssueStatus.Closed.EnumToList().Select(x => new
            {
                Id = x.Key,
                Name = x.Value
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAllDepartment(DataSourceRequest command)
        {
            var departments = _departmentService.SearchDepartment(null,true);

            var udids = _workContext.CurrentUser.Departments.Select(d => d.Id).ToList();

            var deps = departments.Where(d => (_permissionService.Authorize(PermissionProvider.ViewIssues) && (_workContext.CurrentUser != null && udids.Any(id => id == d.Id))
                                                || (_workContext.CurrentUser == null || _workContext.CurrentUser.IsAdmin())));
            var data = deps.Select(x => new DepartmentModel
            {
                Id = x.Id,
                Name = x.Name
            }).OrderBy(d => d.Id).ToList();
            if (_workContext.CurrentUser == null || _workContext.CurrentUser.IsAdmin())
                data.Insert(0, new DepartmentModel() { Id = 0, Name = "--  All Department  --" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAllUser(DataSourceRequest command, int departmentId)
        {
            var users = _userService.GetAllUsersAsync();
            var data = users.Where(u => (u.Active == true) && (departmentId == 0 || u.Departments.Any(d=>d.Id == departmentId))).Select(x => new Web.Models.Meeting.UserForMeetingViewModel
            {
                Id = x.Id,
                Name = x.Username
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private void SaveSupplyChainMpsa(DateTime date, int lineId, int makingValue, int packingValue, int planingValue, string makingRemark, string packingRemark, string planingRemark)
        {
            var line = _lineService.GetByIdAsync(lineId).Result;
            NewSupplyChainMpsa(line, ((int)SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking).ToString(), date, makingValue, makingRemark);
            NewSupplyChainMpsa(line, ((int)SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPacking).ToString(), date, packingValue, packingRemark);
            NewSupplyChainMpsa(line, ((int)SupplyChainMPSAMeasure.ReasonCodePOMissedduetoPlanning).ToString(), date, planingValue, planingRemark);
        }

        private void NewSupplyChainMpsa(Line line, string measureName, DateTime date, int value, string remark)
        {
            var supplychainMaking = _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(measureName, date);
            var lastSupplyChainMaking =  _supplyChainMPSA.GetSupplyChainMPSAMeasureCodeAndDate(measureName, date.AddDays(-1));
            string owner = string.Empty;
            if (lastSupplyChainMaking != null)
                owner = lastSupplyChainMaking.Owner;
            else
            {
                var userOwner = _userService.GetAllUsersAsync().LastOrDefault();
                if (userOwner != null)
                    owner = userOwner.Username;
            }
  
            if (supplychainMaking != null)
            {
                UpdateSupplyChainMpsa(line, supplychainMaking, value, remark);
                _supplyChainMPSA.UpdateAsync(supplychainMaking).Wait();
            }
            else
            {
                var newModel = new SupplyChainMPSA()
                {
                    CreatedDate = date,
                    Owner = owner,
                    UpdatedDate = date,
                    MeasureCode = (int)SupplyChainMPSAMeasure.ReasonCodePOMissedduetoMaking,                    
                };
                UpdateSupplyChainMpsa(line, newModel, value, remark);
                _supplyChainMPSA.CreateAsync(newModel).Wait();
                _supplyChainMPSA.CreateUserInSupplyChainMPSAAsync(new UserInSupplyChainMPSA()
                {
                    SupplyChainMPSAId = newModel.Id,
                    UserId = _workContext.CurrentUser == null ? 0 : _workContext.CurrentUser.Id
                }).Wait();
            }
        }
        private void UpdateSupplyChainMpsa(Line line, SupplyChainMPSA mpsa, int value, string remark)
        {
            if (line.LineCode == "FR")
            {
                mpsa.FR = value;
                mpsa.FRRemarks = remark;
            }
            if (line.LineCode == "Bottle")
            {
                mpsa.Bottle = value;
                mpsa.BottleRemarks = remark;
            }
            if (line.LineCode == "Sachet1")
            {
                mpsa.Sachet1 = value;
                mpsa.Sachet1Remarks = remark;
            }
            if (line.LineCode == "Sachet2")
            {
                mpsa.Sachet2 = value;
                mpsa.Sachet2Remarks = remark;
            }
            if (line.LineCode == "Pouch")
            {
                mpsa.Pouch = value;
                mpsa.PouchRemark = remark;
            }
        }

        private void SaveSupplyChainDds(DateTime date, int lineId, int type, string lastDay, string mtd, string lastDayRemark, string mtdRemark, bool isMaking = true)
        {
            var line = _lineService.GetByIdAsync(lineId).Result;
            if (lastDay != null)
                NewSupplyChainDds(date, line, (int)SupplyChainDDSMeasure.PRLastDay, type, lastDay, lastDayRemark, isMaking);
            if (mtd != null)
                NewSupplyChainDds(date, line, (int)SupplyChainDDSMeasure.PRMTD, type, mtd, mtdRemark, isMaking);

        }
        private void NewSupplyChainDds(DateTime date, Line line, int measureCode, int type, string value, string remark, bool isMaking = true)
        {

            var supplychainResult = _supplyChainDdsService.GetSupplyChainDDSMeasureCodeAndDateAndType(measureCode.ToString(), date, type);
            var lastSupplyChainResult = _supplyChainDdsService.GetSupplyChainDDSMeasureCodeAndDateAndType(measureCode.ToString(), date.AddDays(-1), type);
            string owner = string.Empty;
            if (lastSupplyChainResult != null)
                owner = lastSupplyChainResult.Owner;
            else
            {
                var userOwner = _userService.GetAllUsersAsync().LastOrDefault();
                if (userOwner != null)
                    owner = userOwner.Username;
                    
            }
            var currentUser = _workContext.CurrentUser;

            if (supplychainResult == null)
            {
                var newModel = new SupplyChainDDS()
                {
                    CreatedDate = date,
                    Owner = owner,
                    UpdatedDate = DateTime.Now,
                    MeasureCode = measureCode,
                    type = type,
                    UserCreatedId = currentUser.Id,
                    UserUpdatedId = currentUser.Id,
                };
                UpdateSupplyChainDds(line, newModel, value, remark, isMaking);
                _supplyChainDdsService.CreateAsync(newModel).Wait();
            }
            else
            {
                supplychainResult.CreatedDate = date;
                supplychainResult.Owner = supplychainResult.Owner == null ? owner : supplychainResult.Owner;
                supplychainResult.UpdatedDate = DateTime.Now;
                supplychainResult.MeasureCode = measureCode;
                supplychainResult.type = type;
                UpdateSupplyChainDds(line, supplychainResult, value, remark, isMaking);
                _supplyChainDdsService.UpdateAsync(supplychainResult).Wait();
            }
        }
        private void UpdateSupplyChainDds(Line line, SupplyChainDDS dds, string value, string remark, bool isMaking = true)
        {
            if (line.LineCode == "FE Batch")
                dds.Batch = value;
            if (line.LineCode == "Bottle")
            {
                dds.Bottle = value;
                dds.BottleRemark = remark;
            }
            if (line.LineCode == "Sachet1")
            {
                dds.Sachet = value;
                dds.SachetRemark = remark;
            }

            if (line.LineCode == "Pouch")
            {
                dds.Pouch = value;
                dds.PouchRemark = remark;
            }

            if (line.LineCode == "LPD1")
                dds.LPD1 = value;
            if (line.LineCode == "LPD2")
                dds.LPD2 = value;
            if (line.LineCode == "LPD3")
                dds.LPD3 = value;
            if (line.LineCode == "FR")
            {
                if (isMaking)
                {
                    dds.FRMK = value;
                }
                else
                {
                    dds.FRPK = value;
                    dds.FRPKRemark = remark;
                }
            }
        }

    }
}
