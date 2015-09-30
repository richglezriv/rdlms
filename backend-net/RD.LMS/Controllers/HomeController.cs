using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string id)
        {
            if (id != null)
            {
                int userId = Convert.ToInt32(id);
                Models.JSonModel model = new Models.JSonModel();
                Models.LMSUser user = new LMSUser();

                model.status = user.BeginSession(userId);

                if (model.status.Equals(Utilities.SUCCESS)){
                    Session.Add(Utilities.USER, user);
                    System.Diagnostics.Debug.WriteLine("sesion id " + Session.SessionID);
                }
                else
                {
                    Session.Clear();
                    Session.Abandon();
                }
                    

            }
            else
            {
                Session.Clear();
                Session.Abandon();
            }

            return View();
        }

        public ActionResult ResetPassword(string ser, string id)
        {
            Models.JSonModel model = new Models.JSonModel();
            if (ser != null && id != null)
            {
                
                string hashString = Business.Utilities.GetSerialHash(id);
                int userId = Convert.ToInt32(id);

                if (!hashString.Equals(ser))
                    model.status = "fail";
                else
                {
                    Models.LMSUser user = new LMSUser();
                    model.status = user.BeginSession(userId);

                    if (model.status.Equals(Utilities.SUCCESS))
                    {
                        model.data = new JSonUserModel()
                        {
                            sessionType = "restore-password",
                            user = user
                        };
                        Session.Add(Utilities.USER, user);
                    }
                }
            }
            

            return View();
        }

        public ActionResult ReestablishPassword(string data)
        {
            Models.LMSUser user = (Models.LMSUser)Session[Utilities.USER];
            Models.JSonModel model = new JSonModel();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            try
            {
                Models.ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                resetPasswordModel.Id = user.id;
                resetPasswordModel.NewPassword = toFetch["password"].ToString();
                if (!resetPasswordModel.NewPassword.Equals(toFetch["passwordCheck"].ToString())){
                    user = new LMSUser();
                    user.fields = new Dictionary<string, string>();
                    user.fields.Add("passwordCheck","Las contraseñas deben coincidir");
                    user.SetReason("validation-error");
                    throw new Exception("Error: Las contraseñas deben coincidir");
                }

                if (!resetPasswordModel.IsStrongPassword())
                {
                    user = new LMSUser();
                    user.fields = new Dictionary<string, string>();
                    user.fields.Add("password", "La contraseña no cumple con los lineamientos especificados");
                    user.SetReason("validation-error");
                    throw new Exception("La contraseña no cumple con los lineamientos especificados");
                }
                resetPasswordModel.UpdatePassword();
                
            }
            catch (Exception)
            {
                model.status = "fail";
                model.data = user;
            }

            return Json(model);
        }

        public ActionResult Profile(FormCollection fc)
        {
            Models.JSonModel model = new Models.JSonModel();

            if (Session[Utilities.USER] != null)
            {
                model.data = (Models.LMSUser)Session[Utilities.USER];
            }

            if (fc.Keys.Count > 0)
            {
                Models.LMSUser user = (Models.LMSUser)Session[Utilities.USER];
                if (!fc["input-old-password"].Equals(user.password))
                    user.SetReason("La contraseña actual no coincide");
                else
                {
                    Models.ResetPasswordModel resetPass = new ResetPasswordModel();
                    resetPass.Id = user.id;
                    try
                    {
                        resetPass.NewPassword = fc["input-password"].ToString();
                        if (!resetPass.NewPassword.Equals(fc["input-retype-password"].ToString()))
                            user.SetReason("Error: Las contraseñas deben coincidir");
                        else
                        {
                            resetPass.UpdatePassword();
                            user.SetReason(resetPass.Message);
                        }
                    }
                    catch (Exception)
                    {
                        user.SetReason("No se pudo actualizar la contraseña");
                    }
                }

                model.data = user;
                Session[Utilities.USER] = user;
            }

            return View(model);
        }

        public ActionResult ForgotPassword(string data)
        {
            JSonModel model = new JSonModel();
            LMSUser user = new LMSUser();
            Newtonsoft.Json.Linq.JObject toFetch = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            user.RestablishPassword(toFetch["username"].ToString());

            return Json(model);
        }
    }
}
