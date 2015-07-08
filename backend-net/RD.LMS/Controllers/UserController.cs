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
        private string USER_COURSE = "USER_COURSE";

        public ActionResult Login(string data)
        {
            Models.JSonModel model = new Models.JSonModel();
            Newtonsoft.Json.Linq.JObject json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            LMSUser user = new LMSUser()
            {
                login = json["username"].ToString(),
                password = json["password"].ToString(),
            };

            model.status = user.Validate();

            if (model.status.Equals(Utilities.SUCCESS))
                Session.Add(Utilities.USER, user);

            if (user.TryOuts > 3)
            {
                model.status = Utilities.FAIL;
                user.SetReason();
            }
            
            model.data = user;

            return Json(model);
        }

        public ActionResult Logout()
        {
            Models.JSonModel model = new Models.JSonModel();
            model.status = Utilities.SUCCESS;

            try
            {
                if (Session[Utilities.USER] != null)
                {
                    LMSUser user = (LMSUser)Session[Utilities.USER];
                    user.LogOut();
                }
            }
            catch (Exception) { }

            Session.Abandon();
            
            return Json(model);
        }

        public ActionResult Get()
        {
            Models.JSonModelCollection model = new Models.JSonModelCollection();
            List<Models.UserCourseModel> courses = Models.UserCourseModel.Get(((Models.LMSUser)Session[Utilities.USER]).id);

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

        public ActionResult GetUser() { throw new NotImplementedException(); }

        public ActionResult Fetch(string data)
        {

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.LMSModel lms = new Models.LMSModel();
            Entities.UserCourse course = Business.UserController.GetCourse(Convert.ToInt32(toFetch["courseId"].ToString()));
            LMSUser user = Session[Utilities.USER] != null ? (LMSUser)Session[Utilities.USER] : null;
            CourseModel toCourse = new CourseModel();

            toCourse.LoadCourse(course);
            lms.LoadCourse(course);
            lms.StudentName = user.name;
            toCourse.dataModel = lms.GenerateJSonString();
            object o = Newtonsoft.Json.JsonConvert.DeserializeObject(toCourse.dataModel);
            JSonModel model = new JSonModel()
            {
                data = toCourse,
                status = "success"
            };

            Session.Add(USER_COURSE, course);

            return Json(model);
        }

        public ActionResult Commit(string data)
        {
            Entities.UserCourse course = (Entities.UserCourse)Session[USER_COURSE];
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.LMSModel lms = new Models.LMSModel();
            lms.LoadCourse(course);
            lms.LoadValues(toFetch);
            lms.EvalCourse(course);
            LMSUser user = Session[Utilities.USER] != null ? (LMSUser)Session[Utilities.USER] : null;
            
            JSonModel model = new JSonModel()
            {
                data = new CourseModel()
                {
                    dataModel = lms.GenerateJSonString()
                }
            };

            Session.Remove(USER_COURSE);

            return Json(model);
        }

    }
}
