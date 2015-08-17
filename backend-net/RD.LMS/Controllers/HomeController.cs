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

                if (model.status.Equals(Utilities.SUCCESS))
                    Session.Add(Utilities.USER, user);
                else
                {
                    Session.Clear();
                    Session.Abandon();
                }
                    

            }


            return View();
        }

        public ActionResult ResetPassword(FormCollection fc)
        {
            Models.ResetPasswordModel model = new ResetPasswordModel();
            string serial = Request.QueryString["ser"] != null ? Request.QueryString["ser"].ToString(): null;
            model.Id = Request.QueryString["id"] != null ? Request.QueryString["id"] : null;
            const String KEY = "ZaZaCruREcHA8a7*as";
            if (serial != null && model.Id != null)
            {
                System.Security.Cryptography.SHA1 hashKey = System.Security.Cryptography.SHA1.Create();
                hashKey.ComputeHash(Encoding.UTF8.GetBytes(KEY));
                string hashString = BitConverter.ToString(hashKey.Hash).Replace("-", String.Empty).ToLower();
                model.Message = String.Empty;

                if (!hashString.Equals(serial))
                    model.Message = "Denegado: Acceso invalido";

                try
                {
                    if (fc.Keys.Count > 0)
                    {
                        model.NewPassword = fc["input-password"].ToString();
                        if (!model.NewPassword.Equals(fc["input-retype-password"].ToString()))
                            throw new Exception("Error: Las contraseñas deben coincidir");
                        model.UpdatePassword();
                    }
                }
                catch (Exception ex)
                {
                    model.Message = ex.Message;
                }
                

            }
            else
                model.Message = "Denegado: Acceso invalido";
                
            
            return View(model);
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
    }
}
