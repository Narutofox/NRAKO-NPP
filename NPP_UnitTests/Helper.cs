using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    public static class Helper
    {
        public static Context GetContext()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "nrakoivancicek",
                DataSource = "den1.mssql6.gear.host",
                UserID = "nrakoivancicek",
                Password = "Sp9p_~ddWAkq"
            };

            return new Context(builder.ConnectionString);
        }

        public static LoginUser GetLoginUserAdmin()
        {
            return new LoginUser(){Email = "test@nrako.com",FirstName = "Ivan",LastName = "Čiček", UserId  = 2, UserTypeId = 50};
        }

        public static LoginUser GetLoginUser()
        {
            return new LoginUser() { Email = "test2@nrako.com", FirstName = "Naruto", LastName = "Uzumaki", UserId = 3, UserTypeId = 10 };
        }
    }
}
