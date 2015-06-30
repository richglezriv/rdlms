using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    internal class ScormDAO : IDAO
    {
        private String _password;
        private Scorm _persist;

        public Scorm GetSaved
        {
            get { return this._persist; }
        }

        public ScormDAO(string contextPassword)
        {
                this._password = contextPassword;
        }

        public ScormDAO(string contextPassword, Entities.Scorm toPersist)
        {
            this._persist = toPersist;
            this._password = contextPassword;
        }

        void IDAO.Save()
        {

            if (this._persist != null)
            {
                RDModelContainer model = Context.SetContext(_password).model;
                model.Scorms.Add(this._persist);
                model.SaveChanges();
            }

        }

        void IDAO.Update()
        {
            throw new NotImplementedException();
        }

        void IDAO.Delete()
        {
            throw new NotImplementedException();
        }
    }
}
