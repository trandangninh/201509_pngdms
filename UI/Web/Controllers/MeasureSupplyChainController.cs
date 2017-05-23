using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Departments;
using Service.Interface;
using Service.Users;
using Web.Extend;
using Web.Models.MeasureSupplyChain;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class MeasureSupplyChainController : Controller
    {
        private readonly IMeasureSupplyChainService _measureSupplyChainService ;
        private readonly IUserService _userService;
        public MeasureSupplyChainController(IMeasureSupplyChainService measureSupplyChainService, 
            IUserService userService)
        {
            _measureSupplyChainService = measureSupplyChainService;
            _userService = userService;
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
        public async Task<ActionResult> List(DataSourceRequest command)
        {
            var allMeasureSupplyChain = await _measureSupplyChainService.GetAllMeasureSupplyChains();
            var total = allMeasureSupplyChain.Count;
            if (command.Page > 0)
                allMeasureSupplyChain = allMeasureSupplyChain.Skip(command.PageSize * (command.Page - 1)).Take(command.PageSize).ToList();
            var allMeasureSupplyChainModel = allMeasureSupplyChain.OrderByDescending(p => p.UpdatedDate).ToList().Select(
                p => new MeasureSupplyChainModel
                {
                    Id = p.Id,
                    MeasureSupplyChainName = p.MeasureSupplyChainName,
                    DmsCode = p.DmsCode,
                    MeasureSupplyChainCode = p.MeasureSupplyChainCode,
                    UpdatedDate = p.UpdatedDate

                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = allMeasureSupplyChainModel,
                Total = total
            };

            return Json(gridModel);

        }
        [HttpPost]
        public async Task<ActionResult> Create(MeasureSupplyChainModel model)
        {
          
                var checkListMeasureSupplyChain = await _measureSupplyChainService.GetAllMeasureSupplyChains();
                var checkMeasureSupplyChain =
                    checkListMeasureSupplyChain.FirstOrDefault(p => p.DmsCode == model.DmsCode && p.MeasureSupplyChainName == model.MeasureSupplyChainName);
                if (checkMeasureSupplyChain != null)
                {
                    return Json(new { status = "we have MeasureSupplyChain with DMS and name like this before" });
                }

                var MeasureSupplyChain = new MeasureSupplyChain();
                MeasureSupplyChain.MeasureSupplyChainName = model.MeasureSupplyChainName;
                MeasureSupplyChain.MeasureSupplyChainCode = model.MeasureSupplyChainCode;
                MeasureSupplyChain.CreatedDate = DateTime.Now;
                MeasureSupplyChain.UpdatedDate = DateTime.Now;

                await _measureSupplyChainService.CreateAsync(MeasureSupplyChain);
                
                return Json(new { status = "success" });
            
        }


        [HttpPost]
        public async Task<ActionResult> Update(MeasureSupplyChainModel model)
        {
              var MeasureSupplyChain = await _measureSupplyChainService.GetMeasureSupplyChainById(model.Id);
                if (MeasureSupplyChain == null)
                    return Json(new { status = "No MeasureSupplyChain found with the specified id" });
                else
                {
                    MeasureSupplyChain.MeasureSupplyChainName = model.MeasureSupplyChainName;
                    MeasureSupplyChain.MeasureSupplyChainCode = model.MeasureSupplyChainCode;
                    MeasureSupplyChain.UpdatedDate = DateTime.Now;
                    await _measureSupplyChainService.UpdateAsync(MeasureSupplyChain);
                    return Json(new { status = "success" });
                }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {

            var MeasureSupplyChain = await _measureSupplyChainService.GetMeasureSupplyChainById(id);
            if (MeasureSupplyChain == null)
                throw new ArgumentException("No MeasureSupplyChain found with the specified id");
            await _measureSupplyChainService.DeleteAsync(MeasureSupplyChain);
            return new NullJsonResult();
        }
	}
}