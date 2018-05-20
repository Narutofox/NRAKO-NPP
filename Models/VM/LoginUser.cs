using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models.VM
{
    public class LoginUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }        
        public int UserTypeId { get; set; }
        //public string ProfileImagePath { get; set; }
        
        public string Fullname
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}