using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System.Linq;

namespace NRAKO_IvanCicek.DAL
{
    public class LoginDal : ILoginDAL
    {
        Context Context;
        public LoginDal(Context context = null)
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

        public static LoginDal GetInstance(Context context = null)
        {
            return new LoginDal(context);
        }

        public LoginUser LoginCheck(Login login)
        {            
            User user = Context.Users.FirstOrDefault(x => x.Email == login.Email && x.RecordStatusId == (int)RecordStatus.Active);

            if (user != null && Hashing.Hash(login.Password, user.Salt) == user.Password)
            {
                return new LoginUser { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserTypeId = user.UserTypeId,UserId = user.UserId };
            }

            return null;
        }
    }
}