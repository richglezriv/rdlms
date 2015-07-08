using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.LMS.Models
{
    public class UserModel:IDataModel   
    {

        public string id
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string lastName { get; set; }

        public string secondLastName { get; set; }

        public string email { get; set; }

        public string extra { get; set; }

        public List<IDataModel> Users { get; private set; }

        public UserModel() { }

        public UserModel(Entities.User user)
        {
            this.id = user.Id.ToString();
            this.name = user.FirstName;
            this.email = string.Empty;
            this.extra = string.Empty;
            this.lastName = user.LastName;
            this.secondLastName = string.Empty;
            
        }

        public void DoSearch(string likeName)
        {
            this.Users = new List<IDataModel>();
            foreach(Entities.User item in Business.UserController.GetUsers(likeName))
            {
                this.Users.Add(new UserModel(item));
            }
        }
    }
}