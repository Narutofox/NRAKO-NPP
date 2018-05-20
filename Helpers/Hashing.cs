using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
// TODO Strategy pattern
namespace NRAKO_IvanCicek.Helpers
{
    public static class Hashing
    {
        private static IHashingStrategy HashingStrategy = new HashingStrategySHA256();
        private static ISaltingStrategy SaltingStrategy = new SaltingStrategy128();
        public static string Hash(String input,string salt, IHashingStrategy strategy = null)
        {
            if (strategy != null)
            {
                return strategy.Hash(input, salt);
            }
            else
            {
                return HashingStrategy.Hash(input, salt);
            }         
        }


        public static string GetSalt(ISaltingStrategy strategy = null)
        {
            if (strategy != null)
            {
                return strategy.GetSalt();
            }
            else
            {
                return SaltingStrategy.GetSalt();
            }        
        }

    }
}