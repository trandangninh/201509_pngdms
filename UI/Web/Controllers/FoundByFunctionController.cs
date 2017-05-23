using Service.Interface;
using Service.QualityAlerts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Service.Security;
using Utils;
using Web.Models.FoundByFunction;
using Nois.Web.Framework.Kendoui;
using Service.FoundByFunctions;
using Entities.Domain.FoundByFunction;
using Entities.Domain.Users;

namespace Web.Controllers
{
    public class FoundByFunctionController : BaseController
    {
        private readonly IFoundByFunctionService _foundByFunctionService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public FoundByFunctionController(IFoundByFunctionService foundByFunctionService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _foundByFunctionService = foundByFunctionService;
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

            var foundByFunctions = await _foundByFunctionService.GetAllAsync(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = foundByFunctions.Select(x => new FoundByFunctionModel
                {
                    Id = x.Id,
                    Name = x.Name
                }),
                Total = foundByFunctions.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FoundByFunctionModel foundByFunctionModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            var existedFoundByFunction = await _foundByFunctionService.GetFoundByFunctionByName(foundByFunctionModel.Name);
            if (existedFoundByFunction != null)
                return Content("FoundByFunction Name has Existed!");

            var foundByFunction = new FoundByFunction(){Name = foundByFunctionModel.Name};
            await _foundByFunctionService.InsertAsync(foundByFunction);
            return Json(new
            {
                status = "success",
            });
        }

        [HttpPost]
        public async Task<ActionResult> Update(FoundByFunction model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            var foundByFunction = await _foundByFunctionService.GetByIdAsync(model.Id);
            if (foundByFunction == null)
                throw new ArgumentException("No foundByFunction found with the specified id");

            var existedFoundByFunction = await _foundByFunctionService.GetFoundByFunctionByName(model.Name);
            if (existedFoundByFunction != null && existedFoundByFunction.Id != foundByFunction.Id)
                return Content("FoundByFunction Name has Existed!");

            foundByFunction.Name = model.Name;
            await _foundByFunctionService.UpdateAsync(foundByFunction);
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
            var foundByFunction = await _foundByFunctionService.GetByIdAsync(id);
            if (foundByFunction == null)
                throw new ArgumentException("No foundByFunction found with the specified id");
            await _foundByFunctionService.DeleteAsync(foundByFunction);
            return Json(new
            {
                status = "success",
            });
        }

        //get list for dropdown control
        [HttpPost]
        public async Task<JsonResult> GetAllFoundByFunction(DataSourceRequest command)
        {
            var data = await _foundByFunctionService.GetListFoundByFunctions();
            return Json(data.Select(x => new FoundByFunction { Id = x.Id, Name = x.Name.Trim() }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }
    }
}