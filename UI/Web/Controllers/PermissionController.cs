using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Service.Security;
using Service.Users;
using Web.Models.Permission;

namespace Web.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        public PermissionController(IPermissionService permissionService, IUserService userService)
        {
            _permissionService = permissionService;
            _userService = userService;
        }
        //
        // GET: /Permission/
        public async Task<ActionResult> Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.Management))
                return AccessDeniedView();

            var model = new PermissionMappingModel();
            var permissionRecords = await _permissionService.GetAllAsync();
            var roles = await _userService.GetAllUserRolesAsync();
            
            foreach (var pr in permissionRecords.OrderBy(p=>p.Order))
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Category = pr.Category,
                    Name = pr.Name,
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in roles)
            {
                model.AvailableRoles.Add(new RoleModel
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }

            foreach (var pr in permissionRecords)
                foreach (var cr in roles)
                {
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = pr.UserRoles.Any(r => r.Id == cr.Id);
                }

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        public async Task<ActionResult> PermissionsSave(FormCollection form)
        {
            if (!_permissionService.Authorize(PermissionProvider.Management))
                return AccessDeniedView();

            var permissionRecords = await _permissionService.GetAllAsync();
            var roles = await _userService.GetAllUserRolesAsync();

            foreach (var cr in roles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null
                    ? form[formKey].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                foreach (var pr in permissionRecords)
                {
                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        if (pr.UserRoles.FirstOrDefault(x => x.Id == cr.Id) == null)
                        {
                            pr.UserRoles.Add(cr);
                            await _permissionService.UpdateAsync(pr);
                        }
                    }
                    else
                    {
                        var ur = pr.UserRoles.FirstOrDefault(x => x.Id == cr.Id);
                        if ( ur != null)
                        {
                            pr.UserRoles.Remove(ur);
                            await _permissionService.UpdateAsync(pr);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
	}
}