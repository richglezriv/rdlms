using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RD.LMS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private const string TRYOUTS = "USER_TRYOUTS";
        List<UserTryout> tryouts;

        protected void Application_Start()
        {
            CaptchaMvc.Infrastructure.CaptchaUtils.CaptchaManager.StorageProvider = new CaptchaMvc.Infrastructure.SessionStorageProvider();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_End(Object sender, EventArgs E)
        {
            tryouts = GetTryouts();
            for (int i = tryouts.Count - 1; i >= 0; i--)
            {
                if (DateTime.Now.Subtract(tryouts[i].lastTry).Minutes >= 60)
                    tryouts.RemoveAt(i);
            }
        }

        private List<UserTryout> GetTryouts()
        {
            return Application[TRYOUTS] == null ? new List<UserTryout>() : (List<UserTryout>)Application[TRYOUTS];
        }
    }
}