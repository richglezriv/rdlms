using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class CourseStatsModel : IDataModel
    {
        public string id { get; set; }

        public int passed { get; set; }

        public int failed { get; set; }

        public int incomplete { get; set; }

        public int notAttempted { get; set; }

        public string name { get; set; }

        public int completed { get; set; }
    }
}