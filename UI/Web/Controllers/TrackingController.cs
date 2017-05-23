using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Entities.Domain.Users;
using Service.Common;
using Service.Interface;
using Service.Users;
using Web.Extend;
using Web.Models.Packing;
using Utils;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ITrackingService _trackingService;
        private readonly IUserService _userService;
        private readonly IExcellService _excellService;
        private readonly IWorkContext _workContext;

        public TrackingController(
            ITrackingService trackingService, 
            IUserService userService, 
            IExcellService excellService,
            IWorkContext workContext
            )
        {
            _trackingService = trackingService;
            _userService = userService;
            _excellService = excellService;
            _workContext = workContext;
        }

        public ActionResult Manage()
        {
            if (_workContext.CurrentUser != null && _workContext.CurrentUser.IsAdmin())
            {
                return View("ManageTrackingAdmin");
            }
            return View("ManageTrackingEmployee");
        }


        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, SearchTrackingModel model)
        {
            
            // Works
            DateTime dateSearch = DateTime.Now;
            if (model.Datetime != null)
            {
                dateSearch = DateTime.Parse(model.Datetime, CultureInfo.InvariantCulture);
            }
            var Trackings = await _trackingService.GetTrackingByDateAndLine(model.LineCode, dateSearch);
            if (String.IsNullOrEmpty(model.SearchKeyword))
                Trackings = Trackings.OrderByDescending(p => p.UpdatedDate).ToList();

            var totalQuantity = Trackings.Sum(t => t.Quantity);

            var listTrackingModel = Trackings.Select(p => new TrackingModel()
                                                          {
                                                              Id = p.Id,
                                                              UpdatedDate = p.UpdatedDate.ToShortDateString(),
                                                              UserCreated = p.UserCreated,
                                                              LineCode = p.LineCode,
                                                              FGCode = p.FGCode,
                                                              Variant = p.Variant,
                                                              Cause = p.Cause,
                                                              CreatedDate = p.CreatedDate.ToShortDateString(),
                                                              Lot = p.Lot,
                                                              Quantity = p.Quantity,
                                                              Size = p.Size,
                                                              Where =p.Where,
                                                              TotalQuantity = totalQuantity
                                                          }).AsQueryable();

            // Calculate the total number of records before paging
            var total = listTrackingModel.Count();


            // Apply paging
            if (command.Page > 0)
            {
                listTrackingModel = listTrackingModel.Skip((command.Page - 1)*command.PageSize);
            }
            listTrackingModel = listTrackingModel.Take(command.PageSize);

            var result = new DataSourceResult()
                         {
                             Data = listTrackingModel.AsEnumerable(), // Process data (paging and sorting applied)
                             Total = total // Total number of records
                         };

            // Return the result as JSON
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> ListManager(DataSourceRequest command, SearchTrackingModel model)
        {

            DateTime dateSearchStart;

            if (String.IsNullOrEmpty(model.FromDate)) dateSearchStart = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(model.FromDate, culture, styles, out dateSearchStart))
                {
                    dateSearchStart = DateTime.Now;
                }
            }

            DateTime dateSearchEnd;


            if (String.IsNullOrEmpty(model.ToDate)) dateSearchEnd = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(model.ToDate, culture, styles, out dateSearchEnd))
                {
                    dateSearchEnd = DateTime.Now;
                }
            }

            var Trackings = await _trackingService.GetTrackingByTwoDateAndLine(model.LineCode, dateSearchStart, dateSearchEnd);
            if (String.IsNullOrEmpty(model.SearchKeyword))
                Trackings = Trackings.OrderByDescending(p => p.UpdatedDate).ToList();


            var totalQuantity = Trackings.Sum(t => t.Quantity);


            var listTrackingModel = Trackings.Select(p => new TrackingModel()
            {
                Id = p.Id,
                UpdatedDate = p.UpdatedDate.ToShortDateString(),
                UserCreated = p.UserCreated,
                LineCode = p.LineCode,
                FGCode = p.FGCode,
                Variant = p.Variant,
                Cause = p.Cause,
                CreatedDate = p.CreatedDate.ToShortDateString(),
                Lot = p.Lot,
                Quantity = p.Quantity,
                Size = p.Size,
                Where = p.Where,
                TotalQuantity = totalQuantity

            }).AsQueryable();

            // Calculate the total number of records before paging
            var total = listTrackingModel.Count();


            // Apply paging
            if (command.Page > 0)
            {
                listTrackingModel = listTrackingModel.Skip((command.Page - 1) * command.PageSize);
            }
            listTrackingModel = listTrackingModel.Take(command.PageSize);

            var result = new DataSourceResult()
            {
                Data = listTrackingModel.AsEnumerable(), // Process data (paging and sorting applied)
                Total = total // Total number of records
            };

            // Return the result as JSON
            return Json(result);
        }


        [HttpPost]
        public async Task<ActionResult> Create(TrackingModel model)
        {

            var actualDay = DateTime.Now;
            //var currentUser = await _userService.GetUserByUsernameAsync("");
            //Khang comment var currentUser = await _userService.GetUserByUsernameAsync(User.Identity.GetUsername());
            if(_workContext.CurrentUser == null)
                return Content("User is Invalid");
            var currentUser = _workContext.CurrentUser;
            var newIssue = new Tracking()
            {
              
                CreatedDate = actualDay,
                UpdatedDate = actualDay,
                UserCreated = currentUser.Id,
                LineCode = model.LineCode,
                FGCode = model.FGCode,
                Variant = model.Variant,
                Cause = model.Cause,
                Lot = model.Lot,
                Quantity = model.Quantity,
                Size = model.Size,
                Where = model.Where

            };
            await _trackingService.CreateAsync(newIssue);
            return new NullJsonResult();
        }

        public async Task<ActionResult> Update(TrackingModel model)
        {
           
            var actualDay = DateTime.Now;
            //var currentUser = await _userService.GetUserByUsernameAsync("");
            //Khang comment var currentUser = await _userService.GetUserByUsernameAsync(User.Identity.GetUsername());

            if (_workContext.CurrentUser == null)
                return Content("User is Invalid");
            var currentUser = _workContext.CurrentUser;

            var tracking = await _trackingService.GetTrackingById(model.Id);
            if (tracking != null)
            {
                tracking.CreatedDate = actualDay;
                tracking.UpdatedDate = actualDay;
                //tracking.CreatedUser = currentUser;
                tracking.UserCreated = currentUser.Id;
                tracking.LineCode = model.LineCode;
                tracking.FGCode = model.FGCode;
                tracking.Variant = model.Variant;
                tracking.Cause = model.Cause;
                tracking.Lot = model.Lot;
                tracking.Quantity = model.Quantity;
                tracking.Size = model.Size;
                tracking.Where = model.Where;
                await _trackingService.UpdateAsync(tracking);
            }
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var tracking = await _trackingService.GetTrackingById(id);
            if (tracking == null)
                throw new ArgumentException("No line found with the specified id");
            await _trackingService.DeleteAsync(tracking);
            return new NullJsonResult();
        }


        public class SearchTrackingModel
        {
            public string Datetime { get; set; }

            public string FromDate { get; set; }
            public string ToDate { get; set; }

            public string SearchKeyword { get; set; }
            public string LineCode { get; set; }
            public string Status { get; set; }

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
                var table = _excellService.ReadExcellToDataTable(path, true);
                if (table != null)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {

                        var trackingModel = new Tracking()
                        {
                            CreatedDate = DateTime.Parse(table.Rows[i]["Create Date"].ToString(), CultureInfo.InvariantCulture),
                            UpdatedDate = DateTime.Parse(table.Rows[i]["Create Date"].ToString(), CultureInfo.InvariantCulture),
                            FGCode = table.Rows[i]["FGCode"].ToString(),
                            LineCode = table.Rows[i]["LineCode"].ToString(),
                            Quantity = int.Parse(table.Rows[i]["Quantity"].ToString()),
                            Variant = table.Rows[i]["Variant"].ToString(),
                            Where = table.Rows[i]["Where"].ToString(),
                            Lot = table.Rows[i]["Lot"].ToString(),
                            Size = table.Rows[i]["Size"].ToString(),
                            Cause = table.Rows[i]["Cause"].ToString()

                        };
                        await _trackingService.CreateAsync(trackingModel);

                    }
                }
                return Json(new { status = "success", type = "create" });
            }

            return Json(new { status = "error", type = "create" });

        }
    }
}