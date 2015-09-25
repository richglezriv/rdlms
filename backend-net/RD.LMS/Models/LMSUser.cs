﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class LMSUser : IDataModel
    {
        #region properties
        public Boolean resetPassword { get; set; }
        
        public string id
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public String login { get; set; }

        public string password { get; set; }

        public string newPassword { get; set; }

        public string newPasswordCheck { get; set; }

        public string email { get; set; }

        public Boolean isAdmin { get; set; }

        public int TryOuts { get; set; }

        public string reason { get; private set; }

        public string lastName { get; set; }

        public string secondLastName { get; set; }

        public string birthday { get; set; }

        public string gender { get; set; }

        public short occupation { get; set; }

        public short organization { get; set; }

        public IDictionary<string, string> fields { get; set; }
        #endregion

        #region constructor
        public LMSUser()
        {
            this.TryOuts = 0;
        }
        #endregion

        #region methods
        internal string BeginSession(int id)
        {
            RD.Entities.User daoUser = RD.Business.UserController.GetUserById(id);
            if (daoUser == null || !daoUser.IsLogged)
            {
                this.reason = "credentials-error";
                return "fail";
            }
            this.TryOuts += 1;
            SetUser(daoUser);
            
            return "success";
        }

        private void SetUser(RD.Entities.User daoUser)
        {
            const String FORMAT_DATE = "yyyy-MM-dd";
            this.isAdmin = daoUser.IsAdmin;
            this.name = daoUser.FirstName;
            this.id = daoUser.Id.ToString();
            this.lastName = daoUser.LastName;
            this.secondLastName = daoUser.SecondLastName;
            this.email = daoUser.Email;
            this.birthday = daoUser.BirthDay.HasValue ? daoUser.BirthDay.Value.ToString(FORMAT_DATE) : string.Empty;
            this.gender = daoUser.Gender;
            this.occupation = daoUser.Ocupation;
            this.organization = daoUser.Organization;
        }

        internal String Validate()
        {
            RD.Entities.User daoUser = RD.Business.UserController.GetUser(null, password, email);
            this.TryOuts += 1;

            if (daoUser == null)
            {
                this.reason = "credentials-error";
                return "fail";
            }

            if (!daoUser.IsActive)
            {
                this.reason = "user-inactive";
                this.fields = new System.Collections.Generic.Dictionary<String, String>();
                this.fields.Add("email", "Usuario no activo");
                return "fail";
            }

            this.resetPassword = daoUser.LastLogged != null ?
                daoUser.IsAdmin && Utilities.MonthDiff(daoUser.LastLogged.Value, DateTime.Now.Date) > 3 : false;
            SetUser(daoUser);
            
            return "success";
        }

        internal void SetReason()
        {
            this.reason = "too-many-tries";
        }

        internal void SetReason(string reason)
        {
            this.reason = reason;
        }

        internal String GetDataResponse()
        {
            String response = "{\"status\":\"success\", \"data\":{\"isAdmin\":\"" + this.isAdmin.ToString() + "\"}}";

            return response;
        }

        internal void LogOut()
        {
            RD.Business.UserController.UpdateSessionState(Convert.ToInt32(this.id), Business.UserController.SessionState.LoggedOut);
        }

        internal string GetSessionType()
        {
            switch (this.isAdmin)
            {
                case true:
                    return "admin";
                case false:
                    return "student";
                default:
                    break;
            }

            return "restore-password";
        }

        internal void UpdateUser()
        {
            Entities.User user = Business.UserController.GetUserById(Convert.ToInt32(this.id));
            user.Ocupation = this.occupation;
            user.Organization = this.organization;
            user.Gender = this.gender;
            string[] date = this.birthday.Split(Char.Parse("-"));
            user.BirthDay = new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]));

            Business.UserController.SaveUser(user);

            if (this.password != String.Empty)
            {
                if (Business.UserController.GetUser(null, this.password, user.Email) != null)
                {
                    if (this.newPassword.Equals(this.newPasswordCheck))
                    {
                        ResetPasswordModel pwdModel = new ResetPasswordModel();
                        pwdModel.Id = this.id;
                        pwdModel.NewPassword = this.newPassword;
                        pwdModel.UpdatePassword();
                    }
                }
            }
        }

        internal void Register()
        {
            string[] bDay = this.birthday.Split(char.Parse("-"));
            Entities.User newUSer = new Entities.User()
            {
                BirthDay = new DateTime(int.Parse(bDay[0]), int.Parse(bDay[1]), int.Parse(bDay[2])),
                Email = this.email,
                FirstName = this.name,
                Gender = this.gender,
                IsAdmin = false,
                IsLogged = false,
                LastLogged = new DateTime(1899, 11, 30),
                LastName = this.lastName,
                Ocupation = this.occupation,
                Organization = this.organization,
                Password = this.password,
                SecondLastName = this.secondLastName
            };
            try
            {
                ResetPasswordModel passwordModel = new ResetPasswordModel()
                {
                    NewPassword = this.password
                };
                if (!passwordModel.IsStrongPassword())
                {
                    this.fields = new Dictionary<string, string>();
                    this.fields.Add("password", "La contraseña no cumple con los lineamientos especificados");
                    throw new Exception("La contraseña no cumple con los lineamientos especificados");
                }

                if (Business.UserController.GetUserByMail(this.email) != null)
                {
                    this.fields = new Dictionary<string, string>();
                    this.fields.Add("email", "El email especificado ya existe");
                    throw new Exception("El email especificado ya existe");
                }
                    

                Business.UserController.SaveUser(newUSer);
                Business.NotificationController notify = new Business.NotificationController();
                notify.SendConfirmationMail(newUSer);
            }
            catch (Exception)
            {
                this.reason = "validation-error";
                throw;
            }
        }

        internal void RestablishPassword(string userEmail)
        {
            try
            {
                Entities.User user = Business.UserController.GetUserByMail(userEmail);
                if (user != null)
                {
                    Business.NotificationController notification = new Business.NotificationController();
                    notification.SendPasswordRecoveryMail(user);
                }
            }
            catch (Exception) { }

        }
        #endregion
    }

    public struct UserTryout
    {
        public String login { get; set; }
        public DateTime lastTry { get; set; }
    }
}