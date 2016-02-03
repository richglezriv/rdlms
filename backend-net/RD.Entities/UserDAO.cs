using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    public class UserDAO : IDAO
    {
        private String _password;
        private User _persist;

        public UserDAO(string contextPassword)
        {
                this._password = contextPassword;
        }

        public UserDAO(string contextPassword, Entities.User toPersist) {
            this._persist = toPersist;
            this._password = contextPassword;
        }

        /// <summary>
        /// Searches for the user, returns null if doesn't exists
        /// </summary>
        /// <param name="login">user login</param>
        /// <param name="password">SHA5 password</param>
        /// <returns></returns>
        public User Validate(String login, String password)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            if (!model.Users.Any(u => u.Login.Equals(login)))
                return null;

            var user = model.Users.Single(u => u.Login.Equals(login));

            if (user.Password.Equals(password))
                return user;
            else
                return null;
        }

        public User ValidateByMail(string email, string password)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            if (!model.Users.Any(u => u.Email.Equals(email)))
                return null;

            var user = model.Users.Single(u => u.Email.Equals(email));

            if (user.Password.Equals(password))
                return user;
            else
                return null;

        }

        public User GetBy(int id)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            return model.Users.SingleOrDefault(u => u.Id.Equals(id));

        }

        public List<User> GetBy(String mailLike)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            List<User> result = model.Users.Where(u => u.Email.Contains(mailLike)).ToList();

            return result;
        }

        public User GetByMail(String email)
        {
            RDModelContainer model = Context.SetContext(_password).model;
            User result = model.Users.SingleOrDefault(u => u.Email.Equals(email));

            return result;
        }

        void IDAO.Save()
        {
            RDModelContainer model = Context.SetContext(_password).model;
            model.Users.Add(_persist);
            model.SaveChanges();
        }

        void IDAO.Update()
        {
            RDModelContainer model = Context.SetContext(_password).model;
            User result = model.Users.Single(u => u.Id.Equals(_persist.Id));
            result.FirstName = _persist.FirstName;
            result.IsAdmin = _persist.IsAdmin;
            result.IsLogged = _persist.IsLogged;
            result.LastName = _persist.LastName;
            result.Login = _persist.Login;
            result.Password = _persist.Password;
            result.SecondLastName = _persist.SecondLastName;
            result.BirthDay = _persist.BirthDay;
            result.Email = _persist.Email;
            result.Gender = _persist.Gender;
            result.Ocupation = _persist.Ocupation;
            result.Organization = _persist.Organization;
            
            model.SaveChanges();
        }

        void IDAO.Delete()
        {
            RDModelContainer model = Context.SetContext(_password).model;
            User result = model.Users.Single(u => u.Id.Equals(_persist.Id));

            foreach (UserCourse item in result.UserCourses)
            {
                model.Scorms.Remove(item.Scorm);
            }
            model.UserCourses.RemoveRange(result.UserCourses);
            model.Users.Remove(result);
            model.SaveChanges();
        }
    }
}
