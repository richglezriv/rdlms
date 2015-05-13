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
        public String message { get; set; }
        public IDataModel data { get; set; }

        public JSonModel()
        {

        }

    }

}