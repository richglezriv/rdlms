﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class CaptchaController : Controller
    {
        //
        // GET: /Captcha/

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult _PartialCaptcha()
        {
            
            return View();
        }
    }
}