using Entities.Domain.ClassificationDefects;
using Nois.Web.Framework.Kendoui;
using Service.ClassificationDefects;
using Service.Security;
using Service.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils;
using Web.ClassificationDefects;
using Web.Models;
using Web.Models.ClassificationDefect;
using Web.Models.Common;

namespace Web.Controllers
{
    public class ClassificationDefectController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IClassificationDefectService _classificationDefectService;
        private readonly IMaterialService _materialService;
        private readonly ISupplierService _supplierService;

        public ClassificationDefectController(
            IPermissionService permissionService,
            IWorkContext workContext,
            IClassificationDefectService classificationDefectService,
            IMaterialService materialService,
            ISupplierService supplierService
            )
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _classificationDefectService = classificationDefectService;
            _materialService = materialService;
            _supplierService = supplierService;
        }

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ClassificationDefectManagement))
                return AccessDeniedView();

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SearchClassificationDefectModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ClassificationDefectManagement))
                return AccessDeniedView();

            var classificationDefects = _classificationDefectService.GetAllClassificationDefectAsync(searchByClassificationDefectName: model.ClassificationDefectName,
                                                                                                        pageIndex: command.Page - 1,
                                                                                                        pageSize: command.PageSize).Result;

            var materials = _materialService.GetAllMaterials();
            var data = materials.Select(m => new MaterialModel
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();

            DataSourceResult gridModel;
            gridModel = new DataSourceResult
            {
                Data = classificationDefects.Select(s => new ClassificationDefectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Note = s.Note,
                    DisplayOrder = s.DisplayOrder,
                    Materials = ConvertMaterialIdsToMaterialModels(materials, s.Materials),
                    Suppliers = s.Suppliers.Select(x => new SelectionModel { Id = x.Id, Name = x.Name }).ToList()
                }),
                Total = classificationDefects.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        private List<MaterialModel> ConvertMaterialIdsToMaterialModels(IList<Material> materials, string materialIds)
        {
            if (materials == null)
                return null;

            List<int> listMaterialId;
            if (!string.IsNullOrEmpty(materialIds))
            {
                try
                {
                    listMaterialId = materialIds.Split(',').Select(Int32.Parse).ToList();
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;

            var listMaterialModel = new List<MaterialModel>();

            return materials.Where(m => listMaterialId.Any(i => i == m.Id))
                .Select(m => new MaterialModel() { Id = m.Id, Name = m.Name })
                .ToList();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ClassificationDefectModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ClassificationDefectManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (_classificationDefectService.CheckNameHasExisted(model.Name).Result)
                    return Content("Classification Defect Name has existed.");

                if (model.Materials.FirstOrDefault() == null)
                    model.Materials = null;

                try
                {
                    var classificationDefect = new ClassificationDefect()
                    {
                        Name = model.Name,
                        Note = model.Note,
                        DisplayOrder = model.DisplayOrder,
                        Materials = ConvertMaterialModelsToStringMaterialIds(model.Materials)
                    };

                    await _classificationDefectService.InsertAsync(classificationDefect);

                    foreach (var supplier in model.Suppliers)
                    {
                        var supplierEntity = await _supplierService.GetByIdAsync(supplier.Id);
                        if (supplierEntity == null)
                        {
                            await _classificationDefectService.UpdateAsync(classificationDefect);
                            return Content("Identity of Supplier Defect is invalid.");
                        }

                        classificationDefect.Suppliers.Add(supplierEntity);
                    }

                    await _classificationDefectService.UpdateAsync(classificationDefect);

                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        private string ConvertMaterialModelsToStringMaterialIds(List<MaterialModel> materials)
        {
            if (materials == null || materials.FirstOrDefault() == null)
                return string.Empty;

            return string.Join(",", materials.Select(m => m.Id).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Update(ClassificationDefectModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ClassificationDefectManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Content("Identity of Classification Defect is invalid.");

                var classificationDefect = await _classificationDefectService.GetByIdAsync(model.Id);
                if (classificationDefect == null)
                    return Content("Can not found Classification Defect.");

                var exiestedClassificationDefect = await _classificationDefectService.GetClassificationDefectByNameAsync(model.Name);
                if (exiestedClassificationDefect != null && exiestedClassificationDefect.Id != classificationDefect.Id)
                    return Content("Classification Defect Name has existed.");

                try
                {
                    classificationDefect.Name = model.Name;
                    classificationDefect.Note = model.Note;
                    classificationDefect.DisplayOrder = model.DisplayOrder;
                    classificationDefect.Materials = ConvertMaterialModelsToStringMaterialIds(model.Materials);

                    var listRemove = classificationDefect.Suppliers.Select(x => x.Id).Where(x => model.Suppliers.All(y => y.Id != x)).ToList();

                    foreach(var supplierId in listRemove)
                    {
                        var supplierEntity = await _supplierService.GetByIdAsync(supplierId);
                        if (supplierEntity == null)
                        {
                            await _classificationDefectService.UpdateAsync(classificationDefect);
                            return Content("Identity of Supplier Defect is invalid.");
                        }

                        classificationDefect.Suppliers.Remove(supplierEntity);
                    }

                    var listAdd = model.Suppliers.Where(x => classificationDefect.Suppliers.All(y => y.Id != x.Id));

                    foreach (var supplier in listAdd)
                    {
                        //get testing sample identity
                        var supplierEntity = await _supplierService.GetByIdAsync(supplier.Id);
                        //check if it not existed
                        if (supplierEntity == null)
                        {
                            await _classificationDefectService.UpdateAsync(classificationDefect);
                            return Content("Identity of Supplier Defect is invalid.");
                        }

                        classificationDefect.Suppliers.Add(supplierEntity);
                    }

                    await _classificationDefectService.UpdateAsync(classificationDefect);
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteClassificationDefects(List<int> listId)
        {
            if (!_permissionService.Authorize(PermissionProvider.ClassificationDefectManagement))
                return AccessDeniedView();

            try
            {
                await _classificationDefectService.DeleteClassificationDefectsAsync(listId);
            }
            catch (Exception ex)
            {
                return Json(new { status = "false", Content = ex.Message });
            }

            return Json(new
            {
                status = "success",
            });
        }

        //get all materials for classification defect
        [HttpPost]
        public JsonResult GetAllMaterials(DataSourceRequest command)
        {
            var materials = _materialService.GetAllMaterials();
            var data = materials.Select(m => new MaterialModel
            {
                Id = m.Id,
                Name = m.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
