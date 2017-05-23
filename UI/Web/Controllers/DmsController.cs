using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Departments;
using Service.Interface;
using Service.Users;
using Web.Models.Dms;
using Web.Models.Common;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{

    //Khang comment [Authorize(Roles = "Admin")]
    public class DmsController : Controller
    {
        private readonly IDmsService _dmsService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        public DmsController(IDmsService dmsService, 
            IUserService userService,
            IDepartmentService departmentService
            )
        {
            _dmsService = dmsService;
            _userService = userService;
            _departmentService = departmentService;
        }

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllUserAvailable()
        {

            var listUsername = _userService.GetAllUsersAsync(active: true).Select(p => p.Username).ToList();
            var allUserModel = listUsername.ToList().Select(
                p => new UserAvailableModel()
                {
                    id = p,
                    text = p

                }).ToList();

            return Json(allUserModel, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, int? departmentId)
        {
            var departmentDefault = _departmentService.SearchDepartment().FirstOrDefault();

            var allDms = await _dmsService.GetDmsByDepartmentId(departmentId == null ? departmentDefault.Id : departmentId.Value, command.Page - 1, command.PageSize);
            
            var allDmsModel = allDms.Select(
               d => new DmsModel
               {
                   Id = d.Id,
                   DmsCode = d.DmsCode,
                   Description = d.Description,
                   Note = d.Note,
                   Department = new DepartmentViewModel() { Id = d.DepartmentId, Name = d.Department.Name },
                   ListUsername = d.Users.Select(u => u.Username).ToList(),
                   Active = d.Active,
                   Order = d.Order
               }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = allDmsModel,
                Total = allDms.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(DmsModel model, int departmentId)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.DmsCode))
                    return Content("DmsCode is required!");

                //var existDms = _dmsService.GetDmsByDmsCode(model.DmsCode);
                var existDms = await _dmsService.GetDmsByDmsCodeAndDepartmentId(model.DmsCode, departmentId);
                //check exist Dms
                if (existDms != null)
                    return Content(model.DmsCode + " has Existed in this Department!");

                //if (model.Department.Id < 1)
                //    return Content("Department is required!");

                var dms = new Dms()
                {
                    Description = model.Description,
                    Note = model.Note,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    DmsCode = model.DmsCode,
                    DepartmentId = departmentId,
                    Active = model.Active,
                    Order = model.Order
                };
                await _dmsService.InsertAsync(dms);
                await _dmsService.UpdateDmsOwner(dms, model.ListUsername);
                return Json(new { Data = new[] { model } });
            }
            return Content("Can not create Dms because model state is invalid");
        }


        [HttpPost]
        public async Task<ActionResult> Update(DmsModel model)
        {
            if (ModelState.IsValid)
            {
                //if (model.Department.Id < 1)
                //    return Content("Department is required!");

                var dms = await _dmsService.GetByIdAsync(model.Id);
                if (dms == null)
                    throw new ArgumentException("No dms found with the specified id");
                if (model.DmsCode != dms.DmsCode)
                {
                    var existedDms = await _dmsService.GetDmsByDmsCodeAndDepartmentId(model.DmsCode, model.Department.Id);
                    if (existedDms == null)
                        dms.DmsCode = model.DmsCode;
                    else
                        return Content("DmsCode has Existed in this Department");
                }

                dms.Description = model.Description;
                dms.Note = model.Note;
                dms.UpdatedDate = DateTime.Now;
                dms.Active = model.Active;
                dms.Order = model.Order;

                await _dmsService.UpdateDmsOwner(dms, model.ListUsername);
                return Json(new { status = "success" });
            }

            return Content("Can not edit Dms because model state is invalid"); ;
        }

        //[HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{

        //    var dms = await _dmsService.GetDmsById(id);
        //    if (dms == null)
        //        throw new ArgumentException("No dms found with the specified id");
        //    await _dmsService.DeletaAllUserOfDms(id);
        //    await _dmsService.DeleteAsync(dms);
        //    return new NullJsonResult();
        //}

        [HttpPost]
        public JsonResult GetAllDmsForView(DataSourceRequest command, int? departmentId)
        {
            var departmentDefault = _departmentService.SearchDepartment().FirstOrDefault();
            var departmentIdDefault = departmentId == null ? departmentDefault.Id : departmentId.Value;

            var Dmss = _dmsService.GetAllAsync().Result;
            var data = Dmss.Where(d => d.Active == true && d.DepartmentId == departmentIdDefault).OrderBy(d=>d.Order).Select(x => new
            {
                Id = x.Id,
                Code = x.DmsCode
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}