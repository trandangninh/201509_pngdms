using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Common;
using Service.Interface;
using Service.Security;
using Service.Users;
using Web.Models.ProductPlanning;
using System.IO;

namespace Web.Controllers
{
    public class ProductPlanningController : Controller
    {
        private readonly IProductPlanningService _productPlanningService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IXmlService _xmlService;
        private readonly IExcellService _excellService;

        public ProductPlanningController(IProductPlanningService productPlanningService, IUserService userService, IPermissionService permissionService, IXmlService xmlService, IExcellService excellService)
        {
            _productPlanningService = productPlanningService;
            _userService = userService;
            _permissionService = permissionService;
            _xmlService = xmlService;
            _excellService = excellService;
        }

        public async Task<ActionResult> Index(string date)
        {

            DateTime dateSearch;

            if (String.IsNullOrEmpty(date)) dateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(date, culture, styles, out dateSearch))
                {
                    dateSearch = DateTime.Now;
                }
            }

            var model = new List<ProductPlanningNewModel>();
            // if (await _permissionService.Authorize(PermissionProvider.ViewSupplyChain, User.Identity.Name))
            //{

            var listLineType = new List<PlanLineHardCodeType>()
                                   {
                                       PlanLineHardCodeType.Line1,
                                       PlanLineHardCodeType.Line2,
                                       PlanLineHardCodeType.Line3,
                                       PlanLineHardCodeType.Gabbana,
                                       PlanLineHardCodeType.FRMK3,
                                       PlanLineHardCodeType.FRMK4,
                                  
                                     
                                   };


            var listShiftType = new List<PlanShiftHardCodeType>()
                                    {
                                        PlanShiftHardCodeType.Shift1,
                                        PlanShiftHardCodeType.Shift2,
                                        PlanShiftHardCodeType.Shift3

                                    };

            for (int i = 0; i < 7; i++)
            {
                foreach (var shift in listShiftType)
                {
                    var submodel = new ProductPlanningNewModel()
                                   {
                                       DateTime = dateSearch.AddDays(i).ToShortDateString(),
                                       ShiftType = shift.ToString(),
                                   };

                    foreach (var line in listLineType)
                    {
                        {
                            var productPlanning = await
                                _productPlanningService.GetProductPlanningByDateAndShiftAndLine(
                                    dateSearch.AddDays(i),
                                    shift, line);

                            var productPlanningResult = productPlanning != null ? productPlanning.Result : "";
                            var productName = productPlanning != null ? productPlanning.ProductName : "#ffff";

                            var newProductLineResult = new ProductLineResult
                                                       {
                                                           LineResult = productPlanningResult,
                                                           LineType = line.ToString(),
                                                           ProductName = productName,
                                                           Color = GetColorByProductName(productName)
                                                       };
                            submodel.ListProductLineResult.Add(newProductLineResult);
                        }

                    }
                    model.Add(submodel);
                }

            }
            if (!String.IsNullOrEmpty(User.Identity.Name))
            {
                return View(model);
            }
            return View("ProductionPlanningForGuest", model);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateProductPlanningResult(DateTime dateTime, string shiftType, string lineCode, string result)
        {
            PlanLineHardCodeType lc;
            Enum.TryParse(lineCode, out lc);
            var line = lc;
            PlanShiftHardCodeType sf;
            Enum.TryParse(shiftType, out sf);
            var shift = sf;
            string productName = result.Split(':')[0];
            var Result = await _productPlanningService.GetProductPlanningByDateAndShiftAndLine(dateTime,
                 shift, line);
            if (Result == null)
            {
                //create new
                var newResult = new ProductPlanning()
                {
                    Result = result,
                    CreatedDate = dateTime,
                    UpdatedDate = dateTime,
                    Line = line,
                    Shift = shift,
                    ProductName = productName.Replace("productName", "")

                };
                await _productPlanningService.CreateAsync(newResult);
                return Json(new { status = "success", type = "create" });
            }
            else
            {
                //update
                Result.Result = result;
                Result.ProductName = productName;
                await _productPlanningService.UpdateAsync(Result);
                return Json(new { status = "success", type = "update" });
            }

        }

