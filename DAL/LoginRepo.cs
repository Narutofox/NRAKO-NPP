using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System.Linq;

namespace NRAKO_IvanCicek.DAL
{
    public class LoginRepo : ILoginDAL
    {
        readonly Context _context;
        public LoginRepo(Context context = null)
        {
            if (context != null)
            {
                _context = context;
            }
            else
            {
                _context = new Context();
            }
        }

        public static LoginRepo GetInstance(Context context = null)
        {
            return new LoginRepo(context);
        }

        public LoginUser LoginCheck(Login login)
        {            
            User user = _context.Users.FirstOrDefault(x => x.Email == login.Email 
                                                           && x.RecordStatusId == (int)RecordStatus.Active);

            if (user != null && Hashing.Hash(login.Password, user.Salt) == user.Password)
            {
                return new LoginUser
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserTypeId = user.UserTypeId,
                    UserId = user.UserId
                };
            }

            return null;
        }
    }
}