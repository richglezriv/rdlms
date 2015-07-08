using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public interface ResponseModel
    {
        String Status { get; set; }

        String Message { get; set; }

        String GetData { get; set; }

        
        
    }
}