using RD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Business
{
    /// <summary>
    /// Class to manage courses
    /// </summary>
    public class CourseController
    {
        private static RD.Entities.CourseDAO _dao;

        /// <summary>
        /// Gets the list of courses
        /// </summary>
        /// <returns></returns>
        public static List<Course> GetCourses()
        {
            try
            {
                _dao = new CourseDAO(String.Empty);
                return _dao.GetCourses();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static void SaveCourse(Course course)
        {
            try
            {
                IDAO dao = new CourseDAO(String.Empty, course);
                if (course.Id.Equals(0))
                    dao.Save();
                else
                    dao.Update();
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteCourse(Course course)
        {
try
            {
                IDAO dao = new CourseDAO(String.Empty, course);
                dao.Delete();
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<UserCourse> GetStats()
        {
            UserCourseDAO dao = new UserCourseDAO(string.Empty);
            return dao.GetAll();
        }
    }
}
