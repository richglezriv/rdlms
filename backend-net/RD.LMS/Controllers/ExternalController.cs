using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class ExternalController : Controller
    {
        //
        // GET: /External/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            Models.JSonModel model = new Models.JSonModel();

            return Json(model);
        }
    }
}
