using RD.LMS.Models;
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
            model.status = "success";

            return Json(model);
        }

        public ActionResult Logout()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.status = "success";

            return Json(model);
        }

        public ActionResult Get()
        {
            Models.JSonModelCollection model = new Models.JSonModelCollection();
            List<Models.UserCourseModel> courses = new List<UserCourseModel>();

            //loads courses
            courses.Add(new UserCourseModel()
            {
                id = "123",
                name = "Reaccion Digital",
                status = "browsed",
                active = true,
                description = "RD Course Demo",
                totalTime = "102",
                thumbnail = "thumb1.png"
            });

            model.data = courses.ToList<IDataModel>();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registration()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.status = "success";

            return Json(model);
        }

        public ActionResult Edit()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.status = "success";

            return Json(model);
        }

        public ActionResult User() { throw new NotImplementedException(); }

        public ActionResult Fetch(string data)
        {

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.LMSModel lms = new Models.LMSModel();
            lms.LoadValues(toFetch);
            lms.StudentId = Session["USR_ID"].ToString();

            JSonModel model = new JSonModel()
            {
                data = new CourseModel()
                {
                    id = toFetch["data"]["courseId"].ToString(),
                    name = String.Format("Name of course {0}", toFetch["data"]["courseId"].ToString()),
                    scorm = "reacciondigital",
                    scoIndex = "player.html",
                    scoPath = "reacciondigital",
                    status = "active",
                    dataModel = lms.GenerateJSonString()
                },

            };

            return Json(model);
        }

        public ActionResult Commit(string jsonModel)
        {

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonModel);
            Models.LMSModel lms = new Models.LMSModel();
            lms.LoadValues(toFetch);
            lms.StudentId = Session["USR_ID"].ToString();

            //toFetch["data"]["cmi.student_id"]
            JSonModel model = new JSonModel()
            {
                data = new CourseModel()
                {
                    id = "123",
                    dataModel = lms.GenerateJSonString()
                }
            };

            return Json(model);
        }

    }
}