        private PlanLineHardCodeType GetLineType(string line)
        {
            PlanLineHardCodeType lineType;

            if (line == "LPD 1")
                line = "Line1";
            if (line == "LPD 2")
                line = "Line2";
            if (line == "LPD 3")
                line = "Line3";

            if (Enum.TryParse(line.Replace(" ", ""), out lineType))
                return lineType;

            return PlanLineHardCodeType.Line1;
        }

        [HttpPost]
        public async Task<ActionResult> ImportDataFromExcell(FormCollection form)
        {
            var file = Request.Files["postedFile"];
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);
                // TODO: need to define destination
                var path = Path.Combine(Server.MapPath("~/AttachmentFilesForFolder"), fileName);
                file.SaveAs(path);
                var table = _excellService.ReadExcellToDataTable(path, false);

                List<DateTime> dateList = new List<DateTime>();

                if (table != null)
                {
                    var createDates = table.Rows[0].ItemArray.Cast<string>().Where(i => !String.IsNullOrEmpty(i) && i != "Date").ToList();

                    for (int i = 0; i < createDates.Count(); i++)
                    {
                        DateTime createDate;

                        DateTime.TryParseExact(createDates[i], "d-MMM-yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                            out createDate);

                        for (int j = 2; j < table.Rows.Count; j++)
                        {
                            for (int k = (i * 3) + 1; k <= (i + 1) * 3; k++)
                            {
                                PlanShiftHardCodeType shift;

                                Enum.TryParse(table.Rows[1].ItemArray[k].ToString().Replace(" ", ""), out shift);

                                PlanLineHardCodeType line = GetLineType(table.Rows[j].ItemArray[0].ToString());

                                var Result = await _productPlanningService.GetProductPlanningByDateAndShiftAndLine(createDate,
                         shift, line);

                                if (Result == null)
                                {

                                    var item = new ProductPlanning()
                                               {
                                                   Result = table.Rows[j].ItemArray[k].ToString(),
                                                   CreatedDate = createDate,
                                                   UpdatedDate = DateTime.Now,
                                                   Line = line,
                                                   Shift = shift,
                                                   ProductName = table.Rows[j].ItemArray[k].ToString()
                                               };
                                    await _productPlanningService.CreateAsync(item);
                                }
                                else
                                {
                                    //update
                                    Result.Result = table.Rows[j].ItemArray[k].ToString();
                                    Result.ProductName = table.Rows[j].ItemArray[k].ToString();
                                    await _productPlanningService.UpdateAsync(Result);
                                }
                            }
                        }
                    }


                }



                // Read date
                /*    for (int j = 1; j < table.Rows[0].ItemArray.Length; j++)
                    {
                        if (string.IsNullOrEmpty(table.Rows[0].ItemArray[j].ToString()))
                        {
                            DateTime date;
                            DateTime.TryParseExact(table.Rows[0].ItemArray[j].ToString(), "dd-MMM-yy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                            dateList.Add(date);
                        }
                    }
                    var iDate = 0;
                    for (int j = 1; j < table.Rows[j].ItemArray.Length; j++)
                    {
                        
                        string line = table.Rows[j].ItemArray[0].ToString();
                        var iShift = 1;
                        while (iShift <= table.Rows[j].ItemArray.Length)
                        {
                             string productName = table.Rows[j].ItemArray[iShift].ToString();

                              var item = new ProductPlanning()
                                {
                                    Result = productName,
                                    CreatedDate = dateList[(iShift-1)/3],
                                    UpdatedDate = dateList[(iShift-1)/3],
                                    Line = GetLineType(line),
                                    Shift = PlanShiftHardCodeType.Shift1,
                                    ProductName = productName
                                };

                                await _productPlanningService.CreateAsync(item);
                                iShift++;
                         
                        }

                    }*/

                return Json(new { status = "success", type = "create" });
            }

            return Json(new { status = "error", type = "create" });

        }

        #region funtion

        public string GetColorByProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                return "#ffff";
            productName = productName.TrimEnd().TrimStart();
            var listProductionPlanningColor = _xmlService.GetAllProductionPlanningColors();
            foreach (var item in listProductionPlanningColor)
            {
                if (productName.Contains(item.ProductionName))
                {
                    return item.Color;
                }
            }
            return "#ffff";
        }
        #endregion

    }
}