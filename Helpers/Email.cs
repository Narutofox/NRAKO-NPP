using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Helpers
{
    public static class Email
    {
        public static bool IsValid(string email)
        {
            try
            {
                if (!String.IsNullOrEmpty(email))
                {
                    var Address = new System.Net.Mail.MailAddress(email);
                    return true;
                }

            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}