using Service.Interface;
using Service.Meetings;
using System.Linq;
using System.Web.Mvc;
using Service.Security;
using Utils;
using Web.Models.Meeting;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    public class UserInMeetingController : Controller
    {
        private readonly IUserInMeetingService _userInMeetingService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public UserInMeetingController(IUserInMeetingService userInMeetingService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _userInMeetingService = userInMeetingService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        //get all leader for Meeting View
        [HttpPost]
        public JsonResult GetAllLeader(DataSourceRequest command)
        {
            var userInMeetings = _userInMeetingService.GetAllAsync().Result;
            var data = userInMeetings.Where(u => u.IsLeader == true).Select(x => new UserForMeetingViewModel
            {
                Id = x.UserId,
                Name = x.User.Username
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}