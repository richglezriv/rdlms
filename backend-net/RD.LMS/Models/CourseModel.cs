using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    [Serializable]
    public class CourseModel : IDataModel
    {

        #region properties
        public String id { get; set; }

        public String name { get; set; }

        public String scorm { get; set; }

        public String scoPath { get; set; }

        public String scoIndex { get; set; }

        public String status { get; set; }

        public String dataModel { get; set; }
        
        public String thumbnail { get; set; }

        public String description { get; set; }

        public String conditions { get; set; }
        #endregion

        #region methods
        public void LoadValues(Newtonsoft.Json.Linq.JObject json)
        {
            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                if (p.Name.Equals("id") && json["courseId"] != null)
                    this.GetType().GetProperty(p.Name).SetValue(this,json["courseId"].ToString());
                else
                    this.GetType().GetProperty(p.Name).SetValue(this, json[p.Name] != null ? json[p.Name].ToString() : null);
            }

        }
        /// <summary>
        /// Gets the courses to admin
        /// </summary>
        /// <returns></returns>
        public static List<CourseModel> GetCourses()
        {
            List<CourseModel> result = new List<CourseModel>();
            try
            {
                List<RD.Entities.Course> list = RD.Business.CourseController.GetCourses();
                foreach (RD.Entities.Course item in list)
                {
                    result.Add(new CourseModel()
                    {
                        id = item.Id.ToString(),
                        description = item.Description,
                        name = item.Name,
                        scoIndex = item.ScoIndex,
                        scoPath = item.ScormPackage,
                        scorm = item.ScormPackage,
                        status = "active",
                        thumbnail = item.Thumbnail,
                        conditions = item.ParentCourses
                    });
                }
            }
            catch (Exception) { }
            
            return result;
        }

        public void LoadCourse(Entities.UserCourse userCourse)
        {
            this.conditions = userCourse.Course.ParentCourses;
            this.description = userCourse.Course.Description;
            this.id = userCourse.Id.ToString();
            this.name = userCourse.Course.Name;
            this.scoIndex = userCourse.Course.ScoIndex;
            this.scoPath = userCourse.Course.ScormPackage.Remove(userCourse.Course.ScormPackage.Length - 4, 4);
            this.scorm = userCourse.Course.ScormPackage;
            this.status = userCourse.Status;
            this.thumbnail = userCourse.Course.Thumbnail;
            
            
        }
        #endregion

        internal void Save()
        {
            Int32 id = 0;
            if (!int.TryParse(this.id, out id))
                id = 0;
            try
            {
                Business.CourseController.SaveCourse(new Entities.Course()
                {
                    Description = this.description,
                    Id = id,
                    Name = this.name,
                    ParentCourses = this.conditions,
                    ScoIndex = this.scoIndex,
                    ScormPackage = this.scorm,
                    Thumbnail = this.thumbnail,
                    IsEnabled = true
                });
            }
            catch (Exception) { throw; }
        }

        internal void SetManifest(string path)
        {
            path = path + this.scorm.Remove(this.scorm.Length - 4, 4);
            string fileName = "imsmanifest";
            string manifest = string.Empty;
            foreach (string item in System.IO.Directory.EnumerateFiles(path))
            {
                if (item.Contains(fileName))
                {
                    manifest = item;
                    break;
                }

            }

            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.Load(manifest);
            System.Xml.XmlNode node = document.DocumentElement.SelectSingleNode("resources");
            
            foreach (System.Xml.XmlNode item in document.ChildNodes[1].ChildNodes)
            {
                if (item.Name.Equals("resources"))
                {
                    node = item;
                        break;
                }
                    
            }

            this.scoIndex = node.FirstChild.FirstChild.Attributes["href"].Value;
            

        }

        internal void Delete()
        {
            try
            {
                Entities.Course course = Business.CourseController.GetCourses().Single(c => c.Id.Equals(int.Parse(this.id)));
                course.IsEnabled = false;
                Business.CourseController.SaveCourse(course);
            }
            catch (Exception) { throw; }
        }
    }
}