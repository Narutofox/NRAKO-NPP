using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRAKO_IvanCicek.Interfaces
{
    public interface ILoginDAL
    {
        LoginUser LoginCheck(Login login);
    }
}
