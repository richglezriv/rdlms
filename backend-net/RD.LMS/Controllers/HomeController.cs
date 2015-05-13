using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(String Id)
        {

            if (Id != null)
            {
                Session.Add("USR_ID", Id);
                Models.LMS r = new Models.LMS();
                r.StudentId = "0";
            }
                
            
            return View();
        }


        public ActionResult Fetch(string jsonModel)
        {

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonModel);
            Models.LMS lms = new Models.LMS();
            lms.LoadValues(toFetch);
            lms.StudentId = Session["USR_ID"].ToString();

            JSonModel model = new JSonModel()
            {
                message = "success",
                data = new CourseModel()
                {
                    id = toFetch["data"]["courseId"].ToString(),
                    name = String.Format("Name of course {0}", toFetch["data"]["courseId"].ToString()),
                    scoPath = "/root/index.html",
                    scoIndex = "0",
                    status = "active",
                    dataModel = lms.GenerateJSonString()
                },
                
            };
            
            return Json(model);
        }

        public ActionResult Commit(string jsonModel)
        {

            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonModel);
            Models.LMS lms = new Models.LMS();
            lms.LoadValues(toFetch);
            lms.StudentId = Session["USR_ID"].ToString();
            
            //toFetch["data"]["cmi.student_id"]
            JSonModel model = new JSonModel()
            {
                message= "success",
                data = new CourseModel()
                {
                    id = "id_123",
                    dataModel = lms.GenerateJSonString()
                }
            };

            return Json(model);
        }


        public ActionResult Course()
        {
            return View();
        }

    }
}
