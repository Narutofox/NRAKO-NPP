using System;

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
                    var mailAddress = new System.Net.Mail.MailAddress(email);
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