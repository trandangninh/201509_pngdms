using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Departments;
using Service.Interface;
using Service.Users;
using Utils;
using Web.Extend;
using Web.Models.Measure;
using System.Collections.Generic;
using Service.Security;
using Nois.Web.Framework.Kendoui;


namespace Web.Controllers
{
    public class MeasureController : BaseController
    {
        private readonly IMeasureService _measureService;
        private readonly IUserService _userService;
        private readonly IDmsService _dmsService;
        private readonly IPermissionService _permissionService;
        public MeasureController(IMeasureService measureService,
            IUserService userService,
            IDmsService dmsService,
            IPermissionService permissionService
            )
        {
            _measureService = measureService;
            _userService = userService;
            _dmsService = dmsService;
            _permissionService = permissionService;
        }

        public ActionResult Index()
        {
            if(!_permissionService.Authorize(PermissionProvider.ManageMeasure))
                return AccessDeniedView();

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, int? dmsId)
        {
            if (dmsId.HasValue)
            {
                var allMeasure = await _measureService.GetAllMeasureByDmsId(dmsId.Value, command.Page - 1, command.PageSize);

                var allMeasureModel = allMeasure.Select(
                    m => new MeasureModel
                    {
                        Id = m.Id,
                        MeasureCode = m.MeasureCode,
                        MeasureName = m.MeasureName,
                        Target = m.Target,
                        Unit = m.Unit,
                        Note = m.Note ?? "",
                        MeasureType = new MeasureModel.MeasureTypeViewModel() { Id = m.MeasureTypeId, Name = m.MeasureType.ToString() },
                        Dms = new MeasureModel.DmsViewModel() { Id = m.DmsId, Code = m.Dms.DmsCode },
                        ListUsername = m.Users.Select(u => u.Username).ToList(),
                        Active = m.Active,
                        Order = m.Order
                    }).ToList();

                var gridModel = new DataSourceResult
                {
                    Data = allMeasureModel,
                    Total = allMeasure.TotalCount
                };

                return Json(gridModel);
            }
            return Json(new { status = "dmsId is null" });
        }

        [HttpPost]
        public async Task<ActionResult> Create(MeasureModel model, int dmsId)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.MeasureCode))
                    return Content("MeasureCode is required!");

                //if (model.Dms.Id < 1)
                //    return Content("Dms is required!");

                if (model.MeasureType.Id < 1)
                    return Content("Progress is required!");

                var existMeasure = await _measureService.GetMeasureByCode(model.MeasureCode, dmsId);
                if (existMeasure != null)
                    return Content("Measure with DMS and MeasureCode like this has Existed!");

                //var checkListMeasure = _measureService.GetAllMeasures();
                //var checkMeasure =
                //    checkListMeasure.FirstOrDefault(p => p.DmsId == model.Dms.Id && p.MeasureName == model.MeasureName);
                //if (checkMeasure != null)
                //    return Content("Measure with DMS and name like this has Existed!");

                var measure = new Measure();
                measure.MeasureCode = model.MeasureCode;
                measure.MeasureName = model.MeasureName;
                measure.DmsId = dmsId;
                measure.MeasureType = (MeasureType)model.MeasureType.Id;
                measure.Target = model.Target;
                measure.Note = model.Note ?? "";
                measure.Unit = model.Unit;
                measure.CreatedDate = DateTime.Now;
                measure.UpdatedDate = DateTime.Now;
                measure.Active = model.Active;
                measure.Order = model.Order;
                await _measureService.InsertAsync(measure);
                await _measureService.UpdateMeasureOwner(measure, model.ListUsername);
                return Json(new { status = "success" });
            }
            return Content("Can not create Measure because model state is invalid");
        }


        [HttpPost]
        public async Task<ActionResult> Update(MeasureModel model, int dmsId)
        {
            if (ModelState.IsValid)
            {
                //if (model.Dms.Id < 1)
                //    return Content("Department is required!");

                if (model.MeasureType.Id < 1)
                    return Content("Progress is required!");

                var measure = await _measureService.GetByIdAsync(model.Id);
                if (measure == null)
                    return Content("No measure found with the specified id");

                //if Dms has changed
                /*if (measure.DmsId != model.Dms.Id)
                {
                    var existMeasure = _measureService.GetMeasureByCode(model.MeasureCode, dmsId);
                    if (existMeasure.Result != null)
                        return Content("Measure with DMS and MeasureCode like this has Existed!");

                    measure.MeasureCode = model.MeasureCode;
                    measure.MeasureName = model.MeasureName;
                    measure.DmsId = model.Dms.Id;
                    measure.MeasureType = (MeasureType)model.MeasureType.Id;
                    measure.Target = model.Target;
                    measure.Note = model.Note ?? "";
                    measure.Unit = model.Unit;
                    measure.UpdatedDate = DateTime.Now;
                    measure.Active = model.Active;

                    await _measureService.UpdateMeasureOwner(measure, model.ListUsername);
                    return Json(new { status = "success" });
                }*/

                if (model.MeasureCode != measure.MeasureCode)
                {
                    var existedMeasure = await _measureService.GetMeasureByMeasureCodeAndDmsId(model.MeasureCode, model.Dms.Id);
                    if(existedMeasure == null)
                        measure.MeasureCode = model.MeasureCode;
                    else
                        return Content("MeasureCode has Existed in this Dms");
                }
                measure.MeasureName = model.MeasureName;
                measure.MeasureType = (MeasureType)model.MeasureType.Id;
                measure.Target = model.Target;
                measure.Note = model.Note;
                measure.Unit = model.Unit;
                measure.UpdatedDate = DateTime.Now;
                measure.Active = model.Active;
                measure.Order = model.Order;

                await _measureService.UpdateMeasureOwner(measure, model.ListUsername);
                return Json(new { status = "success" });
            }

            return Content("Can not update Measure because model state is invalid");
        }


        //can not delete, only set active -> true or false
        //[HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{

        //    var measure = await _measureService.GetMeasureById(id);
        //    if (measure == null)
        //        throw new ArgumentException("No measure found with the specified id");
        //    await _measureService.DeleteAsync(measure);

        //    await _measureService.DeletaAllUserOfMeasure(id);
        //    return new NullJsonResult();
        //}

        [HttpPost]
        public JsonResult GetAllTypeMeasureForView(DataSourceRequest command)
        {
            var data = MeasureType.IP.EnumToList().Select(x => new { Id = x.Key, Name = x.Value }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}