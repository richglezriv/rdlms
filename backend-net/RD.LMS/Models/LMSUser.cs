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

        public Boolean isAdmin { get; set; }

        public int TryOuts { get; private set; }

        public string reason { get; private set; }
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

            this.isAdmin = daoUser.IsAdmin;
            this.name = daoUser.FirstName;
            this.id = daoUser.Id.ToString();
            this.TryOuts += 1;

            return "success";

                
        }

        internal String Validate()
        {
            RD.Entities.User daoUser = RD.Business.UserController.GetUser(login, password);

            if (daoUser == null)
            {
                this.reason = "credentials-error";
                return "fail";
            }

            this.isAdmin = daoUser.IsAdmin;
            this.name = daoUser.FirstName;
            this.id = daoUser.Id.ToString();
            this.TryOuts += 1;
            this.resetPassword = daoUser.IsAdmin && Utilities.MonthDiff(daoUser.LastLogged.Value, DateTime.Now.Date) > 3;

            return "success";
        }

        internal void SetReason()
        {
            this.reason = "too-many-tries";
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
        #endregion
    }
}