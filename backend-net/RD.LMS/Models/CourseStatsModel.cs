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

        public CourseStatsModel() { }

        public CourseStatsModel(string courseId) {
            this.id = courseId;
            Entities.Course course = Business.CourseController.GetBy(int.Parse(id));
            this.name = course.Name;
            SetStats();
        }

        private void SetStats()
        {
            passed = Business.CourseController.GetByStatus("passed", int.Parse(this.id));
            failed = Business.CourseController.GetByStatus("failed", int.Parse(this.id));
            incomplete = Business.CourseController.GetByStatus("incomplete", int.Parse(this.id));
            notAttempted = Business.CourseController.GetByStatus("not attempted", int.Parse(this.id));
            completed = Business.CourseController.GetByStatus("completed", int.Parse(this.id));
        }
    }
}