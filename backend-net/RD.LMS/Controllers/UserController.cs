using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Login()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.message = "success";

            return Json(model);
        }

        public ActionResult Logout()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.message = "success";

            return Json(model);
        }

        public ActionResult Get()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.message = "success";

            return Json(model);
        }

        public ActionResult Register()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.message = "success";

            return Json(model);
        }

        public ActionResult Edit()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.message = "success";

            return Json(model);
        }

    }
}
