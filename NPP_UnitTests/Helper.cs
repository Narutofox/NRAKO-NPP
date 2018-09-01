using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRAKO_IvanCicek.Models;

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
    }
}
