﻿using System;
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

        public User GetBy(int id)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            return model.Users.SingleOrDefault(u => u.Id.Equals(id));

        }

        public List<User> GetBy(String nameLike)
        {
            RDModelContainer model = Context.SetContext(_password).model;

            return model.Users.Where(u => u.FirstName.Contains(nameLike) || u.LastName.Contains(nameLike)).ToList<User>();
        }

        void IDAO.Save()
        {
            throw new NotImplementedException();
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
