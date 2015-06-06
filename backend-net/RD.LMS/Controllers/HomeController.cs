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

            
                Session.Add("USR_ID", 1);
                Models.LMSModel r = new Models.LMSModel();
                r.StudentId = "1";
            
                
            
            return View();
        }

        public ActionResult Course()
        {
            return View();
        }

    }
}
