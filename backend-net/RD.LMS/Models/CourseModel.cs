using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class CourseModel : IDataModel
    {
        public String id { get; set; }

        public String name { get; set; }

        public String scoPath { get; set; }

        public String scoIndex { get; set; }

        public String status { get; set; }

        public String dataModel { get; set; }
        
        public String thumbnail { get; set; }

        public String description { get; set; }

        
    }
}