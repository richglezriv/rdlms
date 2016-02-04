using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    /// <summary>
    /// Json Model class
    /// </summary>
    public class JSonModel
    {
        public String status { get; set; }
        public IDataModel data { get; set; }

        public JSonModel()
        {
            status = "success";
        }

    }

    public class MessageData : IDataModel
    {
        public MessageData() { }

        public MessageData(string message)
        {
            this.message = message;
            this.reason = message;
        }

        public string id { get; set; }

        public string name { get; set; }

        public string message { get; set; }

        public String reason { get; set; }

        public static MessageData GetSessionExpired()
        {
            MessageData model = new MessageData()
            {
                reason = "session-expired",
            };
            return model;
        }
    }

    public class JSonUserModel : IDataModel
    {
        public string sessionType { get; set; }

        public IDataModel user { get; set; }

        public string id
            { get; set; }

        public string name
           { get; set; }

        public static JSonUserModel GetLoggedOut()
        {
            JSonUserModel model = new JSonUserModel()
            {
                sessionType = "logged-out",
                user = null
            };
            return model;
        }

    }

}