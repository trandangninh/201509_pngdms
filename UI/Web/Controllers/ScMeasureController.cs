using Entities.Domain.ScMeasures;
using Nois.Web.Framework.Kendoui;
using Service.ScMeasures;
using Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils;
using Web.Models.ScMeasure;

namespace Web.Controllers
{
    public class ScMeasureController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IScMeasureService _scMeasureService;


        public ScMeasureController(
            IPermissionService permissionService,
            IWorkContext workContext,
            IScMeasureService scMeasureService
            )
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _scMeasureService = scMeasureService;
        }

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCardMeasureManagement))
                return AccessDeniedView();

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SearchSCMeasureModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCardMeasureManagement))
                return AccessDeniedView();

            var scMeasures = _scMeasureService.GetAllScMeasureAsync(searchBySCMeasureName: model.SCMeasureName, searchBySCMeasureCode: model.SCMeasureCode, pageIndex: command.Page - 1, pageSize: command.PageSize).Result;

            var gridModel = new DataSourceResult
            {
                Data = scMeasures.Select(sm => new ScMeasureModel
                {
                    Id = sm.Id,
                    Name = sm.Name,
                    Note = sm.Note,
                    DisplayOrder = sm.DisplayOrder,
                    Code = sm.Code,
                    IsDisplay = sm.IsDisplay,
                    IsImported = sm.IsImported
                }),
                Total = scMeasures.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ScMeasureModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCardMeasureManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (_scMeasureService.CheckCodeHasExisted(model.Code).Result)
                    return Content("Scorecard Measure Code has existed.");

                var scMeasure = new ScMeasure()
                {
                    Name = model.Name,
                    Note = model.Note,
                    DisplayOrder = model.DisplayOrder,
                    Code = model.Code,
                    IsDisplay = model.IsDisplay,
                    IsImported = model.IsImported,
                    Formula = "ManualEdit"
                };

                try
                {
                    await _scMeasureService.InsertAsync(scMeasure);
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
        public async Task<ActionResult> Update(ScMeasureModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCardMeasureManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Content("Identity of Scorecard Measure is invalid.");

                var scMeasure = await _scMeasureService.GetByIdAsync(model.Id);
                if (scMeasure == null)
                    return Content("Can not found Scorecard Measure.");

                var exiestedScMeasure = await _scMeasureService.GetScMeasureByCodeAsync(model.Code);
                if (exiestedScMeasure != null && exiestedScMeasure.Id != scMeasure.Id)
                    return Content("Scorecard Measure Code has existed.");

                scMeasure.Name = model.Name;
                scMeasure.Note = model.Note;
                scMeasure.DisplayOrder = model.DisplayOrder;
                scMeasure.Code = model.Code;
                scMeasure.IsDisplay = model.IsDisplay;
                scMeasure.IsImported = model.IsImported;

                try
                {
                    await _scMeasureService.UpdateAsync(scMeasure);
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
        public async Task<ActionResult> DeleteScMeasures(List<int> listId)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCardMeasureManagement))
                return AccessDeniedView();

            try
            {
                await _scMeasureService.DeleteScMeasuresAsync(listId);
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
    }
}
