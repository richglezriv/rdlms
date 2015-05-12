using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            string test = "{scorm_model: {status: success, data: {actionRequested:, params:}";

            return Json(test);
        }

        public ActionResult Logout()
        {
            string test = "{scorm_model: {status: success, data: {actionRequested:, params:}";

            return Json(test);
        }

    }
}
