using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class CoursesController : Controller
    {
        //
        // GET: /Courses/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            string test = "{scorm_model: {status: success, data: {actionRequested:, params:}";

            return Json(test);
        }
    }
}
