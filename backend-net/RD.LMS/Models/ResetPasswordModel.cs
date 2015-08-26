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
            string[] blackList = new string[] { "password", "contrasena", "senha", "passw0rd","pa55w0rd"
                ,"pas5word"
                ,"pas5w0rd"
                ,"pa5sword"
                ,"pa5sw0rd"
                ,"passw0rd"
                ,"contra5enha"
                ,"c0ntra5enha"
                ,"contra53nha"
                ,"c0ntra53nha"
                ,"c0ntras3nha"
                ,"c0ntrasenha"
                ,"contras3nha"
                ,"c0ntra"
                ,"5enha"
                ,"53nha"
                ,"s3nha" };
            //Regex regex1 = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]){6,20}$");
            //Match match1 = regex1.Match(NewPassword);
            success = false;
            string special = "#?!@$%^&*-_|[{}]+/";
            for (int i = 0; i < special.Length; i++)
            {
                if (NewPassword.Contains(special[i]))
                    success = true;
            }
            if (!NewPassword.Any(char.IsLower))
                success = false;
            if (!NewPassword.Any(char.IsUpper))
                success = false;
            if (!NewPassword.Any(char.IsDigit))
                success = false;
            if (NewPassword.Length < 8)
                success = false;
            success = blackList.Any(b => NewPassword.Trim().ToLower().Contains(b)) ? false : success;

            return success;
        }
    }
}