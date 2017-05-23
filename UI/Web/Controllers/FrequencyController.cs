using Entities.Domain.ScMeasures;
using Nois.Web.Framework.Kendoui;
using Service.Frequencys;
using Service.ScMeasures;
using Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils;
using Web.Models.Frequency;
using Web.Models.ScMeasure;

namespace Web.Controllers
{
    public class FrequencyController : BaseController
    {
        private readonly IFrequencyService _frequencyService;

        public FrequencyController(
            IFrequencyService frequencyService
            )
        {
            _frequencyService = frequencyService;
        }

        public ActionResult Index()
        {
            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            var frequencys = _frequencyService.GetAllFrequencyAsync(pageIndex: command.Page, pageSize: command.PageSize).Result;

            var gridModel = new DataSourceResult
            {
                Data = frequencys.Select(sm => new FrequencyModel
                {
                    Id = sm.Id,
                    Name = sm.Name,
                    Mark = sm.Mark
                }),
                Total = frequencys.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

    }
}
