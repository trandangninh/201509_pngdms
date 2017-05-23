using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Lines;
using Web.Extend;
using Web.Models.Line;
using Web.Models.Common;
using Nois.Web.Framework.Kendoui;
using Utils;
using Entities.Domain.Users;

namespace Web.Controllers
{

    //[Authorize(Roles = "Admin")]
    public class LineController : Controller
    {
        private readonly ILineService _lineService;
        private readonly IWorkContext _workContext;

        public LineController(ILineService lineService, IWorkContext workContext)
        {
            _lineService = lineService;
            _workContext = workContext;
        }

        public ActionResult Index()
        {
            return View();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, int? departmentId)
        {
            if (departmentId.HasValue)
            {

                var allLine = await _lineService.SearchLines(departmentId: departmentId.Value, pageIndex: command.Page - 1, pageSize: command.PageSize);

                var allLineModel = allLine
                    .Select(p => new LineModel
                    {
                        Id = p.Id,
                        LineDesc = p.LineDesc,
                        LineName = p.LineName,
                        LineCode = p.LineCode,
                        Note = p.Note,
                        Order = p.Index,
                        Department = new DepartmentViewModel { Id = p.Department.Id, Name = p.Department.Name },
                        ListUsername = p.Users.Select(u => u.Username).ToList(),
                        Active = p.Active
                    });

                var gridModel = new DataSourceResult
                {
                    Data = allLineModel,
                    Total = allLine.TotalCount
                };

                return Json(gridModel);
            }

            return Json(new { status = "departmentId is null" });
        }

        [HttpPost]
        public async Task<ActionResult> Create(LineModel model, int departmentId)
        {
            var existLine = await _lineService.GetLineByLineCodeAndDepartmentId(model.LineCode, departmentId);
            if (existLine != null)
                return Content("Line Code has existed in this department");

            //if(model.Department.Id < 1)
            //    return Content("Department is required");

            var line = new Line
            {
                LineCode = model.LineCode,
                LineDesc = model.LineDesc,
                LineName = model.LineName,
                Note = model.Note,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                DepartmentId = departmentId,
                Index = model.Order,
                Active = model.Active
            };
            await _lineService.InsertAsync(line);
            await _lineService.UpdateLineOwner(line, model.ListUsername);
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> Update(LineModel model)
        {
            var line = await _lineService.GetByIdAsync(model.Id);
            if (line == null)
                throw new ArgumentException("No line found with the specified id");

            //if (model.Department.Id < 1)
            //    return Content("Department is required");            

            if (line.LineCode != model.LineCode)
            {
                var existedLine = await _lineService.GetLineByLineCodeAndDepartmentId(model.LineCode, model.Department.Id);
                if (existedLine == null)
                    line.LineCode = model.LineCode;
                else
                    return Content("Line Code has Existed in this department");
            }

            line.LineName = model.LineName;
            line.Note = model.Note;
            line.UpdatedDate = DateTime.Now;
            line.Index = model.Order;
            line.Active = model.Active;

            await _lineService.UpdateLineOwner(line, model.ListUsername);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var line = await _lineService.GetByIdAsync(id);

            await _lineService.DeleteAsync(line);
            return new NullJsonResult();
        }

        //get all line for Quality Alert
        [HttpPost]
        public async Task<JsonResult> GetAllLine(DataSourceRequest command)
        {
            var userId = 0;
            if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin())
                userId = _workContext.CurrentUser.Id;


            var lines = await _lineService.SearchLines(userId: userId, active: true);
            var data = lines.Select(x => new
            {
                Id = x.Id,
                LineName = x.LineName,
                DepartmentId = x.DepartmentId
            });
            return Json(data.OrderBy(x => x.Id), JsonRequestBehavior.AllowGet);
        }
    }
}