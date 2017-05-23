using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Entities.Domain.Users;
using Service.Common;
using Service.Users;
using Utils;
using Web.Extend;
using Web.Models.ShutdownRequest;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class ShutdownController : BaseController
    {
        private readonly IShutdownRequestService _shutdownRequestService ;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        public ShutdownController(IShutdownRequestService shutdownRequestService, 
            IUserService userService, 
            IWorkContext workContext)
        {
            _shutdownRequestService = shutdownRequestService;
            _userService = userService;
            _workContext = workContext;
        }

        //
        // GET: /Shutdown/
        public ActionResult Index(string month)
        {
            if(_workContext.CurrentUser == null)
                return View("GuestViewShutdown");
            if(_workContext.CurrentUser.IsAdmin())
                return View("AdminViewShutdown");
            return View("EmployeeViewShutdown");
        }
        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, SearchShutDownModel model)   
        {
            DateTime dateSearch;
            if (String.IsNullOrEmpty(model.Datetime)) dateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(model.Datetime, culture, styles, out dateSearch))
                {
                    dateSearch = DateTime.Now;
                }
            } 

            var year = dateSearch.Year;
            var monthCh = dateSearch.Month;
            var startDate = new DateTime(year, monthCh, 1, 0, 0, 0);
            var endDate = monthCh == 12 ? new DateTime(year + 1, 1, 1, 0, 0, 0) :  new DateTime(year, monthCh + 1, 1, 0, 0, 0);

            IPagedList<ShutdownRequest> shutDownOfThisUserInMonth;
            if(_workContext.CurrentUser!=null)
                shutDownOfThisUserInMonth = _shutdownRequestService.GetShutdownRequest(model.SearchKeyword ,_workContext.CurrentUser.IsAdmin() ? 0 : _workContext.CurrentUser.Id, model.StatusId, startDate, endDate, command.Page-1, command.PageSize);
            else
            {
                //shutDownOfThisUserInMonth = new PagedList<ShutdownRequest>(new List<ShutdownRequest>(),0,0);
                shutDownOfThisUserInMonth = _shutdownRequestService.GetShutdownRequest(model.SearchKeyword, 0, model.StatusId, startDate, endDate, command.Page - 1, command.PageSize);
            }

            var allShutDownModel = shutDownOfThisUserInMonth.Select(
                p => new ShutdownRequestModel
                {
                    Id = p.Id,
                    ShutdownId = p.Id.ToString(),
                    Content = p.ShutdownRequestContent,
                    CreatedDate = p.CreatedDate.ToShortDateString(),
                    CanEdit = p.CreatedDate.Date==DateTime.Now.Date,
                    UserCreated = p.CreatedUser.Username,
                    UserCreatedId = p.UserCreatedId,
                    MakingScope = p.MakingScope,
                    PackingScope = p.PackingScope,
                    BasePlanDate = p.BasePlanDate.ToString("yyyy-MM-dd"),
                    Comment = p.Comment,
                    Remark = p.Remark,
                    Status = p.ShutdownStatus.ToString(),
                    StatusId = (int)p.ShutdownStatus
                }).ToList();
           
            var gridModel = new DataSourceResult
            {
                Data = allShutDownModel,
                Total = shutDownOfThisUserInMonth.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> ListByAdmin(DataSourceRequest command, SearchShutDownByAdminModel model)
        {
            DateTime dateSearch;
            if (String.IsNullOrEmpty(model.Datetime)) dateSearch = DateTime.Now;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(model.Datetime, culture, styles, out dateSearch))
                {
                    dateSearch = DateTime.Now;
                }
            }
            
            int year = dateSearch.Year;
            int monthCh = dateSearch.Month;
            var startDate = new DateTime(year, monthCh, 1, 0, 0, 0);
            var endDate = monthCh == 12 ? new DateTime(year + 1, 1, 1, 0, 0, 0) : new DateTime(year, monthCh+1, 1, 0, 0, 0);

            var shutDownOfThisUserInMonth = _shutdownRequestService.GetShutdownRequest(model.SearchKeyword, model.UserId, model.StatusId, startDate, endDate);

            var allShutDownModel = shutDownOfThisUserInMonth.Select(
                p => new ShutdownRequestModel
                {
                    Id = p.Id,
                    ShutdownId = p.Id.ToString(),
                    Content = p.ShutdownRequestContent,
                    CreatedDate = p.CreatedDate.ToShortDateString(),
                    CanEdit = p.CreatedDate.Date == DateTime.Now.Date,
                    UserCreated = p.CreatedUser.Username,
                    UserCreatedId = p.UserCreatedId,
                    MakingScope = p.MakingScope,
                    PackingScope = p.PackingScope,
                    BasePlanDate = p.BasePlanDate.ToString("yyyy-MM-dd"),
                    Comment = p.Comment,
                    Remark = p.Remark,
                    Status = p.ShutdownStatus.ToString(),
                    StatusId = (int)p.ShutdownStatus
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = allShutDownModel,
                Total = shutDownOfThisUserInMonth.TotalCount
            };

            return Json(gridModel);

        }
        [HttpPost]
        public async Task<ActionResult> Create(ShutdownRequestModel model)
        {
            if (_workContext.CurrentUser == null)
                return AccessDeniedView();

            DateTime baseDate;
            var userCreate = await _userService.GetUserByIdAsync(model.UserCreatedId);
            if (model.BasePlanDate.Length > 10)
                baseDate = DateTime.ParseExact(model.BasePlanDate.Substring(0, 24),
                                   "ddd MMM d yyyy HH:mm:ss",
                                   CultureInfo.InvariantCulture);
            else
            {
                baseDate = Convert.ToDateTime(model.BasePlanDate);
            }
            
            var shutdownRequest = new ShutdownRequest
            {
                Comment = model.Comment,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                MakingScope = model.MakingScope,
                PackingScope = model.PackingScope,
                BasePlanDate = baseDate,
                ShutdownRequestContent = model.Content,
                Remark = model.Remark,
                CreatedUser = userCreate,
                UserCreatedId = userCreate.Id,
                ShutdownStatus = (ShutdownStatus)model.StatusId
            };
            await _shutdownRequestService.InsertAsync(shutdownRequest);
           
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> Update(ShutdownRequestModel model)
        {
            if (_workContext.CurrentUser == null)
                return AccessDeniedView();

            DateTime baseDate ;
            if (model.BasePlanDate.Length>10)
            baseDate = DateTime.ParseExact(model.BasePlanDate.Substring(0, 24),
                               "ddd MMM d yyyy HH:mm:ss",
                               CultureInfo.InvariantCulture);
            else
            {
                baseDate = Convert.ToDateTime(model.BasePlanDate);
            }


            var userUpdate = await _userService.GetUserByIdAsync(model.UserCreatedId);
            if (userUpdate != null)
            {
                var shutdownRequest = await _shutdownRequestService.GetByIdAsync(model.Id);
                if (shutdownRequest != null)
                {
                    shutdownRequest.UpdatedDate = DateTime.Now;
                    shutdownRequest.Comment = model.Comment;
                    shutdownRequest.MakingScope = model.MakingScope;
                    shutdownRequest.PackingScope = model.PackingScope;
                    shutdownRequest.Remark = model.Remark;
                    shutdownRequest.ShutdownRequestContent = model.Content;
                    shutdownRequest.BasePlanDate = baseDate;
                    shutdownRequest.ShutdownStatus = (ShutdownStatus)model.StatusId;
                    if (shutdownRequest.UserCreatedId != userUpdate.Id)
                    {
                        shutdownRequest.UserCreatedId = userUpdate.Id;
                        shutdownRequest.CreatedUser = userUpdate;
                    }

                    await _shutdownRequestService.UpdateAsync(shutdownRequest);
                }
            }
            
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (_workContext.CurrentUser == null)
                return AccessDeniedView();

            var shutdownRequest = await _shutdownRequestService.GetByIdAsync(id);
            if (shutdownRequest == null)
                throw new ArgumentException("No shutdownRequest found with the specified id");
            
            await _shutdownRequestService.DeleteAsync(shutdownRequest);
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAdmin(ShutdownRequestModel model)
        {
            if (_workContext.CurrentUser == null)
                return AccessDeniedView();

            DateTime baseDate;
            var userCreate = await _userService.GetUserByIdAsync(model.UserCreatedId);
            if (model.BasePlanDate.Length > 10)
                baseDate = DateTime.ParseExact(model.BasePlanDate.Substring(0, 24),
                    "ddd MMM d yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            else
            {
                baseDate = Convert.ToDateTime(model.BasePlanDate);
            }
            if (userCreate != null)
            {
                var shutdownRequest = new ShutdownRequest
                {
                    Comment = model.Comment,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    MakingScope = model.MakingScope,
                    PackingScope = model.PackingScope,
                    BasePlanDate = baseDate,
                    ShutdownRequestContent = model.Content,
                    Remark = model.Remark,
                    CreatedUser = userCreate,
                    UserCreatedId = userCreate.Id,
                    ShutdownStatus = (ShutdownStatus)model.StatusId
                };
                await _shutdownRequestService.InsertAsync(shutdownRequest);
            }
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAdmin(ShutdownRequestModel model)
        {
            if (_workContext.CurrentUser == null)
                return AccessDeniedView();

            DateTime baseDate;
            if (model.BasePlanDate.Length > 10)
                baseDate = DateTime.ParseExact(model.BasePlanDate.Substring(0, 24),
                                   "ddd MMM d yyyy HH:mm:ss",
                                   CultureInfo.InvariantCulture);
            else
            {
                baseDate = Convert.ToDateTime(model.BasePlanDate);
            }

            var userUpdate = await _userService.GetUserByIdAsync(model.UserCreatedId);
            if (userUpdate != null)
            {
                var shutdownRequest = await _shutdownRequestService.GetByIdAsync(model.Id);
                if (shutdownRequest != null)
                {
                    shutdownRequest.UpdatedDate = DateTime.Now;
                    shutdownRequest.Comment = model.Comment;
                    shutdownRequest.MakingScope = model.MakingScope;
                    shutdownRequest.PackingScope = model.PackingScope;
                    shutdownRequest.Remark = model.Remark;
                    shutdownRequest.ShutdownRequestContent = model.Content;
                    shutdownRequest.BasePlanDate = baseDate;
                    shutdownRequest.ShutdownStatus = (ShutdownStatus)model.StatusId;
                    if (shutdownRequest.UserCreatedId != userUpdate.Id)
                    {
                        shutdownRequest.UserCreatedId = userUpdate.Id;
                        shutdownRequest.CreatedUser = userUpdate;
                    }

                    await _shutdownRequestService.UpdateAsync(shutdownRequest);
                }
            }
            

            return new NullJsonResult();
        }

        [HttpPost]
        public JsonResult GetAllShutdownStatus()
        {
            var data = ShutdownStatus.Closed.EnumToList().Select(x => new
            {
                Id = x.Key,
                Name = x.Value
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}