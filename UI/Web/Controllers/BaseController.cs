using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }
        /// <summary>
        /// Return list error message from ModelState
        /// </summary>
        /// <returns></returns>
        protected String GetErrorMessageFromModelState()
        {
            return String.Join("\n", ModelState.Values.Where(v => v.Errors.Count() != 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }
    }
}