using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login(string jsonModel)
        {
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonModel);
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
