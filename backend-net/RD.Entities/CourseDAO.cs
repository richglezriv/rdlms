using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    public class CourseDAO : IDAO
    {
        private Course _course;
        private String _password;

        public CourseDAO(string contextPassword)
        {
            this._password = contextPassword;
        }

        public CourseDAO(string contextPassword, Course toPersist)
        {
            this._password = contextPassword;
            this._course = toPersist;
        }

        public List<Course> GetCourses()
        {
            RDModelContainer model = Context.SetContext(_password).model;
            return model.Courses.Where(c=>c.IsEnabled == true).ToList();

        }


        void IDAO.Save()
        {
            if (this._course == null)
                return;

            RDModelContainer model = Context.SetContext(_password).model;
            model.Courses.Add(this._course);
            model.SaveChanges();
        }

        void IDAO.Update()
        {
            if (this._course == null)
                return;

            RDModelContainer model = Context.SetContext(_password).model;
            Course update = model.Courses.Single(c => c.Id.Equals(this._course.Id));
            update.Description = _course.Description;
            update.Name = _course.Name;
            update.ParentCourses = _course.ParentCourses;
            update.ScoIndex = _course.ScoIndex == null ? update.ScoIndex :  _course.ScoIndex;
            update.ScormPackage = _course.ScormPackage;
            update.Thumbnail = _course.Thumbnail;
            update.IsEnabled = _course.IsEnabled;

            model.SaveChanges();
        }

        void IDAO.Delete()
        {
            if (this._course == null)
                return;

            RDModelContainer model = Context.SetContext(_password).model;
            Course toDel = model.Courses.SingleOrDefault(c => c.Id.Equals(this._course.Id));
            model.Courses.Remove(toDel);
            model.SaveChanges();
        }
    }
}
