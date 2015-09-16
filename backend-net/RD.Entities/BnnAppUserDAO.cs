using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    public class BnnAppUserDAO
    {
        private String _password;
        private User _persist;

        public BnnAppUserDAO(string contextPassword)
        {
                this._password = contextPassword;
        }

        public BnnAppUser GetBy(int id)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            BnnAppUser user = model.BnnAppUsers.SingleOrDefault(u => u.Id.Equals(id));
            return user;
        }
    }
}
