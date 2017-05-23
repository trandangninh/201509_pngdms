using System;
using System.Linq;
using System.Web.Mvc;
using Entities.Domain.Meetings;
using Service.Departments;
using Service.Interface;
using Service.Meetings;
using Service.Security;
using Service.Users;
using Utils;
using System.Threading.Tasks;
using Web.Attributes;
using Web.Models.Department;
using Entities.Domain.Departments;
using Nois.Web.Framework.Kendoui;
using Entities.Domain.Users;
using System.Collections.Generic;

namespace Web.Controllers
{
    //[AdminAuthorize]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMeetingService _meetingService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;

        public DepartmentController(IDepartmentService departmentService,
            IPermissionService permissionService, 
            IWorkContext workContext, IMeetingService meetingService, 
            IUserService userService)
        {
            _departmentService = departmentService;
            _permissionService = permissionService;
            _workContext = workContext;
            _meetingService = meetingService;
            _userService = userService;
        }


        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (! _permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command)
        {
            if (! _permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();

            var classifications = await _departmentService.GetAllAsync(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = classifications.Select(x => new DepartmentModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.Active,
                    Order = x.Order
                }),
                Total = classifications.TotalCount
            };

            return Json(gridModel);
        }


        //get all department for Meeting View
        [HttpPost]
        public JsonResult GetAllDepartment(DataSourceRequest command)
        {
            var departments = _departmentService.SearchDepartment(null, true);

            var udids = new List<int>();
            if(_workContext.CurrentUser!=null)
                udids = _workContext.CurrentUser.Departments.Select(d => d.Id).ToList();

            var deps = departments.Where(d => (_workContext.CurrentUser != null && udids.Any(id => id == d.Id)
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
        public JsonResult GetAllUser(DataSourceRequest command)
        {
            var users = _userService.GetAllAsync().Result;
            var data = users.Where(u => u.Active).Select(x => new Web.Models.Meeting.UserForMeetingViewModel
            {
                Id = x.Id,
                Name = x.Username
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Create(DepartmentModel departmentModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            //if (ModelState.IsValid)
            {
                var existedDepartment = await _departmentService.GetDepartmentByDepartmentName(departmentModel.Name);
                if (existedDepartment != null)
                    return Content("Department Name has Existed!");

                var department = new Department()
                {
                    Name = departmentModel.Name,
                    Active = true,
                    Order = departmentModel.Order

                };
                await _departmentService.InsertAsync(department);

                var meeting = new Meeting
                {
                    DepartmentId = department.Id,
                };
                await _meetingService.InsertAsync(meeting);

                return Json(department);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update(Department model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageDepartment))
                return AccessDeniedView();
            var existedDepartment = await _departmentService.GetDepartmentByDepartmentName(model.Name);
            var department = await _departmentService.GetByIdAsync(model.Id);

            if (department == null)
                throw new ArgumentException("No department found with the specified id");

            if (existedDepartment != null && existedDepartment.Id != department.Id)
                return Content("Department Name has Existed!");



            department.Name = model.Name;
            department.Active = model.Active;
            department.Order = model.Order;
            await _departmentService.UpdateAsync(department);
            return Json(new
            {
                status = "success",
            });
        }

        //[HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    //int count = _deparmentService.CountAllQualityAlertByClassificationIdAsync(id);
        //    //if (count > 0)
        //    //    return Content("This Classification can't delete because it has references to another Quality Alerts");
        //    var department = await _deparmentService.GetByIdAsync(id);
        //    if (department == null)
        //        throw new ArgumentException("No department found with the specified id");
        //    await _deparmentService.DeleteAsync(department);
        //    return Json(new
        //    {
        //        status = "success",
        //    });

        //}
	}
}