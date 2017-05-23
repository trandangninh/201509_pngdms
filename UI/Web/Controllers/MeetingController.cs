using Entities.Domain.Meetings;
using Nois.Web.Framework.Kendoui;
using Service.Meetings;
using Service.Security;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils;
using Web.Models.Common;
using Web.Models.Meeting;

namespace Web.Controllers
{
    public class MeetingController : BaseController
    {
        private readonly IMeetingService _meetingService;
        private readonly IUserInMeetingService _userInMeetingService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public MeetingController(IMeetingService meetingService,
            IUserInMeetingService userInMeetingService,
            IUserService userService,
            IPermissionService permissionService)
        {
            _meetingService = meetingService;
            _userInMeetingService = userInMeetingService;
            _userService = userService;
            _permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            var meetings = await _meetingService.GetAllAsync(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = meetings.Select(x => new MeetingModel
                {
                    Id = x.Id,
                    Department = new DepartmentViewModel() { Id = x.Department.Id, Name = x.Department.Name },
                    CurrentLeader = PrepareCurrentLeaderForViewMeeting(x.CurrentLeaderId),
                    Leaders = PrepareLeadersForViewMeeting(x),
                    Members = PrepareMembersForViewMeeting(x)
                }),
                Total = meetings.TotalCount
            };

            return Json(gridModel);
        }

        private UserForMeetingViewModel PrepareCurrentLeaderForViewMeeting(int id)
        {
            if (id == 0)
                return new UserForMeetingViewModel();
            var currentLeader = _userService.GetByIdAsync(id).Result;
            return new UserForMeetingViewModel()
            {
                Id = currentLeader==null?0:currentLeader.Id,
                Name = currentLeader==null?"":currentLeader.Username
            };
        }

        private string PrepareMembersForViewMeeting(Meeting x)
        {
            if (x.UserInMeetings == null)
                return null;
            var members = x.UserInMeetings
                                .Where(u => u.IsLeader == false)
                                .Select(m => m.User.Username);
            //parse to string format: member, members,...
            return String.Join(", ", members);
        }

        private string PrepareLeadersForViewMeeting(Meeting x)
        {
            if (x.UserInMeetings == null)
                return null;
            var leaders = x.UserInMeetings
                                .Where(u => u.IsLeader == true)
                                .OrderBy(u=>u.Order)
                                .Select(m => m.UserId == x.CurrentLeaderId ? "<strong style='color: red'>" + m.User.Username + "</strong>" : m.User.Username);
            //parse to string format: leader, leader,...
            return String.Join(", ", leaders);

        }

        /*
        [HttpPost]
        public async Task<ActionResult> Create(MeetingModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Department.Id < 1)
                    return Content("Department is required!");

                var exsitMeeting = await _meetingService.GetMeetingByDepartmentId(model.Department.Id);
                //var existDepartment = _meetingService.GetAllAsync().Where(m => m.DepartmentId == model.Department.Id).FirstOrDefault();
                //check exist department
                if (exsitMeeting != null)
                    return Content(model.Department.Name + " has Existed!");

                var meeting = new Meeting()
                {
                    DepartmentId = model.Department.Id,
                    UpdateCurrentLeaderDate = DateTime.Now.Date
                    //CurrentLeaderId = model.CurrentLeader.Id
                };

                await _meetingService.InsertAsync(meeting);

                return Json(new
                {
                    status = "success",
                });
            }

            // If we got this far, something failed, redisplay form
            //return Json(new { Data = new[] { model } });
            return Content("Can not create because model state is invalid");
        }

        [HttpPost]
        public async Task<ActionResult> Update(MeetingModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Department.Id < 1)
                    return Content("Department is required!");

                var exsitMeeting = await _meetingService.GetMeetingByDepartmentId(model.Department.Id);
                //check department has existed and is not current meeting
                if (exsitMeeting != null && exsitMeeting.Id != model.Id)
                    return Content(model.Department.Name + " has Existed!");

                var meeting = await _meetingService.GetByIdAsync(model.Id);
                if (meeting == null)
                    throw new ArgumentException("No meeting found with the specified id");
               
                meeting.DepartmentId = model.Department.Id;
                await _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, model.CurrentLeader.Id);
                meeting.CurrentLeaderId = model.CurrentLeader.Id;
                await _meetingService.UpdateAsync(meeting);
                return Json(new
                {
                    status = "success",
                });
            }
            return Content("Can not edit because model state is invalid");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var meeting = await _meetingService.GetByIdAsync(id);
            if (meeting == null)
                throw new ArgumentException("No meeting found with the specified id");
            await _meetingService.DeleteAsync(meeting);
            return Json(new
            {
                status = "success",
            });
        }
        */

        #region User in Meeting

        public ActionResult ListUserInMeeting(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
             return AccessDeniedView();

            if (id < 1)
                return Content("MeetingId is invalid!");
            var meetingTransportModel = new MeetingTransportModel();
            meetingTransportModel.Id = id;
            return View(meetingTransportModel);
        }

