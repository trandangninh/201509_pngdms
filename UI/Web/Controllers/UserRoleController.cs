using Entities.Domain.Users;
using Nois.Web.Framework.Kendoui;
using Service.Security;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utils;
using Web.Models.UserManager;

namespace Web.Controllers
{
    public class UserRoleController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IUserRoleService _userRoleService;


        public UserRoleController(
            IPermissionService permissionService,
            IWorkContext workContext,
            IUserRoleService userRoleService
            )
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _userRoleService = userRoleService;
        }

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            var userRoles = _userRoleService.GetAllUserRolesAsync(pageIndex: command.Page - 1, pageSize: command.PageSize).Result;

            var gridModel = new DataSourceResult
            {
                Data = userRoles.Select(ur => new UserRoleModel
                {
                    Id = ur.Id,
                    Name = ur.Name,
                    IsSystem = ur.IsSystem,
                    IsActive = ur.IsActive
                }),
                Total = userRoles.TotalCount
            };
           
            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserRoleModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.IsSystem)
                    return Content("Can not create UserRole system.");

                if (_userRoleService.CheckRoleNameHasExisted(model.Name).Result)
                    return Content("UserRole Name has existed.");

                var userRole = new UserRole()
                {
                    Name = model.Name,
                    IsSystem = false,
                    IsActive = model.IsActive,
                    SystemName = "UserRole"
                };

                try
                {
                    await _userRoleService.InsertAsync(userRole);
                }
                catch(Exception ex)
                {
                    return Content(ex.Message);
                }
                
                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> Update(UserRoleModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if(model.Id <= 0)
                    return Content("Identity of User Role is invalid.");

                var userRole = await _userRoleService.GetByIdAsync(model.Id);
                if (userRole == null)
                    return Content("Can not found User Role.");

                var exiestedUserRole = await _userRoleService.GetUserRoleByNameAsync(model.Name);
                if (exiestedUserRole != null && exiestedUserRole.Id != userRole.Id)
                    return Content("UserRole Name has existed.");

                userRole.Name = model.Name;
                userRole.IsActive = model.IsActive;

                try
                {
                    await _userRoleService.UpdateAsync(userRole);
                }
                catch(Exception ex)
                {
                    return Content(ex.Message);
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> Delete(UserRoleModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Json(new { status = "false", Content = "Identity of User Role is invalid." });

                var userRole = await _userRoleService.GetByIdAsync(model.Id);
                if (userRole == null)
                    return Json(new { status = "false", Content = "Can not found User Role." });

                try
                {
                    await _userRoleService.DeleteAsync(userRole);
                }
                catch(Exception ex)
                {
                    return Json(new { status = "false", Content = ex.Message });
                }

                return Json(new { status = "success" });
            }

            return Json(new { Data = new[] { model } });
        }
    }
}