using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Business
{
    /// <summary>
    /// Class to manage users and users courses
    /// </summary>
    public class UserController
    {
        public enum SessionState { LoggedIn = 0, LoggedOut = 1 }
        private static RD.Entities.UserDAO _dao;

        public static RD.Entities.User GetUser(string login, string password)
        {
            RD.Entities.User user;

            try
            {
                _dao = new RD.Entities.UserDAO(string.Empty);
                user = _dao.Validate(login, password);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static void UpdateSessionState(int userId, SessionState state)
        {
            _dao = new RD.Entities.UserDAO(string.Empty);
            RD.Entities.User user = _dao.GetBy(userId);

            if (user != null)
            {
                user.IsLogged = state.Equals(SessionState.LoggedIn) ? true : false;
                Entities.IDAO control = new RD.Entities.UserDAO(string.Empty, user);
                control.Save();
                
            }
        }

        /// <summary>
        /// Gets all the user courses
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <param name="updateList">if updates with new courses loaded</param>
        /// <returns></returns>
        public static List<Entities.UserCourse> GetUserCourses(int userId, Boolean updateList)
        {
            RD.Entities.UserCourseDAO dao = new Entities.UserCourseDAO(string.Empty);
            List<Entities.UserCourse> courses = dao.GetCourses(userId);
            
            if (!updateList)
                return courses;

            AddCoursesToUser(userId, courses);

            return dao.GetCourses(userId);

        }

        private static void AddCoursesToUser(int userId, List<Entities.UserCourse> userCourses)
        {
            List<Int32> ids = new List<int>();
            foreach (Entities.UserCourse item in userCourses)
            {
                ids.Add(item.CourseId);
            }

            List<RD.Entities.Course> notInList = CourseController.GetCourses().FindAll(c => !ids.Contains(c.Id));
            ids = new List<int>();
            foreach (Entities.Course item in notInList)
            {
                ids.Add(item.Id);
            }


            Entities.IDAO dao;
            RD.Entities.UserCourse userCourse;

            foreach (Int32 item in ids)
            {
                userCourse = new Entities.UserCourse()
                {
                    CourseId = item,
                    UserId = userId,
                    Status = "not attempted"
                };

                dao = new Entities.UserCourseDAO(string.Empty, userCourse);
                dao.Save();
                
            }
        }

        public static Entities.UserCourse GetCourse(int userCourseId)
        {
            Entities.UserCourseDAO dao = new Entities.UserCourseDAO(string.Empty);

            return dao.Get(userCourseId);
        }
    }
}
