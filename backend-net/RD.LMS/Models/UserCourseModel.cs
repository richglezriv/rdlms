using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class UserCourseModel : IDataModel
    {
        #region declarations
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
        #endregion

        #region methods
        public static List<UserCourseModel> Get(string userId)
        {
            List<RD.Entities.UserCourse> list = RD.Business.UserController.GetUserCourses(Convert.ToInt32(userId), true);
            List<UserCourseModel> result = new List<UserCourseModel>();
            
            //loads user courses
            foreach (RD.Entities.UserCourse item in list)
            {
                result.Add(new UserCourseModel()
                {
                    active = true,
                    description = item.Course.Description,
                    name = item.Course.Name,
                    thumbnail = item.Course.Thumbnail,
                    totalTime = item.Scorm.SessionTime,
                    status = item.Status,
                    id = item.Id.ToString()
                });
            }

            return result;

        }

        #endregion
    }
}