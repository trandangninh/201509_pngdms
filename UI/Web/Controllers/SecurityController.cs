using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils;

namespace Web.Controllers
{
    public class SecurityController : BaseController
    {
        public ActionResult AccessDenied(string pageUrl)
        {
            return View();
        }
    }
}