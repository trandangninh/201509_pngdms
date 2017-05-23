using Entities.Domain.ScMeasures;
using Entities.Domain.Suppliers;
using Nois.Web.Framework.Kendoui;
using Service.ScMeasures;
using Service.Security;
using Service.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utils;
using Web.Models.Supplier;

namespace Web.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ISupplierService _supplierService;
        private readonly IScMeasureService _scMeasureService;
        private readonly IScMeasureTargetService _scMeasureTargetService;

        public SupplierController(
            IPermissionService permissionService,
            IWorkContext workContext,
            ISupplierService supplierService,
            IScMeasureService scMeasureService,
            IScMeasureTargetService scMeasureTargetService
            )
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _supplierService = supplierService;
            _scMeasureService = scMeasureService;
            _scMeasureTargetService = scMeasureTargetService;
        }

        public  ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SearchSupplierModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();
            
            var supppliers = _supplierService.GetAllSupplierAsync(searchBySupplierName: model.SupplierName, searchVendorCode: model.VendorCode, pageIndex: command.Page - 1, pageSize: command.PageSize).Result;

            var gridModel = new DataSourceResult
            {
                Data = supppliers.Select(s => new SupplierModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Note = s.Note,
                    VendorCode = s.VendorCode,
                    VendorPrefixCode= s.VendorPrefixCode,
                    DisplayOrder = s.DisplayOrder,
                    VendorContact = s.VendorContact,
                    LocationType = new LocationTypeModel(s.LocationTypeId)
                }),
                Total = supppliers.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SupplierModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (_supplierService.CheckNameHasExisted(model.Name).Result)
                    return Content("Supplier Name has existed.");

                var supplier = new Supplier()
                {
                    Name = model.Name,
                    Note = model.Note,
                    VendorCode = model.VendorCode,
                    VendorPrefixCode= model.VendorPrefixCode,
                    DisplayOrder = model.DisplayOrder,
                    VendorContact = model.VendorContact,
                    LocationTypeId = model.LocationType.Id
                };

                try
                {
                    await _supplierService.InsertAsync(supplier);
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
        public async Task<ActionResult> Update(SupplierModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Content("Identity of Supplier is invalid.");

                var supplier = await _supplierService.GetByIdAsync(model.Id);
                if (supplier == null)
                    return Content("Can not found Supplier.");

                var exiestedSupplier = await _supplierService.GetSupplierByNameAsync(model.Name);
                if (exiestedSupplier != null && exiestedSupplier.Id != supplier.Id)
                    return Content("Supplier Name has existed.");

                supplier.Name = model.Name;
                supplier.Note = model.Note;
                supplier.VendorCode = model.VendorCode;
                supplier.VendorPrefixCode = model.VendorPrefixCode;
                supplier.DisplayOrder = model.DisplayOrder;
                supplier.VendorContact = model.VendorContact;
                supplier.LocationTypeId = model.LocationType.Id;

                try
                {
                    await _supplierService.UpdateAsync(supplier);
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
        public async Task<ActionResult> Delete(SupplierModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Json(new { status = "false", Content = "Identity of Supplier is invalid." });

                var supplier = await _supplierService.GetByIdAsync(model.Id);
                if (supplier == null)
                    return Json(new { status = "false", Content = "Can not found Supplier." });

                try
                {
                    await _supplierService.DeleteAsync(supplier);
                }
                catch (Exception ex)
                {
                    return Json(new { status = "false", Content = ex.Message });
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteSuppliers(List<int> listId)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            try
            {
                await _supplierService.DeleteSuppliersAsync(listId);
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


        public ActionResult ScMeasureTarget()
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            return View("ScMeasureTarget");
        }

        [HttpPost]
        public ActionResult ScMeasureTarget(DataSourceRequest command, int? supplierId)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            if (!supplierId.HasValue)
                supplierId = _supplierService.GetAllAsync().Result.FirstOrDefault().Id;

            var scMeasures = _scMeasureService.GetAllAsync(pageIndex: command.Page - 1, pageSize: command.PageSize).Result;
            var scMeasureTargets = _scMeasureTargetService.GetAllScMeasureTargetBySupplierId(supplierId.Value).Result;

            List<ScMeasureTargetModel> scMeasureTargetModels = new List<ScMeasureTargetModel>();
            foreach(var scMeasure in scMeasures)
            {
                ScMeasureTargetModel scMeasureTargetModel = new ScMeasureTargetModel();
                scMeasureTargetModel.ScMeasureName = scMeasure.Name;
                scMeasureTargetModel.ScMeasureId = scMeasure.Id;

                ScMeasureTarget scMeasureTarget = scMeasureTargets.FirstOrDefault(x => x.ScMeasureId == scMeasure.Id);
                if(scMeasureTarget != null)
                {
                    scMeasureTargetModel.Id = scMeasureTarget.Id;
                    scMeasureTargetModel.Target = scMeasureTarget.Target;
                }
                scMeasureTargetModels.Add(scMeasureTargetModel);
            }

            var gridModel = new DataSourceResult
            {
                Data = scMeasureTargetModels,
                Total = scMeasures.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdateScMeasureTarget(DataSourceRequest command, int supplierId, ScMeasureTargetModel scMeasureTargetModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.SupplierManagement))
                return AccessDeniedView();

            if(scMeasureTargetModel.Id > 0)
            {
                ScMeasureTarget scMeasureTarget = _scMeasureTargetService.GetByIdAsync(scMeasureTargetModel.Id).Result;
                if (scMeasureTarget == null)
                    return Content("ScMeasureTarget not existed.");

                if (scMeasureTarget.SupplierId != supplierId)
                    return Content("Supplier Identity not match with Supplier identity of ScMeasureTarget");

                if (scMeasureTarget.ScMeasureId != scMeasureTargetModel.ScMeasureId)
                    return Content("Measure Identity not match with Measure identity of ScMeasureTarget");

                scMeasureTarget.Target = scMeasureTargetModel.Target;

                try
                {
                    _scMeasureTargetService.UpdateAsync(scMeasureTarget).Wait();
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            else
            {
                Supplier supplier = _supplierService.GetByIdAsync(supplierId).Result;
                if (supplier == null)
                    return Content("Supplier not existed.");

                ScMeasure scMeasure = _scMeasureService.GetByIdAsync(scMeasureTargetModel.ScMeasureId).Result;
                if (scMeasure == null)
                    return Content("ScMeasure not exiested.");

                ScMeasureTarget existedScMeasureTarget = _scMeasureTargetService.GetScMeasureTargetBySupplierIdAndScMeasureId(supplierId, scMeasureTargetModel.ScMeasureId).Result;
                if (existedScMeasureTarget != null)
                    return Content("This ScMeasureTarget has existed.");

                ScMeasureTarget scMeasureTarget = new ScMeasureTarget();
                scMeasureTarget.ScMeasureId = scMeasureTargetModel.ScMeasureId;
                scMeasureTarget.SupplierId = supplierId;
                scMeasureTarget.Target = scMeasureTargetModel.Target;

                try
                {
                    _scMeasureTargetService.InsertAsync(scMeasureTarget).Wait();
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            return Json(new { status = "success" });           
        }
    }
}
