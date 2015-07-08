using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class LMSModel
    {
        private Dictionary<String, String> myDictionary = new Dictionary<string, string>();
        
        public String Children { get; set; }
        public String StudentId { get; set; }
        public String StudentName { get; set; }
        public String LessonLocation { get; set; }
        public String Credit { get; set; }
        public String LessonStatus { get; set; }
        public String Entry { get; set; }
        public String ScoreChildren { get; set; }
        public String ScoreRaw { get; set; }
        public String ScoreMin { get; set; }
        public String ScoreMax { get; set; }
        public String TotalTime { get; set; }
        public String LessonMode { get; set; }
        public String Exit { get; set; }
        public String SessionTime { get; set; }
        public String SuspendData { get; set; }
        public String LaunchData { get; set; }
        
        private String GetScormName(String propertyName)
        {
            return myDictionary.First(r => r.Key.Equals(propertyName)).Value;
        }

        public void LoadCourse(Entities.UserCourse course)
        {
            this.Credit = course.Scorm.Credit;
            this.Entry = course.Scorm.Entry;
            this.Exit = course.Scorm.Exit;
            this.LaunchData = course.Scorm.LaunchData;
            this.LessonLocation = course.Scorm.LessonLocation;
            this.LessonStatus = course.Status;
            this.ScoreMax = course.Scorm.ScoreMax;
            this.ScoreMin = course.Scorm.ScoreMin;
            this.ScoreRaw = course.Scorm.ScoreRaw;
            this.SessionTime = course.Scorm.SessionTime;
            this.StudentId = course.UserId.ToString();
            this.SuspendData = course.Scorm.SuspendData;
            this.TotalTime = course.Scorm.TotalTime;
            
        }

        public void LoadValues(Newtonsoft.Json.Linq.JObject collection) {

            String value;

            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                value = collection[GetScormName(p.Name)] != null ? 
                    collection[GetScormName(p.Name)].ToString() : String.Empty;
                p.SetValue(this, value);
            }
        }

        public string GenerateJSonString()
        {
            String json = "{";

            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                json += Char.ToString('"') + GetScormName(p.Name) + Char.ToString('"') + ":" +
                    Char.ToString('"') + p.GetValue(this) + Char.ToString('"') + ",";
            }

            json = json.Substring(0, json.Length - 1);

            json += "}";

            return json;
        }

        public LMSModel()
        {
            this.myDictionary.Add("Children", "cmi.core._children");
            this.myDictionary.Add("StudentId", "cmi.core.student_id");
            this.myDictionary.Add("StudentName", "cmi.core.student_name");
            this.myDictionary.Add("LessonLocation", "cmi.core.lesson_location");
            this.myDictionary.Add("Credit", "cmi.core.credit");
            this.myDictionary.Add("LessonStatus", "cmi.core.lesson_status");
            this.myDictionary.Add("Entry", "cmi.core.entry");
            this.myDictionary.Add("ScoreChildren", "cmi.core.score._children");
            this.myDictionary.Add("ScoreRaw", "cmi.core.score.raw");
            this.myDictionary.Add("ScoreMin", "cmi.core.score.min");
            this.myDictionary.Add("ScoreMax", "cmi.core.score.max");
            this.myDictionary.Add("TotalTime", "cmi.core.total_time");
            this.myDictionary.Add("LessonMode", "cmi.core.lesson_mode");
            this.myDictionary.Add("Exit", "cmi.core.exit");
            this.myDictionary.Add("SessionTime", "cmi.core.session_time");
            this.myDictionary.Add("SuspendData", "cmi.suspend_data");
            this.myDictionary.Add("LaunchData", "cmi.launch_data");
        }

        internal void EvalCourse(Entities.UserCourse course)
        {
            course.Status = this.LessonStatus;
            course.Scorm.LessonLocation = this.LessonLocation;
            course.Scorm.Credit = this.Credit;
            course.Scorm.Entry = this.Entry;
            course.Scorm.Exit = this.Exit;
            course.Scorm.LaunchData = this.LaunchData;
            course.Scorm.ScoreMax = this.ScoreMax;
            course.Scorm.ScoreMin = this.ScoreMin;
            course.Scorm.ScoreRaw = this.ScoreRaw;
            course.Scorm.SessionTime = this.SessionTime;
            course.Scorm.SuspendData = this.SuspendData;
            course.Scorm.TotalTime = this.TotalTime;
            
            Business.UserController.UpdateUserCourse(course);
        }
    }
}