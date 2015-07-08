using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    internal class Context
    {
        public RDModelContainer model;
        private static Context context;

        public static Context SetContext(String password)
        {
            if (context == null)
                context = new Context(password);

            return context;
        }

        private Context(String password)
        {
            if (this.model == null)
                this.model = new RDModelContainer();
        }

    }
}
