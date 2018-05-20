using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRAKO_IvanCicek.Interfaces
{
    public interface IHashingStrategy
    {
        string Hash(String input, string salt);
    }
}
