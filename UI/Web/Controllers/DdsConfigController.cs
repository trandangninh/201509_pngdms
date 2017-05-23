using Entities.Domain;
using Entities.Domain.Dds;
using Nois.Web.Framework.Kendoui;
using Service.Dds;
using Service.Departments;
using Service.Interface;
using Service.Lines;
using Service.Security;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utils;
using Web.Models.DdsConfig;

namespace Web.Controllers
{
    public class DdsConfigController : Controller
    {
        private readonly ILineService _lineService;
        private readonly IMeasureService _measureService;
        private readonly IWorkContext _workContext;
        private readonly IDdsConfigService _ddsConfigService;

        public DdsConfigController(ILineService lineService,
            IMeasureService measureService,
            IWorkContext workContext,
            IDdsConfigService ddsConfigService
            )
        {
            _lineService = lineService;
            _measureService = measureService;
            _workContext = workContext;
            _ddsConfigService = ddsConfigService;
        }
        //
        // GET: /DdsConfig/
        public ActionResult Index(int id)
        {
            if (_workContext.CurrentUser != null)
            {
                var listLine = _lineService.SearchLines(departmentId: id, active: true).Result;

                var model = new DdsConfigTransportModel
                {
                    //Lines = listLine.Select((l, i) => new DdsConfigTransportModel.ResultLineModelForDdsConfig { Name = l.LineName, Field = "Lines[" + i + "].Active", Template = "Lines[" + i + "].Result" }).ToList(),
                    Lines = listLine.Select((l, i) => new DdsConfigTransportModel.ResultLineModelForDdsConfig 
                                                        { 
                                                            Name = l.LineName, 
                                                            Field = "Lines[" + i + "].ReadOnly", 
                                                            Template = "Lines[" + i + "].Result", 
                                                            View = "Lines[" + i + "][ReadOnly]", 
                                                            LineId = l.Id,
                                                            Colspan = "Lines[" + i + "].Colspan",
                                                            IsHiddenForSpanColumns = "Lines[" + i + "].IsHiddenForSpanColumns"
                                                        }).ToList(),
                    //Lines = listLine.Select((l, i) => new DdsConfigTransportModel.ResultLineModelForDdsConfig { Name = l.LineName, Field = l.LineCode, Template = l.LineName + ".Result" }).ToList(),
                    DepartmentId = id
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult List(int departmentId)
        {
            var listLine = _lineService.SearchLines(departmentId: departmentId, active: true).Result;
            var listModel = new List<DdsConfigModel>();
            var listDdsConfig = _ddsConfigService.GetDdsConfigByDepartmentId(departmentId);
            var listMeasure = _measureService.GetAllMeasureByDepartmentId(departmentId).OrderBy(d => d.Dms.Order).ThenBy(d => d.Order).Distinct();;
            foreach (var measure in listMeasure)
            {
                var model = new DdsConfigModel()
                {
                    DepartmentId = departmentId,
                    Dms = measure.Dms.Description,
                    MeasureId = measure.Id,
                    Measure = measure.MeasureName,
                    MeasureType = measure.MeasureType.ToString() == "Null" ? "" : (measure.MeasureType.ToString() == "IPorOP" ? "IP/OP" : measure.MeasureType.ToString()),
                    Owner = string.Join(",", measure.Users.Select(u => u.Username)),
                    Target = measure.Target,
                    Unit = measure.Unit
                };

                foreach (var line in listLine)
                {
                    var measureResult = listDdsConfig.FirstOrDefault(d => d.LineId == line.Id && d.MeasureId == measure.Id);

                    var ResultModel = new LineResultOfDdsConfig()
                    {
                        Id = line.Id,
                        LineName = line.LineName,
                        LineCode = line.LineCode,
                        Colspan = ((listLine.FirstOrDefault() == line) && (measure.MeasureSystemTypeId == 15)) ? listLine.Count.ToString() : "",
                        IsHiddenForSpanColumns = ((listLine.FirstOrDefault() != line) && (measure.MeasureSystemTypeId == 15))
                    };
                    if (measureResult != null)
                    {
                        ResultModel.DdsConfigId = measureResult.Id;
                        ResultModel.ReadOnly = true;
                    }
                    else
                    {
                        ResultModel.ReadOnly = false;
                    }

                    model.Lines.Add(ResultModel);
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


        //[HttpPost]
        //public async Task<ActionResult> Update(DdsConfigModel model)
        //{
        //    var listDdsConfig = _ddsConfigService.GetDdsConfigByDepartmentId(model.DepartmentId);
        //    foreach (var line in model.Lines)
        //    {
        //        var existedDdsConfig = listDdsConfig.FirstOrDefault(d => d.MeasureId == model.MeasureId && d.LineId == line.Id);
        //        if (line.ReadOnly && existedDdsConfig == null)
        //        {
        //            var newMeetingResult = new DdsConfig()
        //            {
        //                LineId = line.Id,
        //                UserCreatedId = _workContext.CurrentUser.Id,
        //                MeasureId = model.MeasureId,
        //                CreatedDateTime = DateTime.Now

        //            };
        //            await _ddsConfigService.InsertAsync(newMeetingResult);
        //        }
        //        if (!line.ReadOnly && existedDdsConfig != null)
        //        {
        //            await _ddsConfigService.DeleteAsync(existedDdsConfig);
        //        }
        //    }

        //    return Json(new { status = "success"});           
        //}

        [HttpPost]
        public async Task<ActionResult> Update(int departmentId, int measureId, int lineId, bool readOnly)
        {
            var listDdsConfig = _ddsConfigService.GetDdsConfigByDepartmentId(departmentId);
            var existedDdsConfig = listDdsConfig.FirstOrDefault(d => d.MeasureId == measureId && d.LineId == lineId);
            if (readOnly && existedDdsConfig == null)
            {
                var newMeetingResult = new DdsConfig()
                {
                    LineId = lineId,
                    UserCreatedId = _workContext.CurrentUser.Id,
                    MeasureId = measureId,
                    CreatedDateTime = DateTime.Now

                };
                await _ddsConfigService.InsertAsync(newMeetingResult);
            }
            if (!readOnly && existedDdsConfig != null)
            {
                await _ddsConfigService.DeleteAsync(existedDdsConfig);
            }
            return Json(new { status = "success" });
        }
    }
}