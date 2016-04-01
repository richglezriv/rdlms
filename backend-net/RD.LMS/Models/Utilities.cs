﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace RD.LMS.Models
{
   public class Utilities
   {
      public static readonly string THUMBS = "THUMBS";
      public static readonly string SCORMS = "SCORMS";
      public static readonly String USER = "USER_LMS";
      public static readonly String FAIL = "fail";
      public static readonly String SUCCESS = "success";
      public static readonly String SESSION_EXPIRED = "session-expired";

      public static int MonthDiff(DateTime dateB, DateTime dateE)
      {

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

      public static JsonResult StateLoggedOut()
      {
         JsonResult result = new JsonResult();
         Models.JSonModel model = new JSonModel();
         model.status = "success";
         model.data = JSonUserModel.GetLoggedOut();
         result.Data = model;
         return result;
      }

      public static JsonResult StateSessionExpired()
      {
         JsonResult result = new JsonResult();
         Models.JSonModel model = new JSonModel();
         model.status = "fail";
         model.data = MessageData.GetSessionExpired();
         result.Data = model;
         HttpContext.Current.Session.Clear();
         HttpContext.Current.Session.Abandon();
         CreateSessionId();
         return result;
      }

      public static Boolean IsValidToken(string token, LMSUser user)
      {
         if (token == null || user == null)
            return false;

         return user.csrftoken.Equals(token);

      }

      public static void CreateSessionId()
      {
         HttpContext context = System.Web.HttpContext.Current;
         Boolean redirected, cookieAdded;
         SessionIDManager manager = new SessionIDManager();
         string newId = manager.CreateSessionID(context);
         manager.RemoveSessionID(context);
         manager.SaveSessionID(context, newId, out redirected, out cookieAdded);
      }

      public static void LogTransaction(string transaction, string errorCode, string returnCode)
      {
         try
         {
            HttpContext context = HttpContext.Current;
            LMSUser lmsUser = context.Session[USER] as LMSUser;
            Business.AppLogFacade facade = new Business.AppLogFacade();
            facade.ErrorCode = errorCode;
            facade.IpAddress = context.Request.UserHostAddress;
            facade.ReturnCode = returnCode;
            facade.SessionId = context.Session.SessionID;
            facade.Transaction = transaction;
            Entities.User user = new Entities.User();
            user.Id = lmsUser != null ? Convert.ToInt32(lmsUser.id) : 0;
            Business.AppLogController control = new Business.AppLogController(user);
            control.RegisterLog(facade);
         }
         catch (Exception) { }
      }
   }

   
}