        [HttpPost]
        public async Task<ActionResult> ListUserInMeeting(DataSourceRequest command, int Id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            var meeting = await _meetingService.GetByIdAsync(Id);

            var userInMeetings = new PagedList<UserInMeeting>(meeting.UserInMeetings.OrderBy(um => um.Order).AsQueryable(), command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = userInMeetings.Select(x => new UserInMeetingModel
                {
                    Id = x.Id,
                    MeetingId = x.MeetingId,
                    User = new UserForMeetingViewModel() { Id = x.User == null ? 0 : x.User.Id, Name = x.User == null ? "" : x.User.Username },
                    IsLeader = x.IsLeader,
                    IsCurrentLeader = x.UserId == x.Meeting.CurrentLeaderId,
                    Order = x.Order
                }),
                Total = userInMeetings.TotalCount
            };

            return Json(gridModel);
        }
        [HttpPost]
        public async Task<ActionResult> CreateUserInMeeting(UserInMeetingModel model, int meetingId)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            if (model.User == null || model.User.Id < 1)
                return Content("User is invalid!");
            if (ModelState.IsValid)
            {
                var meeting = await _meetingService.GetByIdAsync(model.MeetingId);
                var existUser = meeting.UserInMeetings.FirstOrDefault(u => u.UserId == model.User.Id);

                if (existUser != null)
                    return Content(model.User.Name + " has Existed!");

                if (!model.IsLeader)
                    if (model.IsCurrentLeader)
                        return Content("Please set leader before set current leader");

                var userInMeeting = new UserInMeeting()
                {
                    MeetingId = model.MeetingId,
                    UserId = model.User.Id,
                    IsLeader = model.IsLeader,
                    Order = model.Order
                };
                await _userInMeetingService.InsertAsync(userInMeeting);
                ////if set user is current leader or chua co ai la leader and this user isleader
                //if (model.IsCurrentLeader || (meeting.CurrentLeaderId == 0 && userInMeeting.IsLeader))
                if(model.IsCurrentLeader)
                {
                    await _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, model.User.Id);
                    meeting.CurrentLeaderId = model.User.Id;
                    await _meetingService.UpdateAsync(meeting);
                }
                return Json(new
                {
                    status = "success",
                });
            }

            // If we got this far, something failed, redisplay form  
            return Content("Can not Create User in Meeting because model state is invalid");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUserInMeeting(UserInMeetingModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            if (model.User == null || model.User.Id < 1)
                return Content("User is invalid!");

            if (ModelState.IsValid)
            {
                var meeting = await _meetingService.GetByIdAsync(model.MeetingId);
                var existUser = meeting.UserInMeetings.FirstOrDefault(u => u.UserId == model.User.Id);

                //check user has existed and is not current user
                if (existUser != null && existUser.Id != model.Id)
                    return Content(model.User.Name + " has Existed in Meeting");

                if (!model.IsLeader)
                    if (model.IsCurrentLeader)
                        return Content("Please set is leader before set is current leader");

                //if(!model.IsCurrentLeader)
                //    if(meeting.CurrentLeaderId == model.User.Id)
                //        return Content("This user is current leader, can't set it isn't current leader");

                var userInMeeting = await _userInMeetingService.GetByIdAsync(model.Id);
                if (userInMeeting == null)
                    throw new ArgumentException("No meeting found with the specified id");
                userInMeeting.UserId = model.User.Id;
                userInMeeting.IsLeader = model.IsLeader;
                userInMeeting.Order = model.Order;
                //if (model.IsCurrentLeader || (meeting.CurrentLeaderId == 0 && userInMeeting.IsLeader))
                if(model.IsCurrentLeader)
                {
                    await _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, model.User.Id);
                    meeting.CurrentLeaderId = model.User.Id;
                    await _meetingService.UpdateAsync(meeting);
                }
                else
                {
                    if(meeting.CurrentLeaderId == model.User.Id)
                    {
                        await _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, 0);
                        meeting.CurrentLeaderId = 0;
                        await _meetingService.UpdateAsync(meeting);
                    }
                }

                await _userInMeetingService.UpdateAsync(userInMeeting);
                return Json(new
                {
                    status = "success",
                });
            }
            return Content("Can not Edit User in Meeting because model state is invalid");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUserInMeeting(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            var userInMeeting = await _userInMeetingService.GetByIdAsync(id);
            if (userInMeeting == null)
                throw new ArgumentException("No UserInMeeting found with the specified id");

            var meeting = await _meetingService.GetByIdAsync(userInMeeting.MeetingId);

            if (meeting.CurrentLeaderId == userInMeeting.UserId)
            {
                //return Content("This user is current leader, can't delete.");
                return Json(new
                {
                    status = "This user is current leader, can't delete."
                });
                //var firstUser = meeting.UserInMeetings.OrderBy(um => um.Order).FirstOrDefault(u => u.UserId != userInMeeting.UserId && u.IsLeader);

                //await _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, firstUser == null ? 0 : firstUser.UserId);
                //meeting.CurrentLeaderId = firstUser == null ? 0 : firstUser.UserId;
                //await _meetingService.UpdateAsync(meeting);
            }

            await _userInMeetingService.DeleteAsync(userInMeeting);
            return Json(new
            {
                status = "success",
            });
        }

        //get all leader for Meeting View
        [HttpPost]
        public JsonResult GetAllLeader(DataSourceRequest command, int Id)
        {
            List<UserForMeetingViewModel> data;
            if (Id != 0)
            {
                var meeting = _meetingService.GetByIdAsync(Id);
                data = meeting.Result.UserInMeetings
                    .Where(u => u.IsLeader)
                    .Select(x => new UserForMeetingViewModel
                    {
                        Id = x.UserId,
                        Name = x.User.Username
                    }).ToList();
            }
            else
            {
                data = new List<UserForMeetingViewModel>();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}