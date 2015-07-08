using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public interface IDataModel
    {
        String id { get; set; }

        String name { get; set; }
    }
}