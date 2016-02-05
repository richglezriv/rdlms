using System;
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
            CaptchaMvc.Infrastructure.CaptchaUtils.CaptchaManager.StorageProvider = new CaptchaMvc.Infrastructure.SessionStorageProvider();
            KeyValuePair<string, CaptchaMvc.Interface.ICaptchaValue> keyVal = new KeyValuePair<string, CaptchaMvc.Interface.ICaptchaValue>();
            
            //CaptchaMvc.Infrastructure.CaptchaUtils.CaptchaManager.StorageProvider.Add(new KeyValuePair<string, CaptchaMvc.Interface.ICaptchaValue>);
            return View();
        }

        public ActionResult _PartialCaptcha()
        {
            
                return View();
        }
    }
}
