using System.Web.Mvc;
using Utils;

namespace Web.Controllers
{
    public class CommonController : Controller
    {
        private readonly IWorkContext _workContext;
        public CommonController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        //
        // GET: /Common/
        public ActionResult PermissionDeny()
        {
            return View();
        }
        public ActionResult LoginPartial()
        {
            ViewBag.IsLogin = _workContext.CurrentUser != null;
            if (_workContext.CurrentUser != null)
                ViewBag.Username = _workContext.CurrentUser.Username;
            return PartialView();
        }
    }
}