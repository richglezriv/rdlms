using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class Utilities
    {
        public static readonly string THUMBS = "THUMBS";
        public static readonly string SCORMS = "SCORMS";
        public static readonly String USER = "USER_LMS";
        public static readonly String FAIL = "fail";
        public static readonly String SUCCESS = "success";

        public static int MonthDiff(DateTime dateB, DateTime dateE){

            if (dateB.Year > dateE.Year || dateB > dateE)
                return 0;
            else if (dateB.Year.Equals(dateE.Year))
                return dateE.Month - dateB.Month;
            else if (dateB.Year < dateE.Year)
            {
                return 12 - dateB.Month + dateE.Month;
            }
            
            return 0;
        }

        public static JSonModel IsSessionActive(Object session)
        {
            if (session == null)
                return new JSonModel() { status = "fail", data = new MessageData("session-expired") };
            //else if (((LMSUser)session).isAdmin)
            //    return new JSonModel() { status = "fail", data = new MessageData("admins-only") };
            //else if (!((LMSUser)session).isAdmin)
            //    return new JSonModel() { status = "fail", data = new MessageData("users-only") };

            return null;
        }


    }
}