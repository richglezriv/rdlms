using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class UserCourseModel : IDataModel
    {
        #region properties
        public string id
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public String description { get; set; }

        public String thumbnail { get; set; }

        public String status { get; set; }

        public String totalTime { get; set; }

        public Boolean active { get; set; }

        public string score { get; set; }
        #endregion

        #region methods
        public static List<UserCourseModel> Get(string userId)
        {
            List<RD.Entities.UserCourse> list = RD.Business.UserController.GetUserCourses(Convert.ToInt32(userId), true);
            List<UserCourseModel> result = new List<UserCourseModel>();
            string[] status = new String[] { "passed", "completed" };

            //loads user courses
            foreach (RD.Entities.UserCourse item in list)
            {
                result.Add(new UserCourseModel()
                {
                    active = RD.Business.UserController.GetCourse(item.Id) != null,
                    description = item.Course.Description,
                    name = item.Course.Name,
                    thumbnail = item.Course.Thumbnail,
                    totalTime = item.Scorm.SessionTime,
                    status = item.Status,
                    id = item.Id.ToString(), 
                    score = item.Scorm.ScoreRaw
                });
            }

            return result;

        }

        internal void ResetCourse(string userId, string courseId)
        {
            List<RD.Entities.UserCourse> list = RD.Business.UserController.GetUserCourses(Convert.ToInt32(userId), false);
            RD.Entities.UserCourse course = list.Single(c => c.Id.Equals(Convert.ToInt32(courseId)));
            course.Status = "not attempted";
            course.Scorm.ScoreRaw = "0";
            course.Scorm.SessionTime = "0:00:00";
            course.Scorm.Credit = string.Empty;
            course.Scorm.DataMasteryScore = string.Empty;
            course.Scorm.Entry = string.Empty;
            course.Scorm.Exit = string.Empty;
            course.Scorm.LaunchData = string.Empty;
            course.Scorm.LessonLocation = string.Empty;
            course.Scorm.TotalTime = "0:00:00";
            Business.UserController.UpdateUserCourse(course);
        }
        #endregion


        
    }
}