using Entities.Domain.ScMeasures;
using Entities.Domain.ScoreCards;
using Nois.Web.Framework.Kendoui;
using OfficeOpenXml;
using Service.ScMeasures;
using Service.ScoreCards;
using Service.Security;
using Service.Suppliers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utils;
using Web.Models.ScoreCard;

namespace Web.Controllers
{
    public class ScoreCardController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IScoreCardService _scoreCardService;
        private readonly IMqsMeasureService _mqsMeasureService;
        private readonly ISupplierService _supplierService;


        public ScoreCardController(IPermissionService permissionService,
            IWorkContext workContext,
            IScoreCardService scoreCardService,
            IMqsMeasureService mqsMeasureService,
            ISupplierService supplierService)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _scoreCardService = scoreCardService;
            _mqsMeasureService = mqsMeasureService;
            _supplierService = supplierService;
        }

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();

            return View("List");
        }
        /*
        [HttpPost]
        public ActionResult List(DataSourceRequest command, SupplierFilterModel supplierFilterModel, bool firstTime)
        {
            if (!_permissionService.Authorize(PermissionProvider.QualityAlert))
                return AccessDeniedView();
            if(firstTime)
            {
                supplierFilterModel.Year = DateTime.Now.Year;
                var supplier = _supplierService.GetAllAsync().Result.FirstOrDefault();
                if (supplier != null)
                    supplierFilterModel.SupplierId = supplier.Id;
                else
                    supplierFilterModel.SupplierId = 0;
            }
            else
            {
                if (supplierFilterModel.Year == 0 || supplierFilterModel.SupplierId == 0)
                {
                    var grid = new DataSourceResult
                    {
                        Data = new List<ScoreCardModel>() { new ScoreCardModel
                    {
                        MsqMeasure = "",
                        Ytd = "",
                        Jul = "",
                        Aug = "",
                        Sep = "",
                        Oct = "",
                        Nov = "",
                        Dec = "",
                        Jan = "",
                        Feb = "",
                        Mar = "",
                        Apr = "",
                        May = "",
                        Jun = ""
                    } },
                        Total = 0
                    };
                    return Json(grid);
                }
            }
           
                
            var scoreCardData = _scoreCardService.GetScoreCardData(supplierFilterModel.Year, supplierFilterModel.SupplierId);

            var gridModel = new DataSourceResult
            {
                Data = scoreCardData.Select(sc => new ScoreCardModel
                {
                    MsqMeasure = sc.MsqMeasure,
                    Ytd = sc.Ytd,
                    Jul = sc.Data[0].ToString(),
                    Aug = sc.Data[1].ToString(),
                    Sep = sc.Data[2].ToString(),
                    Oct = sc.Data[3].ToString(),
                    Nov = sc.Data[4].ToString(),
                    Dec = sc.Data[5].ToString(),
                    Jan = sc.Data[6].ToString(),
                    Feb = sc.Data[7].ToString(),
                    Mar = sc.Data[8].ToString(),
                    Apr = sc.Data[9].ToString(),
                    May = sc.Data[10].ToString(),
                    Jun = sc.Data[11].ToString(),
                }),
                Total = scoreCardData.Count()
            };

            // Return the result as JSON
            return Json(gridModel);
        }
        */

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SupplierFilterModel supplierFilterModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();
            if (supplierFilterModel.SupplierId < 1)
            {
                supplierFilterModel.Year = DateTime.Now.Month > 6 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
                var supplier = _supplierService.GetAllAsync().Result.FirstOrDefault();
                if (supplier != null)
                    supplierFilterModel.SupplierId = supplier.Id;
                else
                    supplierFilterModel.SupplierId = 0;
            }
            else
            {
                if (supplierFilterModel.Year == 0 || supplierFilterModel.SupplierId == 0)
                {
                    var grid = new DataSourceResult
                    {
                        Data = new List<ScoreCardModel>() { new ScoreCardModel
                    {
                        MsqMeasure = "",
                        Ytd = "",
                        Jul = "",
                        Aug = "",
                        Sep = "",
                        Oct = "",
                        Nov = "",
                        Dec = "",
                        Jan = "",
                        Feb = "",
                        Mar = "",
                        Apr = "",
                        May = "",
                        Jun = ""
                    } },
                        Total = 0
                    };
                    return Json(grid);
                }
            }

            DateTime fromDate;
            DateTime.TryParse(supplierFilterModel.FromDate, out fromDate);
            DateTime toDate;
            DateTime.TryParse(supplierFilterModel.ToDate, out toDate);

            var scoreCardData = _scoreCardService.GetScoreCardData(supplierFilterModel.Year, supplierFilterModel.SupplierId, fromDate, toDate);


            var gridModel = new DataSourceResult
            {
                Data = scoreCardData.Select(sc => new ScoreCardModel
                {
                    Id = 1,
                    MsqMeasure = sc.MsqMeasure,
                    ScMeasureId = sc.ScMeasureId,
                    Ytd = String.Format(sc.MsqMeasure.Contains("%") ? "{0}%" : "{0}", sc.Ytd?.ToString() ?? "0"),
                    Jul = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[0] != null) ? "{0}%" : "{0}", sc.Data[0]?.ToString("0.##") ?? ""),
                    Aug = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[1] != null) ? "{0}%" : "{0}", sc.Data[1]?.ToString("0.##") ?? ""),
                    Sep = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[2] != null) ? "{0}%" : "{0}", sc.Data[2]?.ToString("0.##") ?? ""),
                    Oct = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[3] != null) ? "{0}%" : "{0}", sc.Data[3]?.ToString("0.##") ?? ""),
                    Nov = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[4] != null) ? "{0}%" : "{0}", sc.Data[4]?.ToString("0.##") ?? ""),
                    Dec = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[5] != null) ? "{0}%" : "{0}", sc.Data[5]?.ToString("0.##") ?? ""),
                    Jan = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[6] != null) ? "{0}%" : "{0}", sc.Data[6]?.ToString("0.##") ?? ""),
                    Feb = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[7] != null) ? "{0}%" : "{0}", sc.Data[7]?.ToString("0.##") ?? ""),
                    Mar = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[8] != null) ? "{0}%" : "{0}", sc.Data[8]?.ToString("0.##") ?? ""),
                    Apr = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[9] != null) ? "{0}%" : "{0}", sc.Data[9]?.ToString("0.##") ?? ""),
                    May = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[10] != null) ? "{0}%" : "{0}", sc.Data[10]?.ToString("0.##") ?? ""),
                    Jun = String.Format((sc.MsqMeasure.Contains("%") && sc.Data[11] != null) ? "{0}%" : "{0}", sc.Data[11]?.ToString("0.##") ?? ""),
                    AvailableEdit = sc.AvailableEdit,
                    IsBold = sc.IsBold
                }),
                Total = scoreCardData.Count()
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(int year, string month, int scMeasureId, int supplierId, string value)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();

            //convert month string to month int
            int monthInt = DateTime.Parse("1." + month + " 2016").Month;
            //time start from july last year to june current year 
            //if month int >= 7 --> year = year - 1
            year = year - monthInt / 7;

            try
            {
                MqsMeasure mqsMeasure = _mqsMeasureService.GetMqsMeasureBySupplierIdAndScMeasureIdAndTime(supplierId, scMeasureId, new DateTime(year, monthInt, 1)).Result;
                if (mqsMeasure != null)
                {
                    mqsMeasure.Value = value;
                    await _mqsMeasureService.UpdateAsync(mqsMeasure);
                }               
                else
                {
                    mqsMeasure = new MqsMeasure();
                    mqsMeasure.SupplierId = supplierId;
                    mqsMeasure.ScMeasureId = scMeasureId;
                    mqsMeasure.Date = new DateTime(year, monthInt, 1);
                    mqsMeasure.Value = value;
                    await _mqsMeasureService.InsertAsync(mqsMeasure);
                }              
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult ImportMqsMeasure(HttpPostedFileBase file)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();

            if (Request.Files["mqsMeasureFile"].ContentLength <= 0)
                return Content("File is emply.");          

            var fileStream = Request.Files["mqsMeasureFile"].InputStream;

            using (var stream = new MemoryStream())
            {
                fileStream.CopyTo(stream);
                var message = "";
                _mqsMeasureService.ImportMqsMeasure(stream, out message);

                if (!string.IsNullOrEmpty(message))
                    return Content(message);
            }

            return Content("Success");
        }

        [HttpPost]
        public ActionResult ImportScoreCardMeasure()
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();

            var fileStream = Request.Files["scoreCardMeasureFile"].InputStream;

            if (Request.Files["scoreCardMeasureFile"].ContentLength <= 0)
                return Content("File is emply.");

            using (var stream = new MemoryStream())
            {
                fileStream.CopyTo(stream);
                var message = "";
                _mqsMeasureService.ImportScoreCardMeasure(stream, out message);

                if (!string.IsNullOrEmpty(message))
                    return Content(message);
            }

            return Content("Success");
        }

        public ActionResult ExportScoreCardMeasureToExcel(int year)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();

            #region export excell

            var path = "";
            try
            {

                var filename = "ScoreCardMeasure.xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _scoreCardService.ExportScoreCardTemplate(stream, year, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("Index");
            }

            #endregion
        }

        public ActionResult ExportMqsMeasureToExcel()
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();
            #region export excell

            var path = "";
            try
            {

                var filename = "MqsMeasure.xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _mqsMeasureService.ExportMqsTemplate(stream, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("Index");
            }

            #endregion
        }

        //export multiScoreCard to excel
        public ActionResult ExportMultiScoreCardMeasureToExcel(int year, string supplierIds, string monthIds)
        {
            if (!_permissionService.Authorize(PermissionProvider.ScoreCard))
                return AccessDeniedView();
            #region export excell

            List<int> listSupplierId = null;
            List<int> listMonthId = null;
            if (!String.IsNullOrEmpty(supplierIds))
                listSupplierId = supplierIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            if (!String.IsNullOrEmpty(monthIds))
                listMonthId = monthIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

            List<ScoreCardsPerYear> listScoreCardPerYear;
            listScoreCardPerYear = GetDataToExport(year, listSupplierId, listMonthId);

            var path = "";
            try
            {

                var filename = "MultiScoreCard.xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _scoreCardService.ExportMultiScoreCardTemplate(stream, year, listScoreCardPerYear, listMonthId, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("Index");
            }

            #endregion
        }

        private List<ScoreCardsPerYear> GetDataToExport(int year, List<int> listSupplierId, List<int> listMonthId)
        {
            List<ScoreCardsPerYear> listScoreCardPerYear = new List<ScoreCardsPerYear>();
            foreach (var supplierId in listSupplierId)
            {
                ScoreCardsPerYear scoreCardPerYear = new ScoreCardsPerYear();
                scoreCardPerYear.SupplierName = _supplierService.GetByIdAsync(supplierId).Result.Name;

                var scoreCardData = _scoreCardService.GetScoreCardData(year, supplierId);
                if(scoreCardData != null)
                {
                    scoreCardPerYear.ScoreCards = scoreCardData;
                    listScoreCardPerYear.Add(scoreCardPerYear);
                }              
            }
            return listScoreCardPerYear;
        }    
    }
}