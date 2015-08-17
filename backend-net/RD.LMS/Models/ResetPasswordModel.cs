using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RD.LMS.Models
{
    public class ResetPasswordModel
    {
        public String Id { get; set; }
        public String NewPassword { get; set; }
        public String Message { get; set; }

        internal void UpdatePassword()
        {
            if (!IsStrongPassword())
            {
                Message = "La contraseña no es segura, intente nuevamente";
                return;
            }
            Business.UserController.UpdateUserPassword(Convert.ToInt32(this.Id), this.NewPassword);
            Message = "Contraseña actualizada.";
        }

        private Boolean IsStrongPassword()
        {
            Boolean success = true;
            string[] blackList = new string[] { "password", "contrasena", "senha" };
            //Regex regex1 = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]){6,20}$");
            //Regex regex2 = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*?[#?!@$%^&*-]){6,20}$");
            //Regex regex3 = new Regex("^(?=.*[0-9])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]){6,20}$");
            //Regex regex4 = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]){6,20}$");
            //Regex regex5 = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]){6,20}$");

            //Match match1 = regex1.Match(NewPassword);
            //Match match2 = regex2.Match(NewPassword);
            //Match match3 = regex3.Match(NewPassword);
            //Match match4 = regex4.Match(NewPassword);
            //Match match5 = regex5.Match(NewPassword);
            if (!NewPassword.Any(char.IsLower))
                success = false;
            if (!NewPassword.Any(char.IsUpper))
                success = false;
            if (!NewPassword.Any(char.IsDigit))
                success = false;
            if (NewPassword.Length < 6)
                success = false;
            if (NewPassword.Trim().ToLower().Contains(blackList[0]) ||
                NewPassword.Trim().ToLower().Contains(blackList[1]) ||
                NewPassword.Trim().ToLower().Contains(blackList[2]))
                success = false;

            return success;
        }
    }
}