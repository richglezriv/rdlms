using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class LMSUser : IDataModel
    {
        #region declarations
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