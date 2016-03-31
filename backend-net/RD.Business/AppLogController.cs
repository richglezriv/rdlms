using RD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Business
{
   public class AppLogController
   {
      User _user;

      public AppLogController(User user)
      {
         this._user = user;
      }

      public void RegisterLog(AppLogFacade logFacade)
      {
         ApplicationLog log = new ApplicationLog();
         log.LogDate = DateTime.Now;
         log.ClientIpAddress = logFacade.IpAddress;
         log.ErrorCode = logFacade.ErrorCode;
         log.ReturnCode = logFacade.ReturnCode;
         log.SessionId = logFacade.SessionId;
         log.Transaction = logFacade.Transaction;
         log.UserId = this._user.Id;
         AppLogDAO dao = new AppLogDAO(string.Empty, log);
         dao.Save();
      }


   }

   public class AppLogFacade
   {
      public String Transaction { get; set; }
      public String IpAddress { get; set; }
      public String SessionId { get; set; }
      public String ErrorCode { get; set; }
      public String ReturnCode { get; set; }
   }
}
