using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class ExternalController : Controller
    {
        //
        // GET: /External/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginUser(string data)
        {
            Models.JSonModel model = new Models.JSonModel();

            try
            {
                const String KEY = "ZaZaCruREcHA8a7*as";
                Newtonsoft.Json.Linq.JObject json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(Request.QueryString[0].ToString());
                string userId = json["data"]["userId"].ToString();
                string serialKey = String.Format("{0}{1}", userId, KEY);
                System.Security.Cryptography.SHA512 hashKey = System.Security.Cryptography.SHA512.Create();
                hashKey.ComputeHash(Encoding.UTF8.GetBytes(serialKey));
                string hashString = BitConverter.ToString(hashKey.Hash).Replace("-", String.Empty).ToLower();

                if (!json["data"]["key"].ToString().Equals(hashString)){
                    model.status = "fail";
                    model.data = new Models.MessageData() { reason = "invalid key" };
                }
                else
                {
                    model = SaveExternalUSer(json);
                }
            }
            catch (Exception ex)
            {
                model.status = "fail";
                model.data = new Models.MessageData() { reason = ex.Message };
            }


            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private Models.JSonModel SaveExternalUSer(Newtonsoft.Json.Linq.JObject json)
        {
            Models.JSonModel model = new Models.JSonModel();

            try
            {
                int userId = Convert.ToInt32(json["data"]["userId"].ToString());
                Business.UserController.UpdateSessionState(userId, Business.UserController.SessionState.LoggedIn);
            }
            catch (Exception ex)
            {
                model.status = "fail: " + ex.Message;
            }

            return model;

        }
    }
}
