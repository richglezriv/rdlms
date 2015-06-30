using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    /// <summary>
    /// Class to manage persitance to user courses
    /// </summary>
    public class UserCourseDAO : IDAO
    {
        #region declarations
        private String _password;
        private UserCourse _persist;
        #endregion

        #region constructor
        /// <summary>
        /// Default constructor to set the context
        /// </summary>
        /// <param name="password"></param>
        public UserCourseDAO(string contextPassword)
        {
            this._password = contextPassword;
        }

        public UserCourseDAO(string contextPassword, Entities.UserCourse toPersist)
        {
            this._persist = toPersist;
            this._password = contextPassword;
        }
        #endregion

        #region methods
        public List<UserCourse> GetCourses(int userId)
        {
            RDModelContainer model = Context.SetContext(_password).model;
            return model.UserCourses.Where(c=>c.UserId.Equals(userId) && c.Course.IsEnabled == true).ToList<UserCourse>();
        }

        public UserCourse Get(int userCourseId)
        {
            RDModelContainer model = Context.SetContext(_password).model;
            return model.UserCourses.Single<UserCourse>(c=> c.Id.Equals(userCourseId));
        }
        #endregion

        void IDAO.Save()
        {

            Scorm scorm = new Scorm() { TotalTime = "0:00:00", ScoreRaw = "0" };
            IDAO daoScorm = new ScormDAO(String.Empty, scorm);
            daoScorm.Save();

            this._persist.ScormId = scorm.Id;

            RDModelContainer model = Context.SetContext(_password).model;
            model.UserCourses.Add(this._persist);
            model.SaveChanges();
            
        }

        void IDAO.Update()
        {
            throw new NotImplementedException();
        }

        void IDAO.Delete()
        {
            throw new NotImplementedException();
        }

        public List<UserCourse> GetAll()
        {
            RDModelContainer model = Context.SetContext(_password).model;
            return model.UserCourses.ToList<UserCourse>();
        }
    }
}
