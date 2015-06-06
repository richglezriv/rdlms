using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class CourseModel : IDataModel
    {
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

        public void LoadValues(Newtonsoft.Json.Linq.JObject json)
        {
            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                this.GetType().GetProperty(p.Name).SetValue(this, json["data"][p.Name] != null ? json["data"][p.Name].ToString() : null);
            }
            
        }
        
    }
}