using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain.Users;
using Service.Departments;
using Service.Interface;
using Service.Messages;
using Service.Users;
using Utils;
using Service.Common;
using Service.Tasks;

namespace Web.Controllers
{
    
    public class HomeController : Controller
    {

        private readonly ISendMailService _sendMailService;
        private readonly IWorkFlowMessageService _workFlowMessageService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IWorkContext _workContext;
        private readonly IScheduleTaskService _scheduleTaskService;

        public HomeController(IUserService userService,
            ISendMailService sendMailService, 
            IWorkFlowMessageService workFlowMessageService, 
            IDepartmentService departmentService, 
            IWorkContext workContext,
            IScheduleTaskService scheduleTaskService)
        {
            _sendMailService = sendMailService;
            _workFlowMessageService = workFlowMessageService;
            _departmentService = departmentService;
            _workContext = workContext;
            _userService = userService;
            _scheduleTaskService = scheduleTaskService;
        }


        public ActionResult Index()
        {
            if(_workContext.CurrentUser == null)
                return RedirectToAction("Index", "QualityAlert");

            var department = _departmentService.SearchDepartment(_workContext.CurrentUser.IsAdmin() ? null : _workContext.CurrentUser, true).FirstOrDefault();

            if (_workContext.CurrentUser!=null)
            {
                if (_workContext.CurrentUser.IsAdmin())
                    return RedirectToAction("Index", "UserManager");
                return RedirectToAction("Index", "DdsMeeting",new { id = department==null?1:department.Id});
            }

            return RedirectToAction("Index", "DdsMeeting", new { id = department == null ? 1 : department.Id });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public Task<ActionResult> ExecuteTask()
        {
            var stask = _scheduleTaskService.GetTaskById(3).Result;

            var task = new Service.Tasks.Task(stask);

            task.Enabled = true;
            task.Execute(true);

            return null;
        }

        public async Task<ActionResult> TestSendMailWithAttachment()
        {
            var userTuyen = await _userService.GetUserByEmailAsync("tuyen.nguyen@newoceaninfosys.com");
            //if (userTuyen != null)
            //{
            //    var listAttachment = new List<string>() {"test.txt"};
                //var queueEmail = _workFlowMessageService.SendReportToUser(userTuyen);
                //var result = _sendMailService.Sendmail(queueEmail, listAttachment);


                //var listAttachment1 = new List<string>();
                //var queueEmail1 = _workFlowMessageService.SendIssusToUser(userTuyen);
                //var result2 = _sendMailService.Sendmail(queueEmail1, listAttachment1);


                //return Content(result.ToString());
            //}




            return Content("not exist user");

        }
    }
}