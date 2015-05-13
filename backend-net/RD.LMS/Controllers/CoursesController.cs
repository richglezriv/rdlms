using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class CoursesController : Controller
    {
        //
        // GET: /Courses/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            String user = Session["USR_ID"].ToString();
            Models.JSonModelCollection model = new Models.JSonModelCollection();

            model.message = "success";
            List<Models.CourseModel> list = new List<Models.CourseModel>();
            list.Add(new Models.CourseModel { id = "123", name = "course 123" });
            list.Add(new Models.CourseModel { id = "234", name = "course 234" });

            model.data = list.ToList<Models.IDataModel>();

            return Json(model);
        }
    }
}
