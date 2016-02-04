using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace RD.LMS.Controllers
{
    public class UserController : Controller
    {
        List<UserTryout> tryouts;
        private const string TRYOUTS = "USER_TRYOUTS";
        private string USER_COURSE = "USER_COURSE";
        private Boolean _sessionActive;

        public ActionResult Login(string data)
        {
            
            Models.JSonModel model = new Models.JSonModel();
            Newtonsoft.Json.Linq.JObject json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            
            LMSUser user = new LMSUser()
            {
                email = json["email"].ToString().Trim(),
                password = json["password"].ToString().Trim(),
                TryOuts = Convert.ToInt32(Session["TRYOUTS"])
            };
            try
            {

                tryouts = GetTryouts();
                ReviewTryouts();

                if (tryouts.Any(u => u.login.Equals(user.email)))
                {
                    model.status = Utilities.FAIL;
                    user.SetReason();
                }
                else
                {
                    model.status = user.Validate();
                    Session.Add("TRYOUTS", user.TryOuts);

                    if (model.status.Equals(Utilities.SUCCESS))
                        Session.Add(Utilities.USER, user);
                    else if (user.TryOuts > 5)
                    {
                        model.status = Utilities.FAIL;
                        user.SetReason();
                        if (!tryouts.Any(u => u.login.Equals(user.login)))
                            tryouts.Add(new UserTryout() { login = user.email, lastTry = DateTime.Now });
                        this.HttpContext.Application[TRYOUTS] = tryouts;
                    }

                }

                model.data = user;
            }
            catch (Exception ex)
            {
                model.status = "fail";
                //string message = ex.Message;
                //message += ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                //user.SetReason(String.Format("exception {0}", message));
                model.data = user;
            }

            return Json(model);
        }

        private void CreateSessionId()
        {
            HttpContext context = System.Web.HttpContext.Current;
            Boolean redirected, cookieAdded;
            SessionIDManager manager = new SessionIDManager();
            System.Diagnostics.Debug.WriteLine("first pass " + manager.GetSessionID(context));
            string newId = manager.CreateSessionID(context);
            System.Diagnostics.Debug.WriteLine("what should be " + newId);
            manager.RemoveSessionID(context);
            manager.SaveSessionID(context, newId, out redirected, out cookieAdded);
            System.Diagnostics.Debug.WriteLine("second pass " + manager.GetSessionID(context));
        }

        private void ReviewTryouts()
        {
            for (int i = tryouts.Count - 1; i >= 0; i--)
            {
                if (DateTime.Now.Subtract(tryouts[i].lastTry).Minutes >= 60)
                    tryouts.RemoveAt(i);
            }
        }

        public ActionResult Logout()
        {
            Models.JSonModel model = new Models.JSonModel();

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
            
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(String csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Models.JSonModelCollection model = new Models.JSonModelCollection();
             
            List<Models.UserCourseModel> courses = Models.UserCourseModel.Get(((Models.LMSUser)Session[Utilities.USER]).id);

            model.data = courses.ToList<IDataModel>();

            return Json(model);
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

        public ActionResult Fetch(string data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Models.LMSModel lms = new Models.LMSModel();
            JSonModel model = new JSonModel();
            try
            {
                Entities.UserCourse course = Business.UserController.GetCourse(Convert.ToInt32(toFetch["courseId"].ToString()));

                if (course == null)
                {
                    model.data = new MessageData(){
                      message=  "No puede acceder todavía a la evaluación de este curso."
                    };
                    model.status = "fail";
                    return Json(model);
                }

                LMSUser user = Session[Utilities.USER] != null ? (LMSUser)Session[Utilities.USER] : null;
                CourseModel toCourse = new CourseModel();

                toCourse.LoadCourse(course);
                //set the index sco file
                SetScoIndexFile(course.Course, toCourse);
                lms.LoadCourse(course);
                lms.StudentName = string.Format("{0} {1} {2}", user.name, user.lastName, user.secondLastName);
                toCourse.dataModel = lms.GenerateJSonString();
                object o = Newtonsoft.Json.JsonConvert.DeserializeObject(toCourse.dataModel);

                model.data = toCourse;

                Session.Add(USER_COURSE, toCourse);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CourseLoad:"))
                {
                    model.data = new MessageData()
                    {
                        message = ex.Message
                    };
                }
                else
                {
                    model.data = new MessageData()
                    {
                        message = "error al cargar el curso, verifique con el adminitrador"
                    };
                }
                model.status = "fail";
            }
            return Json(model);
        }

        private void SetScoIndexFile(Entities.Course course, CourseModel model)
        {
            try
            {
                if (course.ScoIndex == null)
                    model.SetManifest(Server.MapPath("~/scorm-packages/"));
            }
            catch (Exception ex)
            {
                throw new Exception("CourseLoad: " + ex.Message);
            }

        }

        public ActionResult Commit(string data, string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            CourseModel toCourse = (CourseModel)Session[USER_COURSE];
            Entities.UserCourse course = Business.UserController.GetCourse(Convert.ToInt32(toCourse.id));
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

            return Json(model);
        }

        private JsonResult StateLoggedOut()
        {
            Models.JSonModel model = new JSonModel();
            model.status = "success";
            model.data = JSonUserModel.GetLoggedOut();
            return Json(model);
        }

        public ActionResult UserSessionState(string csrftoken = null)
        {
            Models.JSonModel model = new JSonModel();
            if (Session[Utilities.USER] != null)
            {
                LMSUser user = (LMSUser)Session[Utilities.USER];
                if (Utilities.IsValidToken(csrftoken, user))
                {
                    model.status = "success";
                    model.data = new JSonUserModel()
                    {
                        sessionType = user.GetSessionType(),
                        user = user
                    };
                    return Json(model);
                }
                else
                {
                    CreateSessionId();
                    return Utilities.StateLoggedOut();
                }
            }
            else
            {
                CreateSessionId();
                return StateLoggedOut();
            }
        }

        public ActionResult Ping(string csrftoken)
        {
            if (!Utilities.IsValidToken(csrftoken, Session[Utilities.USER] as LMSUser))
                return Utilities.StateSessionExpired();

            Models.JSonModel model = new JSonModel();
            model.status = "success";

            return Json(model);
        }

        private List<UserTryout> GetTryouts()
        {
            return this.HttpContext.Application[TRYOUTS] == null ? new List<UserTryout>() : (List<UserTryout>)this.HttpContext.Application[TRYOUTS];
        }

    }
}
