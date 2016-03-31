using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
   public class AppLogDAO : IDAO
   {
      private ApplicationLog _log;
      private String _password;

      public AppLogDAO(string contextPassword)
      {
         this._password = contextPassword;
      }

      public AppLogDAO(string contextPassword, ApplicationLog log)
      {
         this._password = contextPassword;
         this._log = log;
      }

      public void Delete()
      {
         throw new NotImplementedException();
      }

      public void Save()
      {
         if (this._log == null)
            return;

         RDModelContainer model = Context.SetContext(_password).model;
         model.ApplicationLogs.Add(this._log);
         model.SaveChanges();
      }

      public void Update()
      {
         throw new NotImplementedException();
      }
   }
}
