using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class AdminController : Controller
    {

        public ActionResult Get(string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            RD.LMS.Models.JSonModelCollection model = new RD.LMS.Models.JSonModelCollection();

            Models.JSonModel mSession = Utilities.IsSessionActive(Session[Utilities.USER]);
            if (mSession != null)
                return Json(mSession, JsonRequestBehavior.AllowGet);

            List<Models.CourseModel> list = CourseModel.GetCourses();
            model.data = list.ToList<IDataModel>();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(String data, string csrftoken) {

            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            JSonModel model = new JSonModel();
            CourseModel course = new CourseModel();
            try
            {
                //TODO Read settings from json file
            
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            course.LoadValues(toFetch);
            //course thumbnail
            string source = Server.MapPath("~/uploads/") + course.thumbnail;
            source = source.Replace("home", "FileUp");
            //string extension = course.thumbnail.Remove(0, course.thumbnail.Length - 4);
            //course.thumbnail = course.name + extension;
            string destination = Server.MapPath("~/scorm-images/") + course.thumbnail;
            //copy file
            if (System.IO.File.Exists(source))
            {
                System.IO.File.Copy(source, destination, true);
            }
            
            //scorm course zip
            source = Server.MapPath("~/uploads/" + course.scorm).Replace("home", "FileUp"); ;
            destination = Server.MapPath("~/scorm-packages/") + course.scorm;
            if (System.IO.File.Exists(source))
            {
                System.IO.File.Copy(source, destination, true);
                System.IO.File.Delete(source);
                ProcessController.UnzipFile(destination);
                course.SetManifest(Server.MapPath("~/scorm-packages/"));
            }

            course.Save();
            }
            catch (Exception ex)
            {
                model.status = ex.Message;
            }

            return Json(model);
        }

        public ActionResult Stats(string data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            string id = toFetch["courseId"].ToString();
            RD.LMS.Models.JSonModel model = new RD.LMS.Models.JSonModel();
            Models.CourseStatsModel stats = new CourseStatsModel(id);
            model.data = stats;

            return Json(model);
        }

        public ActionResult Delete(string data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            int id =  int.Parse(toFetch["courseId"].ToString());
            RD.LMS.Models.JSonModel model = new RD.LMS.Models.JSonModel();
            CourseModel course = new CourseModel();
            course.id = id.ToString();
            course.Delete();
            return Json(model);
        }

        public ActionResult FindUsers(String data, String csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            JSonModelCollection model = new JSonModelCollection();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            try
            {
                UserModel uModel = new UserModel();
                uModel.DoSearch(toFetch["query"].ToString());
                model.data = uModel.Users;
            }
            catch (Exception)
            {
                model.status = "fail";   
            }
            return Json(model);
        }

        public ActionResult DeleteUser(String data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Models.JSonModel model = new JSonModel();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.UserModel cUser = new UserModel();
            Entities.User user = new Entities.User()
            {
                Id = Convert.ToInt32(toFetch["userId"].ToString())
            };

            cUser.Delete(user);

            return Json(model);

        }

        public ActionResult SaveUser(String data, String csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Models.JSonModel model = new JSonModel();
            Models.LMSUser user = (LMSUser)Session[Utilities.USER];

            try
            {
                Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                user.occupation = Convert.ToInt16(toFetch["occupation"].ToString());
                user.organization = Convert.ToInt16(toFetch["organization"].ToString());
                user.gender = toFetch["gender"].ToString();
                user.birthday = toFetch["birthday"].ToString();
                user.password = toFetch["oldPassword"].ToString();
                user.newPassword = toFetch["newPassword"].ToString();
                user.newPasswordCheck = toFetch["newPasswordCheck"].ToString();
                user.name = toFetch["name"].ToString();
                user.lastName = toFetch["lastName"].ToString();
                user.secondLastName = toFetch["secondLastName"].ToString();
                user.UpdateUser();

                Session[Utilities.USER] = user;
            }
            catch (Exception ex)
            {
                model.status = "fail";
                model.data = user;
            }

            return Json(model);
        }

        public ActionResult RegisterUser(String data)
        {
            Models.JSonModel model = new JSonModel();
            Models.LMSUser user = new LMSUser();

            try
            {
                Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                System.Collections.Concurrent.ConcurrentDictionary<CaptchaMvc.Models.KeyTimeEntry<string>, CaptchaMvc.Interface.ICaptchaValue> values = (System.Collections.Concurrent.ConcurrentDictionary<CaptchaMvc.Models.KeyTimeEntry<string>, CaptchaMvc.Interface.ICaptchaValue>)Session["____________SessionValidateKey_____________"];
                user.ValidateCaptcha(toFetch["captcha"].ToString(), values);
                user.occupation = Convert.ToInt16(toFetch["occupation"].ToString());
                user.organization = Convert.ToInt16(toFetch["organization"].ToString());
                user.gender = toFetch["gender"].ToString();
                user.birthday = toFetch["birthday"].ToString();
                user.password = toFetch["password"].ToString();
                user.newPasswordCheck = toFetch["passwordCheck"].ToString();
                user.email = toFetch["email"].ToString();
                user.name = toFetch["name"].ToString();
                user.lastName = toFetch["lastName"].ToString();
                user.secondLastName = toFetch["secondLastName"].ToString();
                user.Register();

            }
            catch (Exception ex)
            {
                model.status = "fail";
                //string message = ex.Message;
                //message += ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                //message += ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : string.Empty;
                //user.SetReason(message);
                model.data = user;
            }

            return Json(model);
        }

        public ActionResult GetUserStats(String data)
        {
            Models.JSonModelCollection model = new Models.JSonModelCollection();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            List<Models.UserCourseModel> courses = Models.UserCourseModel.Get(toFetch["userId"].ToString());
            model.data = courses.ToList<IDataModel>();

            return Json(model);
        }

        public ActionResult ClearUserScorm(String data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Models.JSonModel model = new JSonModel();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.UserCourseModel userCourse = new UserCourseModel();
            userCourse.ResetCourse(toFetch["userId"].ToString(), toFetch["courseId"].ToString());

            return Json(model);
        }

        private string isSessionActive(out Boolean active)
        {
            active = true;
            if (Session[Utilities.USER] == null)
            {
                active = false;
                return "session-expired";
            }

            return "success";

        }
    }
}
