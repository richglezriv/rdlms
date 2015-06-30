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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            
            RD.LMS.Models.JSonModelCollection model = new RD.LMS.Models.JSonModelCollection();

            model.status = Utilities.SUCCESS;
            List<Models.CourseModel> list = CourseModel.GetCourses();
            model.data = list.ToList<IDataModel>();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(String data) {
            //TODO Read settings from json file
            JSonModel model = new JSonModel();
            CourseModel course = new CourseModel();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            course.LoadValues(toFetch);
            //course thumbnail
            string source = Server.MapPath("/uploads/") + course.thumbnail;
            string extension = course.thumbnail.Remove(0, course.thumbnail.Length - 4);
            course.thumbnail = course.name + extension;
            string destination = Server.MapPath("/uploads/") + course.thumbnail;
            //copy file
            if (source != destination) {
                System.IO.File.Copy(source, destination, true);
                //System.IO.File.Delete(source);
            }
            
            //scorm course zip
            source = Server.MapPath("/uploads/" + course.scorm);
            extension = course.scorm.Remove(0, course.scorm.Length - 4);
            course.scorm = course.name + extension;
            destination = Server.MapPath("/scorm-packages/") + course.scorm;
            if (System.IO.File.Exists(source))
            {
                System.IO.File.Copy(source, destination, true);
                //System.IO.File.Delete(source);
                ProcessController.UnzipFile(destination);
                course.SetManifest(Server.MapPath("/scorm-packages/"));
            }

            //register course
            course.Save();

            return Json(model);
        }

        public ActionResult Stats(string data)
        {
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            int id = int.Parse(toFetch["courseId"].ToString());
            RD.LMS.Models.JSonModel model = new RD.LMS.Models.JSonModel();
            Models.CourseStatsModel stats = new CourseStatsModel()
            {
                name = "course",
                id = "123",
                incomplete = 100,
                failed = 50,
                notAttempted = 5,
                passed = 10,
                completed = 5
            };
            model.data = stats;

            return Json(model);
        }

        public ActionResult Delete(string data)
        {
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            int id =  int.Parse(toFetch["courseId"].ToString());
            RD.LMS.Models.JSonModel model = new RD.LMS.Models.JSonModel();
            CourseModel course = new CourseModel();
            course.id = id.ToString();
            course.Delete();
            return Json(model);
        }

        public ActionResult FindUsers(String jsonModel) { throw new NotImplementedException(); }

        public ActionResult DeleteUser(String jsonModel) { throw new NotImplementedException(); }

        public ActionResult SaveUser(String jsonModel) { throw new NotImplementedException(); }

        public ActionResult GetUserStats(String jsonModel) { throw new NotImplementedException(); }
        
    }
}
