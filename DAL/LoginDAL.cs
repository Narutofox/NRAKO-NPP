using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.DAL
{
    public class LoginDAL : ILoginDAL
    {
        Context Context;
        public LoginDAL(Context context = null)
        {
            if (context != null)
            {
                Context = context;
            }
            else
            {
                Context = new Context();
            }
        }

        public static LoginDAL GetInstance(Context context = null)
        {
            return new LoginDAL(context);
        }

        public LoginUser LoginCheck(Login login)
        {            
            User User = Context.Users.Where(x => x.Email == login.Email && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();

            if (User != null && Hashing.Hash(login.Password, User.Salt) == User.Password)
            {
                return new LoginUser { Email = User.Email, FirstName = User.FirstName, LastName = User.LastName, UserTypeId = User.UserTypeId,UserId = User.UserId };
            }

            return null;
        }
    }
}