using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    /// <summary>
    /// Json Model class
    /// </summary>
    public class JSonModelCollection
    {
        public String message { get; set; }
        public List<IDataModel> data { get; set; }

        public JSonModelCollection()
        {

        }

    }

}