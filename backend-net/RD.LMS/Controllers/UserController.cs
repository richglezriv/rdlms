﻿using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                login = json["username"].ToString(),
                password = json["password"].ToString(),
                TryOuts = Convert.ToInt32(Session["TRYOUTS"])
            };

            try
            {
                tryouts = GetTryouts();
                ReviewTryouts();

                if (tryouts.Any(u => u.login.Equals(user.login)))
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
                            tryouts.Add(new UserTryout() { login = user.login, lastTry = DateTime.Now });
                        this.HttpContext.Application[TRYOUTS] = tryouts;
                    }
                    
                }

                model.data = user;
            }
            catch (Exception ex)
            {

            }



            return Json(model);
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

        public ActionResult Get()
        {
            Models.JSonModelCollection model = new Models.JSonModelCollection();
            Models.JSonModel mSession = Utilities.IsSessionActive(Session[Utilities.USER]);

            if (mSession != null)
                return Json(mSession , JsonRequestBehavior.AllowGet);
             
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
                lms.LoadCourse(course);
                lms.StudentName = user.name;
                toCourse.dataModel = lms.GenerateJSonString();
                object o = Newtonsoft.Json.JsonConvert.DeserializeObject(toCourse.dataModel);

                model.data = toCourse;

                Session.Add(USER_COURSE, course);
            }
            catch (Exception)
            {
                model.data = new MessageData()
                {
                    message = "error al cargar el curso, verifique con el adminitrador"
                };
                model.status = "fail";
            }
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

            return Json(model);
        }

        public ActionResult SessionEnabled()
        {
            Models.JSonModel model = new JSonModel();
            if (Session[Utilities.USER] != null){
                LMSUser user = (LMSUser)Session[Utilities.USER];
                model.data = new JSonUserModel()
                {
                    sessionType = "admin",
                    user = user
                };
                model.status = "success";
            }
            else
            {
                model.status = "success";
                model.data = new JSonUserModel()
                {
                    sessionType = "logged-out",
                    user = null
                };
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<UserTryout> GetTryouts()
        {
            return this.HttpContext.Application[TRYOUTS] == null ? new List<UserTryout>() : (List<UserTryout>)this.HttpContext.Application[TRYOUTS];
        }

    }
}
