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
                user = _dao.Validate(login, GetStringHashed(password));
                return user;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static Entities.User GetUserById(int id)
        {
            RD.Entities.User user;

            try
            {
                _dao = new RD.Entities.UserDAO(string.Empty);
                user = _dao.GetBy(id);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entities.User> GetUsers(string nameLike)
        {
            List<Entities.User> result = new List<Entities.User>();
            if (nameLike.Length < 3)
                return result;

            _dao = new RD.Entities.UserDAO(string.Empty);
            result = _dao.GetBy(nameLike);

            return result;

        }

        public static void UpdateSessionState(int userId, SessionState state)
        {
            _dao = new RD.Entities.UserDAO(string.Empty);
            RD.Entities.User user = _dao.GetBy(userId);

            if (user != null)
            {
                user.IsLogged = state.Equals(SessionState.LoggedIn) ? true : false;
                Entities.IDAO control = new RD.Entities.UserDAO(string.Empty, user);
                control.Update();
                
            }
            else
            {
                user = new Entities.User();
                user.Id = userId;
                user.IsLogged = true;
                user.IsAdmin = false;
                Entities.IDAO control = new Entities.UserDAO(string.Empty, user);
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
            int id;
            //validate
            Entities.UserCourse toTest = dao.Get(userCourseId);
            string parentString = toTest.Course.ParentCourses;
            parentString = parentString.Substring(1, parentString.Length - 2);
            string[] parent = parentString.Split(',');

            foreach (string item in parent)
            {
                if (!item.Trim().Equals(String.Empty))
                {
                    id = int.Parse(item.Trim().Replace('"', char.Parse(" ")).ToString());

                    if (dao.GetCourses(toTest.UserId).Single(c => c.CourseId.Equals(id)).Status != "passed")
                        return null;
                }
                
            }

            return toTest;
        }

        public static void UpdateUserCourse(Entities.UserCourse course)
        {
            Entities.IDAO dao = new Entities.UserCourseDAO(string.Empty, course);

            dao.Update();
            
        }

        public static void SaveUser(Entities.User user)
        {
            throw new NotImplementedException();
        }

        public static void Delete(Entities.User user)
        {
            Entities.IDAO dao = new Entities.UserDAO(string.Empty, user);
            dao.Delete();
        }

        private static string GetStringHashed(string toHash)
        {
            System.Security.Cryptography.SHA1 hashKey = System.Security.Cryptography.SHA1.Create();
            hashKey.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            string hashString = BitConverter.ToString(hashKey.Hash).Replace("-", String.Empty).ToLower();

            return hashString;
        }

        public static void UpdateUserPassword(int id, string newPassword)
        {
            Entities.UserDAO dao = new Entities.UserDAO(string.Empty);
            Entities.User user = dao.GetBy(id);
            if (!user.IsAdmin)
                throw new Exception("Error: No es un usuario administrador.");

            try
            {
                user.Password = GetStringHashed(newPassword);
                Entities.IDAO control = new Entities.UserDAO(string.Empty, user);
                control.Update();
            }
            catch (Exception)
            {
                throw new Exception("Error: No se pudo actualiar la contraseña");
            }
            
        }

        
    }
}
