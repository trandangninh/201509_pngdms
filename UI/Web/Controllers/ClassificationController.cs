using Entities.Domain.Classifications;
using Service.Interface;
using Service.QualityAlerts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Service.Security;
using Utils;
using Web.Models.Classification;
using Nois.Web.Framework.Kendoui;
using Web.Models.FoundByFunction;

namespace Web.Controllers
{
    public class ClassificationController : BaseController
    {
        private readonly IClassificationService _classificationService;
        private readonly IQualityAlertService _qualityAlertService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public ClassificationController(IClassificationService classificationService,
            IQualityAlertService qualityAlertService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _classificationService = classificationService;
            _qualityAlertService = qualityAlertService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        //list
        public ActionResult Index()
        {
            var user = _workContext.CurrentUser;
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();

            //var classifications = await _classificationService.GetAllAsync(command.Page - 1, command.PageSize);
            var classifications = _classificationService.GetList(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = classifications.Select(x => new ClassificationModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Severity = x.Severity,
                    Dectability=x.Dectability,
                    FoundByFunction = new FoundByFunctionModel() { Id = x.FoundByFunctionId ?? 0, Name = x.FoundByFunctionName}
                }),
                Total = classifications.TotalCount
            };

            return Json(gridModel);
        }


        //get all classification for Quality Alert
        [HttpPost]
        public JsonResult GetAllClassification(DataSourceRequest command, int? foundByFunctionId = null)
        {
            var classifications = _classificationService.GetAllAsync().Result.ToList();

            if (foundByFunctionId.HasValue)
                classifications = classifications.Where(x => x.FoundByFunctionId == foundByFunctionId.Value).ToList();
            //else
            //    classifications = classifications.Where(x => x.Id == -1).ToList();

            var data = classifications.Select(x => new ClassificationModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Id + " -- " + x.Name,
                    Description = x.Description,
                    FoundByFunction = new FoundByFunctionModel() { Id = x.FoundByFunctionId??0}
                });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> Create(ClassificationModel classificationModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            var existedClassification = await _classificationService.GetClassificationByClassificationCode(classificationModel.Code);
            if (existedClassification != null)
                return Content("Classification Code has Existed!");

            var classification = new Classification()
            {
                Code = classificationModel.Code,
                Name = classificationModel.Name,
                Description = classificationModel.Description,
                Severity = classificationModel.Severity,
                Dectability = classificationModel.Dectability,
                FoundByFunctionId = classificationModel.FoundByFunction.Id == 0 ? null : (int?)classificationModel.FoundByFunction.Id
            };
            await _classificationService.InsertAsync(classification);
            //return Json(classification);
            return Json(new
            {
                status = "success",
            });
        }

        [HttpPost]
        public async Task<ActionResult> Update(ClassificationModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            var classification = await _classificationService.GetByIdAsync(model.Id);
            if (classification == null)
                throw new ArgumentException("No classification found with the specified id");

            var existedClassification = await _classificationService.GetClassificationByClassificationCode(model.Code);
            if (existedClassification != null && existedClassification.Id != classification.Id)
                return Content("Classification Code has Existed!");


            classification.Code = model.Code;
            classification.Name = model.Name;
            classification.Description = model.Description;
            classification.Severity = model.Severity;
            classification.Dectability = model.Dectability;
            classification.FoundByFunctionId = model.FoundByFunction.Id == 0 ? null : (int?) model.FoundByFunction.Id;
            await _classificationService.UpdateAsync(classification);
            return Json(new
            {
                status = "success",
            });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            int count = _qualityAlertService.CountAllQualityAlertByClassificationIdAsync(id);
            if (count > 0)
                return Content("This Classification can't delete because it has references to another Quality Alerts");
            var classification = await _classificationService.GetByIdAsync(id);
            if (classification == null)
                throw new ArgumentException("No classification found with the specified id");
            await _classificationService.DeleteAsync(classification);
            return Json(new
            {
                status = "success",
            });

        }
    }
}